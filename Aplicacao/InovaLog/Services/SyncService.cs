using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using InovaLog.Models;

namespace InovaLog.Services
{
    public class SyncService
    {
        private readonly DatabaseService _db;
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseAnonKey;
        private readonly SemaphoreSlim _syncLock = new(1, 1);

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public bool EstaSincronizando { get; private set; }
        public event Action<string> StatusSincronizacaoAlterado;

        public SyncService(DatabaseService db, HttpClient httpClient, string supabaseUrl, string supabaseAnonKey)
        {
            _db = db;
            _httpClient = httpClient;
            _supabaseUrl = supabaseUrl.TrimEnd('/');
            _supabaseAnonKey = supabaseAnonKey;
        }

        /// <summary>
        /// Ativa o monitoramento automático de internet para sincronizar assim que o sinal voltar.
        /// </summary>
        private bool _monitorando;

        public void IniciarMonitoramentoConexao()
        {
            if (_monitorando) return;
            _monitorando = true;

            Connectivity.Current.ConnectivityChanged += async (sender, e) =>
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
                {
                    await SincronizarTudoAsync();
                }
            };
        }

        /// <summary>
        /// Executa o ciclo completo: primeiro envia as pendências locais (Push), depois baixa atualizações (Pull).
        /// </summary>
        public async Task<bool> SincronizarTudoAsync()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return false;

            // Evita duas sincronizações simultâneas se a rede oscilar
            if (!await _syncLock.WaitAsync(0))
                return false;

            try
            {
                EstaSincronizando = true;
                StatusSincronizacaoAlterado?.Invoke("Sincronizando dados...");

                await ConfigurarHeadersAutenticacaoAsync();

                // 1. PUSH (SQLite -> Supabase) respeitando a ordem de Chaves Estrangeiras
                await PushTabelaAsync<Motorista>("motoristas");
                await PushTabelaAsync<Veiculo>("veiculos");
                await PushTabelaAsync<Rota>("rotas");
                await PushTabelaAsync<PlanoManutencao>("planos_manutencao");
                await PushTabelaAsync<Viagem>("viagens");
                await PushTabelaAsync<HistoricoManutencao>("historico_manutencoes");
                await PushTabelaAsync<Divida>("dividas");
                await PushTabelaAsync<Movimentacao>("movimentacoes");
                await PushTabelaAsync<AuditoriaLog>("auditoria_logs");

                // 2. PULL (Supabase -> SQLite)
                await PullTabelaAsync<Perfil>("perfis");
                await PullTabelaAsync<Categoria>("categorias");
                await PullTabelaAsync<FormaPagamento>("formas_pagamento");
                await PullTabelaAsync<Usuario>("usuarios");
                await PullTabelaAsync<Motorista>("motoristas");
                await PullTabelaAsync<Veiculo>("veiculos");
                await PullTabelaAsync<Rota>("rotas");
                await PullTabelaAsync<PlanoManutencao>("planos_manutencao");
                await PullTabelaAsync<Viagem>("viagens");
                await PullTabelaAsync<HistoricoManutencao>("historico_manutencoes");
                await PullTabelaAsync<Divida>("dividas");
                await PullTabelaAsync<Movimentacao>("movimentacoes");

                StatusSincronizacaoAlterado?.Invoke("Dados atualizados");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncService] Erro na sincronização: {ex.Message}");
                StatusSincronizacaoAlterado?.Invoke("Erro ao sincronizar");
                return false;
            }
            finally
            {
                EstaSincronizando = false;
                _syncLock.Release();
            }
        }

        /// <summary>
        /// Envia apenas os registros criados ou editados offline de uma tabela específica.
        /// </summary>
        public async Task PushTabelaAsync<T>(string nomeTabela) where T : BaseModel, new()
        {
            var pendentes = await _db.GetPendentesSincronizacaoAsync<T>();
            if (pendentes == null || pendentes.Count == 0)
                return;

            var url = $"{_supabaseUrl}/rest/v1/{nomeTabela}";

            foreach (var item in pendentes)
            {
                try
                {
                    var json = JsonSerializer.Serialize(item, _jsonOptions);
                    using var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    // Faz Upsert no Supabase (insere se não existir, atualiza se o ID já existir)
                    request.Headers.Add("Prefer", "resolution=merge-duplicates");

                    var response = await _httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        item.Sincronizado = true;
                        item.PrecisaAtualizar = false;
                        await _db.SaveAsync(item);
                    }
                    else
                    {
                        var erro = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"[SyncService] Falha no PUSH de {nomeTabela}: {response.StatusCode} - {erro}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SyncService] Exceção no PUSH de {nomeTabela}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Baixa os dados da nuvem e atualiza o cache local sem sobrescrever registros que ainda estão pendentes de envio.
        /// </summary>
        public async Task PullTabelaAsync<T>(string nomeTabela) where T : BaseModel, new()
        {
            try
            {
                var url = $"{_supabaseUrl}/rest/v1/{nomeTabela}?select=*";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[SyncService] Falha no PULL de {nomeTabela}: {response.StatusCode}");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var itensNuvem = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);

                if (itensNuvem == null || itensNuvem.Count == 0)
                    return;

                // Proteção: não sobrescreve itens locais que falharam no Push e ainda precisam subir
                var pendentesLocais = await _db.GetPendentesSincronizacaoAsync<T>();
                var idsPendentes = new HashSet<string>(
                    pendentesLocais.Select(p => ObterIdComoString(p)).Where(id => id != null)
                );

                var itensParaSalvar = new List<T>();
                foreach (var item in itensNuvem)
                {
                    var id = ObterIdComoString(item);
                    if (id != null && idsPendentes.Contains(id))
                        continue;

                    item.Sincronizado = true;
                    item.PrecisaAtualizar = false;
                    itensParaSalvar.Add(item);
                }

                if (itensParaSalvar.Count > 0)
                {
                    await _db.SaveAllAsync(itensParaSalvar);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncService] Exceção no PULL de {nomeTabela}: {ex.Message}");
            }
        }

        private async Task ConfigurarHeadersAutenticacaoAsync()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseAnonKey);

            // Usa o JWT do usuário logado (se disponível no SecureStorage) para respeitar as políticas RLS do Supabase
            var token = await SecureStorage.Default.GetAsync("auth_token");
            var bearer = string.IsNullOrWhiteSpace(token) ? _supabaseAnonKey : token;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        private static string ObterIdComoString<T>(T item)
        {
            var prop = typeof(T).GetProperty("Id");
            return prop?.GetValue(item)?.ToString();
        }
    }
}