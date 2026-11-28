using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Models;
using InovaLog.Services;
using InovaLog.Views;
using Microsoft.Maui.Graphics;

namespace InovaLog.ViewModels;

public partial class UsuarioItem : ObservableObject
{
    public string Id { get; init; } = string.Empty;
    public string NomeExibicao { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Documentos { get; init; } = string.Empty;
    public string RotuloPerfil { get; init; } = string.Empty;
    public Usuario ModelOriginal { get; init; } = null!;

    [ObservableProperty] private bool ativo = true;

    public bool TemDocumentos => !string.IsNullOrWhiteSpace(Documentos);

    public string TextoStatus => Ativo ? "Ativo" : "Bloqueado";
    public Color CorFundoStatus => Ativo ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoStatus => Ativo ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    public string TextoAcao => Ativo ? "Bloquear" : "Ativar";
    public Color CorFundoAcao => Ativo ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#D1FAE5");
    public Color CorTextoAcao => Ativo ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");

    partial void OnAtivoChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoStatus));
        OnPropertyChanged(nameof(CorFundoStatus));
        OnPropertyChanged(nameof(CorTextoStatus));
        OnPropertyChanged(nameof(TextoAcao));
        OnPropertyChanged(nameof(CorFundoAcao));
        OnPropertyChanged(nameof(CorTextoAcao));
    }
}

public partial class CategoriaItem : ObservableObject
{
    public int Id { get; init; }
    public bool Entrada { get; init; }
    public Categoria ModelOriginal { get; init; } = null!;

    [ObservableProperty] private string nome = string.Empty;
    [ObservableProperty] private bool ativa = true;

    public string TipoTexto => Entrada ? "ENTRADA" : "SAÍDA";
    public Color CorFundoTipo => Entrada ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoTipo => Entrada ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    public string TextoBotao => Ativa ? "Inativar" : "Reativar";
    public Color CorFundoBotao => Ativa ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#D1FAE5");
    public Color CorTextoBotao => Ativa ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;

    partial void OnAtivaChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoBotao));
        OnPropertyChanged(nameof(CorFundoBotao));
        OnPropertyChanged(nameof(CorTextoBotao));
        OnPropertyChanged(nameof(OpacidadeLinha));
    }
}

public partial class FormaPagamentoItem : ObservableObject
{
    public int Id { get; init; }
    public FormaPagamento ModelOriginal { get; init; } = null!;

    [ObservableProperty] private string nome = string.Empty;
    [ObservableProperty] private bool ativa = true;

    public string TextoStatus => Ativa ? "Ativo" : "Inativo";
    public Color CorFundoStatus => Ativa ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#F1F5F9");
    public Color CorTextoStatus => Ativa ? Color.FromArgb("#10B981") : Color.FromArgb("#64748B");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;

    partial void OnAtivaChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoStatus));
        OnPropertyChanged(nameof(CorFundoStatus));
        OnPropertyChanged(nameof(CorTextoStatus));
        OnPropertyChanged(nameof(OpacidadeLinha));
    }
}

public class LogItem
{
    public string Acao { get; init; } = string.Empty;
    public string Tabela { get; init; } = string.Empty;
    public string UsuarioNome { get; init; } = string.Empty;
    public string Detalhes { get; init; } = string.Empty;
    public string DataTexto { get; init; } = string.Empty;
    public bool TemDetalhes => !string.IsNullOrWhiteSpace(Detalhes);
}

