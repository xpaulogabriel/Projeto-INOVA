using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Models;
using InovaLog.Services;
using static Postgrest.Constants;

namespace InovaLog.ViewModels;

[QueryProperty(nameof(UsuarioId), "UsuarioId")]
[QueryProperty(nameof(Modo), "Modo")]
public partial class CadastroUsuarioViewModel : ObservableObject
{
    private readonly Supabase.Client _supabaseClient;
    private readonly DatabaseService _db;
    private Usuario? _usuarioEmEdicao;
    private Motorista? _motoristaEmEdicao;

    public ObservableCollection<Perfil> Perfis { get; } = new();

    [ObservableProperty] private string? usuarioId;
    [ObservableProperty] private string modo = "criar"; // "criar", "editar", "inspecionar"

    // Dados gerais (tabela usuarios)
    [ObservableProperty] private string nome = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string senha = string.Empty;
    [ObservableProperty] private string cpfCnpj = string.Empty;
    [ObservableProperty] private DateTime dataNascimento = new(1995, 1, 1);
    [ObservableProperty] private string telefone = string.Empty;
    [ObservableProperty] private Perfil? perfilSelecionado;
    [ObservableProperty] private bool ehMotorista;

    // Dados específicos (tabela motoristas)
    [ObservableProperty] private string cnhNumero = string.Empty;
    [ObservableProperty] private DateTime cnhValidade = DateTime.Today.AddYears(2);
    [ObservableProperty] private string cnhCategoria = "D";
    [ObservableProperty] private bool ear = true;

    // Controle de tela
    [ObservableProperty] private bool ativo = true;
    [ObservableProperty] private bool estaSalvando;

    public bool EhCriacao => string.Equals(Modo, "criar", StringComparison.OrdinalIgnoreCase);
    public bool EhInspecao => string.Equals(Modo, "inspecionar", StringComparison.OrdinalIgnoreCase);
    public bool PodeEditar => !EhInspecao;
    public bool MostrarBotaoSalvar => !EhInspecao;
    public bool PodeSalvar => !EstaSalvando && !EhInspecao;

    public string TituloPagina => Modo?.ToLowerInvariant() switch
    {
        "editar" => "Editar Usuário",
        "inspecionar" => "Inspecionar Usuário",
        _ => "Novo cadastro de Usuário"
    };

    public string TextoBotaoSalvar => EhCriacao ? "Cadastrar" : "Salvar";
    public string TextoBotaoCancelar => EhInspecao ? "Fechar" : "Cancelar";
    public string TextoStatus => Ativo ? "Status de Acesso: Ativo" : "Status de Acesso: Bloqueado";

    public CadastroUsuarioViewModel(Supabase.Client supabaseClient, DatabaseService db)
    {
        _supabaseClient = supabaseClient;
        _db = db;
    }

    partial void OnAtivoChanged(bool value) => OnPropertyChanged(nameof(TextoStatus));
    partial void OnEstaSalvandoChanged(bool value) => OnPropertyChanged(nameof(PodeSalvar));

    partial void OnModoChanged(string value)
    {
        OnPropertyChanged(nameof(EhCriacao));
        OnPropertyChanged(nameof(EhInspecao));
        OnPropertyChanged(nameof(PodeEditar));
        OnPropertyChanged(nameof(MostrarBotaoSalvar));
        OnPropertyChanged(nameof(PodeSalvar));
        OnPropertyChanged(nameof(TituloPagina));
        OnPropertyChanged(nameof(TextoBotaoSalvar));
        OnPropertyChanged(nameof(TextoBotaoCancelar));
    }

    partial void OnPerfilSelecionadoChanged(Perfil? value)
    {
        EhMotorista = value != null &&
                      !string.IsNullOrWhiteSpace(value.Nome) &&
                      value.Nome.Contains("motorista", StringComparison.OrdinalIgnoreCase);
    }

