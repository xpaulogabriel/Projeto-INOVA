using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Models;
using InovaLog.Services;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using static Postgrest.Constants;

namespace InovaLog.ViewModels;

public partial class CadastroUsuarioViewModel : ObservableObject
{
    private readonly Supabase.Client _supabaseClient;
    private readonly DatabaseService _db;

    public ObservableCollection<Perfil> Perfis { get; } = new();

    [ObservableProperty] private string nome = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private Perfil? perfilSelecionado;
    [ObservableProperty] private bool ehMotorista;

    // Campos específicos de Motorista
    [ObservableProperty] private string cpf = string.Empty;
    [ObservableProperty] private DateTime dataNascimento = new(1995, 1, 1);
    [ObservableProperty] private string cnhNumero = string.Empty;
    [ObservableProperty] private DateTime cnhValidade = DateTime.Today.AddYears(2);
    [ObservableProperty] private string cnhCategoria = "D";

    // Status e controle
    [ObservableProperty] private bool ativo = true;
    [ObservableProperty] private bool estaSalvando;

    public string TextoStatus => Ativo ? "Status de Acesso: Ativo" : "Status de Acesso: Bloqueado";

    public CadastroUsuarioViewModel(Supabase.Client supabaseClient, DatabaseService db)
    {
        _supabaseClient = supabaseClient;
        _db = db;
    }

    partial void OnAtivoChanged(bool value) => OnPropertyChanged(nameof(TextoStatus));

    partial void OnPerfilSelecionadoChanged(Perfil? value)
    {
        EhMotorista = value != null &&
                      value.Nome != null &&
                      value.Nome.Contains("motorista", StringComparison.OrdinalIgnoreCase);
    }

    public async Task CarregarPerfisAsync()
    {
        if (Perfis.Count > 0) return;

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

        PerfilSelecionado = Perfis.FirstOrDefault(p =>
            p.Nome != null && p.Nome.Contains("motorista", StringComparison.OrdinalIgnoreCase))
            ?? Perfis.FirstOrDefault();
    }

    [RelayCommand]
    private async Task CadastrarAsync()
    {
        if (EstaSalvando) return;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para cadastrar novos usuários.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || PerfilSelecionado == null)
        {
            await ExibirAlerta("Campos obrigatórios", "Preencha Nome, Perfil de Acesso e E-mail.");
            return;
        }

        var emailLimpo = Email.Trim().ToLowerInvariant();

        if (EhMotorista && (string.IsNullOrWhiteSpace(Cpf) || string.IsNullOrWhiteSpace(CnhNumero)))
        {
            await ExibirAlerta("Dados do Motorista", "Informe o CPF e o Nº da CNH para o perfil Motorista.");
            return;
        }

        EstaSalvando = true;
        try
        {
            // 1. Verifica se o e-mail já está cadastrado
            var existente = await _supabaseClient
                .From<Usuario>()
                .Filter("email", Operator.ILike, emailLimpo)
                .Get();

            if (existente.Models.Any())
            {
                await ExibirAlerta("E-mail já cadastrado", "Já existe um usuário com este e-mail no InovaLog.");
                return;
            }

            // 2. Monta a Model (ajuste os nomes das propriedades caso algum difira no seu Usuario.cs)
            var novoUsuario = new Usuario
            {
                Nome = Nome.Trim(),
                Email = emailLimpo,
                PerfilId = PerfilSelecionado.Id,
                Ativo = Ativo,
                Cpf = EhMotorista ? Cpf.Trim() : null,
                DataNascimento = EhMotorista ? DataNascimento : null,
                CnhNumero = EhMotorista ? CnhNumero.Trim() : null,
                CnhValidade = EhMotorista ? CnhValidade : null,
                CnhCategoria = EhMotorista ? CnhCategoria : null,
                Sincronizado = true,
                PrecisaAtualizar = false
            };

            // 3. Salva no Supabase e espelha no SQLite
            var resposta = await _supabaseClient.From<Usuario>().Insert(novoUsuario);
            var salvo = resposta.Models.FirstOrDefault() ?? novoUsuario;
            await _db.SaveAsync(salvo);

            await ExibirAlerta("Sucesso", $"Usuário {salvo.Nome} cadastrado! Ele já pode acessar com o Google.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro ao cadastrar", $"Não foi possível salvar o usuário: {ex.Message}");
        }
        finally
        {
            EstaSalvando = false;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;
        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }
}