public partial class ConfiguracoesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly Supabase.Client _supabaseClient;

    public ObservableCollection<UsuarioItem> Usuarios { get; } = new();
    public ObservableCollection<CategoriaItem> Categorias { get; } = new();
    public ObservableCollection<FormaPagamentoItem> FormasPagamento { get; } = new();
    public ObservableCollection<LogItem> Logs { get; } = new();

    [ObservableProperty] private string novaCategoria = string.Empty;
    [ObservableProperty] private int tipoCategoriaIndex = 1; // 0 = Entrada, 1 = Saída
    [ObservableProperty] private string novaFormaPagamento = string.Empty;

    [ObservableProperty] private string nomeConta = string.Empty;
    [ObservableProperty] private string emailConta = string.Empty;
    [ObservableProperty] private string rotuloPerfilConta = string.Empty;
    [ObservableProperty] private bool carregando;

    public ConfiguracoesViewModel(DatabaseService db, Supabase.Client supabaseClient)
    {
        _db = db;
        _supabaseClient = supabaseClient;
    }

    public async Task CarregarDadosAsync()
    {
        NomeConta = SessaoPerfil.NomeUsuario;
        EmailConta = SessaoPerfil.EmailUsuario;
        RotuloPerfilConta = SessaoPerfil.RotuloPerfil(SessaoPerfil.PerfilAtual);

        Carregando = true;
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var respPerfis = await _supabaseClient.From<Perfil>().Get();
                var respUsuarios = await _supabaseClient.From<Usuario>().Get();
                var respMotoristas = await _supabaseClient.From<Motorista>().Get();
                var respCategorias = await _supabaseClient.From<Categoria>().Get();
                var respPagamentos = await _supabaseClient.From<FormaPagamento>().Get();

                await _db.ReplaceAllSyncedAsync(respPerfis.Models);
                await _db.ReplaceAllSyncedAsync(respUsuarios.Models);
                await _db.ReplaceAllSyncedAsync(respMotoristas.Models);
                await _db.ReplaceAllSyncedAsync(respCategorias.Models);
                await _db.ReplaceAllSyncedAsync(respPagamentos.Models);

                if (SessaoPerfil.PodeGerenciarUsuarios())
                {
                    var respLogs = await _supabaseClient.From<AuditoriaLog>().Get();
                    await _db.ReplaceAllSyncedAsync(respLogs.Models);
                }
            }

            // 1. USUÁRIOS
            var perfis = (await _db.GetAllAsync<Perfil>()).ToDictionary(p => p.Id, p => p.Nome);
            var motoristas = (await _db.GetAllAsync<Motorista>()).ToDictionary(m => m.UsuarioId, m => m);
            var usuarios = await _db.GetAllAsync<Usuario>();
            var mapaUsuarios = usuarios.ToDictionary(u => u.Id, u => u.Nome);

            Usuarios.Clear();
            foreach (var u in usuarios.OrderBy(x => x.Nome))
            {
                var nomePerfil = u.PerfilId.HasValue && perfis.TryGetValue(u.PerfilId.Value, out var np)
                    ? np.ToUpperInvariant()
                    : "USUÁRIO";

                var docs = string.Empty;
                if (motoristas.TryGetValue(u.Id, out var mot))
                {
                    docs = $"CNH: {mot.CnhNumero} (Cat {mot.CnhCategoria})";
                    if (!string.IsNullOrWhiteSpace(u.CpfCnpj))
                        docs += $" • CPF: {u.CpfCnpj}";
                }
                else if (!string.IsNullOrWhiteSpace(u.CpfCnpj))
                {
                    docs = $"CPF/CNPJ: {u.CpfCnpj}";
                }

                Usuarios.Add(new UsuarioItem
                {
                    Id = u.Id,
                    NomeExibicao = u.Nome,
                    Email = u.Email,
                    RotuloPerfil = nomePerfil,
                    Documentos = docs,
                    Ativo = u.Ativo ?? true,
                    ModelOriginal = u
                });
            }

            // 2. CATEGORIAS
            var categorias = await _db.GetAllAsync<Categoria>();
            Categorias.Clear();
            foreach (var c in categorias.OrderBy(x => x.Nome))
            {
                var ehEntrada = string.Equals(c.Tipo, "ENTRADA", StringComparison.OrdinalIgnoreCase);
                Categorias.Add(new CategoriaItem
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Entrada = ehEntrada,
                    Ativa = c.Ativo ?? true,
                    ModelOriginal = c
                });
            }

            // 3. FORMAS DE PAGAMENTO
            var pagamentos = await _db.GetAllAsync<FormaPagamento>();
            FormasPagamento.Clear();
            foreach (var f in pagamentos.OrderBy(x => x.Nome))
            {
                FormasPagamento.Add(new FormaPagamentoItem
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Ativa = f.Ativo ?? true,
                    ModelOriginal = f
                });
            }

            // 4. LOGS DE AUDITORIA (apenas Admin)
            if (SessaoPerfil.PodeGerenciarUsuarios())
            {
                var logs = await _db.GetAllAsync<AuditoriaLog>();
                Logs.Clear();

                foreach (var l in logs.OrderByDescending(x => ObterDataLog(x)).Take(50))
                {
                    var uid = ObterPropString(l, "UsuarioId");
                    var nomeUser = !string.IsNullOrWhiteSpace(uid) && mapaUsuarios.TryGetValue(uid, out var n)
                        ? n
                        : "Sistema";

                    var detalhes = ObterPropString(l, "DadosNovos") ?? ObterPropString(l, "DadosAntigos") ?? string.Empty;
                    if (detalhes.Length > 140)
                        detalhes = detalhes[..140] + "...";

                    Logs.Add(new LogItem
                    {
                        Acao = ObterPropString(l, "Acao") ?? "AÇÃO",
                        Tabela = ObterPropString(l, "TabelaAfetada") ?? "-",
                        UsuarioNome = nomeUser,
                        Detalhes = detalhes,
                        DataTexto = ObterDataLog(l).ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Não foi possível carregar as configurações: {ex.Message}");
        }
        finally
        {
            Carregando = false;
        }
    }

    // ==============================================================
    // USUÁRIOS
    // ==============================================================

    [RelayCommand]
    private async Task NovoUsuarioAsync() => await Shell.Current.GoToAsync(nameof(CadastroUsuarioPage));

    [RelayCommand]
    private async Task EditarUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"{nameof(CadastroUsuarioPage)}?UsuarioId={item.Id}&Modo=editar");
    }

    [RelayCommand]
    private async Task InspecionarUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"{nameof(CadastroUsuarioPage)}?UsuarioId={item.Id}&Modo=inspecionar");
    }

    [RelayCommand]
    private async Task AlternarStatusUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;

        if (string.Equals(item.Email, SessaoPerfil.EmailUsuario, StringComparison.OrdinalIgnoreCase))
        {
            await ExibirAlerta("Ação bloqueada", "Você não pode bloquear a sua própria conta de administrador.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para alterar o status de acesso.");
            return;
        }

        try
        {
            var novoStatus = !item.Ativo;
            item.ModelOriginal.Ativo = novoStatus;
            item.ModelOriginal.AtualizadoEm = DateTime.UtcNow;

            await _supabaseClient.From<Usuario>().Update(item.ModelOriginal);
            await _db.SaveAsync(item.ModelOriginal);

            item.Ativo = novoStatus;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao atualizar status: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExcluirUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;

        if (string.Equals(item.Email, SessaoPerfil.EmailUsuario, StringComparison.OrdinalIgnoreCase))
        {
            await ExibirAlerta("Ação bloqueada", "Você não pode excluir a sua própria conta de administrador.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para excluir usuários.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        bool confirmar = await pagina.DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Deseja excluir permanentemente o usuário {item.NomeExibicao} ({item.Email})?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            await _supabaseClient.From<Usuario>().Delete(item.ModelOriginal);
            await _db.DeleteAsync(item.ModelOriginal);

            var motoristas = await _db.GetAllAsync<Motorista>();
            var motLocal = motoristas.FirstOrDefault(m => m.UsuarioId == item.Id);
            if (motLocal != null)
                await _db.DeleteAsync(motLocal);

            Usuarios.Remove(item);
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Não foi possível excluir", $"Se o usuário já possui histórico vinculado, use a opção Bloquear. Detalhe: {ex.Message}");
        }
    }

    // ==============================================================
    // CATEGORIAS (BANCO REAL)
    // ==============================================================

    [RelayCommand]
    private async Task IncluirCategoriaAsync()
    {
        var nome = (NovaCategoria ?? string.Empty).Trim();
        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe o nome da categoria.");
            return;
        }

        if (Categorias.Any(c => string.Equals(c.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa categoria já existe.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para cadastrar categorias.");
            return;
        }

        try
        {
            var ehEntrada = TipoCategoriaIndex == 0;
            var nova = new Categoria
            {
                Nome = nome,
                Tipo = ehEntrada ? "ENTRADA" : "SAIDA",
                Ativo = true,
                Sincronizado = true,
                PrecisaAtualizar = false
            };

            var resp = await _supabaseClient.From<Categoria>().Insert(nova);
            var salva = resp.Models.FirstOrDefault() ?? nova;
            await _db.SaveAsync(salva);

            Categorias.Add(new CategoriaItem
            {
                Id = salva.Id,
                Nome = salva.Nome,
                Entrada = ehEntrada,
                Ativa = salva.Ativo ?? true,
                ModelOriginal = salva
            });

            NovaCategoria = string.Empty;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Não foi possível salvar a categoria: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AlternarCategoriaAsync(CategoriaItem? categoria)
    {
        if (categoria == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para alterar a categoria.");
            return;
        }

        try
        {
            var novoStatus = !categoria.Ativa;
            categoria.ModelOriginal.Ativo = novoStatus;

            await _supabaseClient.From<Categoria>().Update(categoria.ModelOriginal);
            await _db.SaveAsync(categoria.ModelOriginal);

            categoria.Ativa = novoStatus;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao atualizar categoria: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditarCategoriaAsync(CategoriaItem? categoria)
    {
        if (categoria == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para editar categorias.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        string? novoNome = await pagina.DisplayPromptAsync(
            "Editar Categoria",
            "Informe o novo nome da categoria:",
            "Salvar",
            "Cancelar",
            initialValue: categoria.Nome);

        if (string.IsNullOrWhiteSpace(novoNome)) return;
        novoNome = novoNome.Trim();

        if (string.Equals(novoNome, categoria.Nome, StringComparison.OrdinalIgnoreCase)) return;

        if (Categorias.Any(c => c.Id != categoria.Id && string.Equals(c.Nome, novoNome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Já existe uma categoria com esse nome.");
            return;
        }

        try
        {
            categoria.ModelOriginal.Nome = novoNome;
            await _supabaseClient.From<Categoria>().Update(categoria.ModelOriginal);
            await _db.SaveAsync(categoria.ModelOriginal);
            categoria.Nome = novoNome;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao editar categoria: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExcluirCategoriaAsync(CategoriaItem? categoria)
    {
        if (categoria == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para excluir categorias.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        bool confirmar = await pagina.DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Deseja excluir a categoria \"{categoria.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            await _supabaseClient.From<Categoria>().Delete(categoria.ModelOriginal);
            await _db.DeleteAsync(categoria.ModelOriginal);
            Categorias.Remove(categoria);
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Não foi possível excluir", $"Se já existem lançamentos vinculados a esta categoria, use a opção Inativar. Detalhe: {ex.Message}");
        }
    }

    // ==============================================================
    // FORMAS DE PAGAMENTO (BANCO REAL)
    // ==============================================================

    [RelayCommand]
    private async Task IncluirFormaPagamentoAsync()
    {
        var nome = (NovaFormaPagamento ?? string.Empty).Trim();
        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe a forma de pagamento.");
            return;
        }

        if (FormasPagamento.Any(f => string.Equals(f.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa forma de pagamento já existe.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para cadastrar formas de pagamento.");
            return;
        }

        try
        {
            var nova = new FormaPagamento
            {
                Nome = nome,
                Ativo = true,
                Sincronizado = true,
                PrecisaAtualizar = false
            };

            var resp = await _supabaseClient.From<FormaPagamento>().Insert(nova);
            var salva = resp.Models.FirstOrDefault() ?? nova;
            await _db.SaveAsync(salva);

            FormasPagamento.Add(new FormaPagamentoItem
            {
                Id = salva.Id,
                Nome = salva.Nome,
                Ativa = salva.Ativo ?? true,
                ModelOriginal = salva
            });

            NovaFormaPagamento = string.Empty;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Não foi possível salvar a forma de pagamento: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AlternarFormaPagamentoAsync(FormaPagamentoItem? forma)
    {
        if (forma == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para alterar a forma de pagamento.");
            return;
        }

        try
        {
            var novoStatus = !forma.Ativa;
            forma.ModelOriginal.Ativo = novoStatus;

            await _supabaseClient.From<FormaPagamento>().Update(forma.ModelOriginal);
            await _db.SaveAsync(forma.ModelOriginal);

            forma.Ativa = novoStatus;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao atualizar forma de pagamento: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditarFormaPagamentoAsync(FormaPagamentoItem? forma)
    {
        if (forma == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para editar formas de pagamento.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        string? novoNome = await pagina.DisplayPromptAsync(
            "Editar Forma de Pagamento",
            "Informe o novo nome:",
            "Salvar",
            "Cancelar",
            initialValue: forma.Nome);

        if (string.IsNullOrWhiteSpace(novoNome)) return;
        novoNome = novoNome.Trim();

        if (string.Equals(novoNome, forma.Nome, StringComparison.OrdinalIgnoreCase)) return;

        if (FormasPagamento.Any(f => f.Id != forma.Id && string.Equals(f.Nome, novoNome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Já existe uma forma de pagamento com esse nome.");
            return;
        }

        try
        {
            forma.ModelOriginal.Nome = novoNome;
            await _supabaseClient.From<FormaPagamento>().Update(forma.ModelOriginal);
            await _db.SaveAsync(forma.ModelOriginal);
            forma.Nome = novoNome;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao editar forma de pagamento: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExcluirFormaPagamentoAsync(FormaPagamentoItem? forma)
    {
        if (forma == null) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para excluir formas de pagamento.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        bool confirmar = await pagina.DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Deseja excluir a forma de pagamento \"{forma.Nome}\"?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            await _supabaseClient.From<FormaPagamento>().Delete(forma.ModelOriginal);
            await _db.DeleteAsync(forma.ModelOriginal);
            FormasPagamento.Remove(forma);
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Não foi possível excluir", $"Se já existem lançamentos vinculados a esta forma de pagamento, inative-a em vez de excluir. Detalhe: {ex.Message}");
        }
    }

    // ==============================================================
    // APOIO
    // ==============================================================

    private static string? ObterPropString(object obj, string nomeProp) =>
        obj.GetType().GetProperty(nomeProp)?.GetValue(obj)?.ToString();

    private static DateTime ObterDataLog(object obj)
    {
        var prop = obj.GetType().GetProperty("CriadoEm") ?? obj.GetType().GetProperty("DataHora");
        return prop?.GetValue(obj) is DateTime dt ? dt : DateTime.MinValue;
    }

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;
        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }
}