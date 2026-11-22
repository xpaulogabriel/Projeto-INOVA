using InovaLog.Services;
using InovaLog.Views;

namespace InovaLog;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthService _authService;
    private readonly SyncService _syncService;

    public App(
        IServiceProvider serviceProvider,
        AuthService authService,
        SyncService syncService)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _authService = authService;
        _syncService = syncService;

        _syncService.IniciarMonitoramentoConexao();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Inicia sempre trancado na LoginPage (inclusive em DEBUG para podermos testar)
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        var window = new Window(new NavigationPage(loginPage));

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

    public static void EntrarNoSistema()
    {
        if (Current?.Windows.Count > 0)
        {
            Current.Windows[0].Page = new AppShell();
        }
    }

    public static void VoltarParaLogin()
    {
        if (Current is App app && app.Windows.Count > 0)
        {
            app._authService.Logout();
            var novaLoginPage = app._serviceProvider.GetRequiredService<LoginPage>();
            app.Windows[0].Page = new NavigationPage(novaLoginPage);
        }
    }
}