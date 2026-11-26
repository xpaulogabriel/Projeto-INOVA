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
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            var window = new Window(new NavigationPage(loginPage));

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    bool sessaoValida = await _authService.ValidarSessaoExistenteAsync();
                    if (sessaoValida)
                    {
                        EntrarNoSistema();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERRO VALIDAR SESSAO]: {ex}");
                }
            });

            return window;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERRO CREATE WINDOW]: {ex}");
            throw;
        }
    }

    public static void EntrarNoSistema()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Current is App app && app.Windows.Count > 0)
            {
                app.Windows[0].Page = new AppShell();

                // Ativa o monitoramento de rede e dispara a 1ª sincronização em segundo plano
                app._syncService.IniciarMonitoramentoConexao();
                _ = app._syncService.SincronizarTudoAsync();
            }
        });
    }

    public static void VoltarParaLogin()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Current is App app && app.Windows.Count > 0)
            {
                app._authService.Logout();
                var novaLoginPage = app._serviceProvider.GetRequiredService<LoginPage>();
                app.Windows[0].Page = new NavigationPage(novaLoginPage);
            }
        });
    }
}