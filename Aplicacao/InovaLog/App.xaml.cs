
using InovaLog.Services;
using InovaLog.Views;

namespace InovaLog;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthService _authService;

    public App(
        IServiceProvider serviceProvider,
        AuthService authService)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _authService = authService;
    }



    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Inicia no Login.
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        var window = new Window(new NavigationPage(loginPage));

        // Verifica se já existe um Token JWT válido salvo.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool sessaoValida = await _authService.ValidarSessaoExistenteAsync();

            if (sessaoValida)
            {
                EntrarNoSistema();
            }
        });

        return window;
    }



    /// <summary>
    /// Entra no Shell após a autenticação.
    /// Mantido para o fluxo original do aplicativo.
    /// </summary>
    public static void EntrarNoSistema()
    {
        if (Current?.Windows.Count > 0)
        {
            Current.Windows[0].Page = new AppShell();
        }
    }

    /// <summary>
    /// Encerra a sessão e retorna ao Login.
    /// Mantido para o fluxo original do aplicativo.
    /// </summary>
    public static void VoltarParaLogin()
    {
        if (Current is App app && app.Windows.Count > 0)
        {
            app._authService.Logout();

            var novaLoginPage =
                app._serviceProvider.GetRequiredService<LoginPage>();

            app.Windows[0].Page = new NavigationPage(novaLoginPage);
        }
    }
}
