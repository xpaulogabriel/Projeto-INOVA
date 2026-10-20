using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Services;
using InovaLog.Views;

namespace InovaLog.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string senha = string.Empty;

    [ObservableProperty]
    private bool estaCarregando = false;

    public LoginViewModel(AuthService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task EntrarAsync()
    {
        if (EstaCarregando) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
        {
            await ExibirAlerta("Atenção", "Preencha o utilizador/e-mail e a palavra-passe.");
            return;
        }

        // Atalho de validação de arquitetura (Fase A) caso o Supabase ainda não esteja configurado
        if (Email.Trim().ToLower() == "admin@inova.com" && Senha == "123456")
        {
            App.EntrarNoSistema();
            return;
        }

        EstaCarregando = true;
        var resultado = await _authService.LoginComEmailAsync(Email.Trim(), Senha);
        EstaCarregando = false;

        if (resultado.Sucesso)
        {
            App.EntrarNoSistema();
        }
        else
        {
            await ExibirAlerta("Falha no Login", resultado.Mensagem);
        }
    }

    [RelayCommand]
    private async Task EntrarComGoogleAsync()
    {
        if (EstaCarregando) return;

        EstaCarregando = true;
        var resultado = await _authService.LoginComGoogleAsync();
        EstaCarregando = false;

        if (resultado.Sucesso)
        {
            App.EntrarNoSistema();
        }
        else
        {
            await ExibirAlerta("Autenticação Google", resultado.Mensagem);
        }
    }

    // Navegação contida dentro do "Cercadinho" Pré-Login
    [RelayCommand]
    private async Task IrParaEsqueciSenhaAsync()
    {
        var nav = Application.Current?.Windows[0].Page?.Navigation;
        if (nav != null)
        {
            var page = _serviceProvider.GetRequiredService<EsqueciSenhaPage>();
            await nav.PushAsync(page);
        }
    }

    [RelayCommand]
    private async Task IrParaInformacoesAsync()
    {
        var nav = Application.Current?.Windows[0].Page?.Navigation;
        if (nav != null)
        {
            var page = _serviceProvider.GetRequiredService<InformacoesLoginPage>();
            await nav.PushAsync(page);
        }
    }

    private async Task ExibirAlerta(string titulo, string mensagem)
    {
        var paginaAtual = Application.Current?.Windows[0].Page;
        if (paginaAtual != null)
            await paginaAtual.DisplayAlert(titulo, mensagem, "OK");
    }
}