    public async Task CarregarPerfisAsync()
    {
        if (Perfis.Count == 0)
        {
            var listaLocal = await _db.GetAllAsync<Perfil>();

            if (listaLocal.Count == 0 && Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var resp = await _supabaseClient.From<Perfil>().Get();
                listaLocal = resp.Models;
                if (listaLocal.Count > 0)
                    await _db.SaveAllAsync(listaLocal);
            }

            Perfis.Clear();
            foreach (var p in listaLocal)
                Perfis.Add(p);
        }

        // Se veio com UsuarioId (Editar ou Inspecionar), carrega os dados do usuário
        if (!string.IsNullOrWhiteSpace(UsuarioId))
        {
            await CarregarUsuarioExistenteAsync(UsuarioId);
        }
        else if (PerfilSelecionado == null)
        {
            PerfilSelecionado = Perfis.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.Nome) &&
                p.Nome.Contains("motorista", StringComparison.OrdinalIgnoreCase))
                ?? Perfis.FirstOrDefault();
        }
    }

    private async Task CarregarUsuarioExistenteAsync(string id)
    {
        _usuarioEmEdicao = await _db.GetByIdAsync<Usuario>(id);
        if (_usuarioEmEdicao == null) return;

        Nome = _usuarioEmEdicao.Nome ?? string.Empty;
        Email = _usuarioEmEdicao.Email ?? string.Empty;
        CpfCnpj = _usuarioEmEdicao.CpfCnpj ?? string.Empty;
        Telefone = _usuarioEmEdicao.Telefone ?? string.Empty;
        DataNascimento = _usuarioEmEdicao.DataNascimento ?? new DateTime(1995, 1, 1);
        Ativo = _usuarioEmEdicao.Ativo ?? true;

        PerfilSelecionado = Perfis.FirstOrDefault(p => p.Id == _usuarioEmEdicao.PerfilId);

        var motoristas = await _db.GetAllAsync<Motorista>();
        _motoristaEmEdicao = motoristas.FirstOrDefault(m => m.UsuarioId == id);

        if (_motoristaEmEdicao != null)
        {
            CnhNumero = _motoristaEmEdicao.CnhNumero ?? string.Empty;
            CnhCategoria = _motoristaEmEdicao.CnhCategoria ?? "D";
            CnhValidade = _motoristaEmEdicao.CnhValidade ?? DateTime.Today.AddYears(2);
            Ear = _motoristaEmEdicao.Ear ?? true;
        }
    }

    [RelayCommand]
    private async Task CadastrarAsync()
    {
        if (EstaSalvando || EhInspecao) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para salvar usuários.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || PerfilSelecionado == null)
        {
            await ExibirAlerta("Campos obrigatórios", "Preencha Nome, Perfil de Acesso e E-mail.");
            return;
        }

        if (EhCriacao && (string.IsNullOrWhiteSpace(Senha) || Senha.Length < 6))
        {
            await ExibirAlerta("Senha obrigatória", "Defina uma senha com pelo menos 6 caracteres para o novo usuário.");
            return;
        }

        if (EhMotorista && string.IsNullOrWhiteSpace(CnhNumero))
        {
            await ExibirAlerta("Dados do Motorista", "Informe o Nº da CNH para o perfil Motorista.");
            return;
        }

        var emailLimpo = Email.Trim().ToLowerInvariant();
        var agora = DateTime.UtcNow;

        EstaSalvando = true;
        try
        {
            if (EhCriacao)
            {
                // 1. Verifica duplicidade de e-mail
                var existente = await _supabaseClient
                    .From<Usuario>()
                    .Filter("email", Operator.ILike, emailLimpo)
                    .Get();

                if (existente.Models.Any())
                {
                    await ExibirAlerta("E-mail já cadastrado", "Já existe um usuário com este e-mail no InovaLog.");
                    return;
                }

                // 2. Cria a conta com senha no Supabase Auth sem trocar a sessão do Admin
                var (criouAuth, erroAuth) = await CriarContaAuthIsoladaAsync(emailLimpo, Senha);
                if (!criouAuth)
                {
                    await ExibirAlerta("Erro na Autenticação", $"Não foi possível criar a senha de acesso: {erroAuth}");
                    return;
                }

                // 3. Insere em 'usuarios'
                var novoUsuario = new Usuario
                {
                    Id = Guid.NewGuid().ToString(),
                    Nome = Nome.Trim(),
                    Email = emailLimpo,
                    CpfCnpj = string.IsNullOrWhiteSpace(CpfCnpj) ? null : CpfCnpj.Trim(),
                    DataNascimento = DataNascimento,
                    Telefone = string.IsNullOrWhiteSpace(Telefone) ? null : Telefone.Trim(),
                    PerfilId = PerfilSelecionado.Id,
                    Ativo = Ativo,
                    CriadoEm = agora,
                    AtualizadoEm = agora,
                    Sincronizado = true,
                    PrecisaAtualizar = false
                };

                var respUsuario = await _supabaseClient.From<Usuario>().Insert(novoUsuario);
                var usuarioSalvo = respUsuario.Models.FirstOrDefault() ?? novoUsuario;
                await _db.SaveAsync(usuarioSalvo);

                // 4. Se for Motorista, insere em 'motoristas'
                if (EhMotorista)
                {
                    var novoMotorista = new Motorista
                    {
                        Id = Guid.NewGuid().ToString(),
                        UsuarioId = usuarioSalvo.Id,
                        CnhNumero = CnhNumero.Trim(),
                        CnhCategoria = string.IsNullOrWhiteSpace(CnhCategoria) ? "D" : CnhCategoria,
                        CnhValidade = CnhValidade,
                        Ear = Ear,
                        StatusOperacional = "Disponível",
                        CriadoEm = agora,
                        AtualizadoEm = agora,
                        Sincronizado = true,
                        PrecisaAtualizar = false
                    };

                    var respMotorista = await _supabaseClient.From<Motorista>().Insert(novoMotorista);
                    await _db.SaveAsync(respMotorista.Models.FirstOrDefault() ?? novoMotorista);
                }

                await ExibirAlerta("Sucesso", $"Usuário {usuarioSalvo.Nome} cadastrado com senha definida!");
            }
            else if (_usuarioEmEdicao != null)
            {
                // MODO EDIÇÃO
                _usuarioEmEdicao.Nome = Nome.Trim();
                _usuarioEmEdicao.CpfCnpj = string.IsNullOrWhiteSpace(CpfCnpj) ? null : CpfCnpj.Trim();
                _usuarioEmEdicao.DataNascimento = DataNascimento;
                _usuarioEmEdicao.Telefone = string.IsNullOrWhiteSpace(Telefone) ? null : Telefone.Trim();
                _usuarioEmEdicao.PerfilId = PerfilSelecionado.Id;
                _usuarioEmEdicao.Ativo = Ativo;
                _usuarioEmEdicao.AtualizadoEm = agora;

                await _supabaseClient.From<Usuario>().Update(_usuarioEmEdicao);
                await _db.SaveAsync(_usuarioEmEdicao);

                if (EhMotorista)
                {
                    if (_motoristaEmEdicao == null)
                    {
                        var novoMot = new Motorista
                        {
                            Id = Guid.NewGuid().ToString(),
                            UsuarioId = _usuarioEmEdicao.Id,
                            CnhNumero = CnhNumero.Trim(),
                            CnhCategoria = string.IsNullOrWhiteSpace(CnhCategoria) ? "D" : CnhCategoria,
                            CnhValidade = CnhValidade,
                            Ear = Ear,
                            StatusOperacional = "Disponível",
                            CriadoEm = agora,
                            AtualizadoEm = agora,
                            Sincronizado = true,
                            PrecisaAtualizar = false
                        };
                        var respMot = await _supabaseClient.From<Motorista>().Insert(novoMot);
                        await _db.SaveAsync(respMot.Models.FirstOrDefault() ?? novoMot);
                    }
                    else
                    {
                        _motoristaEmEdicao.CnhNumero = CnhNumero.Trim();
                        _motoristaEmEdicao.CnhCategoria = string.IsNullOrWhiteSpace(CnhCategoria) ? "D" : CnhCategoria;
                        _motoristaEmEdicao.CnhValidade = CnhValidade;
                        _motoristaEmEdicao.Ear = Ear;
                        _motoristaEmEdicao.AtualizadoEm = agora;

                        await _supabaseClient.From<Motorista>().Update(_motoristaEmEdicao);
                        await _db.SaveAsync(_motoristaEmEdicao);
                    }
                }

                await ExibirAlerta("Sucesso", $"Dados de {_usuarioEmEdicao.Nome} atualizados!");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro ao salvar", $"Não foi possível concluir a operação: {ex.Message}");
        }
        finally
        {
            EstaSalvando = false;
        }
    }

    private static async Task<(bool Sucesso, string Erro)> CriarContaAuthIsoladaAsync(string email, string senha)
    {
        using var http = new HttpClient();
        var url = $"{Secrets.SupabaseUrl.TrimEnd('/')}/auth/v1/signup";

        var payload = JsonSerializer.Serialize(new { email, password = senha });
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("apikey", Secrets.SupabaseKey);

        var response = await http.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return (true, string.Empty);

        var erro = await response.Content.ReadAsStringAsync();
        return (false, erro);
    }

    [RelayCommand]
    private async Task CancelarAsync() => await Shell.Current.GoToAsync("..");

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;
        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }
}