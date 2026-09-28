using InovaLog.Views;

namespace InovaLog;

public partial class App : Application
{
    private readonly LoginPage _loginPage;

    // O "maestro" do MAUI vai injetar a LoginPage pronta aqui
    public App(LoginPage loginPage)
    {
        InitializeComponent();
        _loginPage = loginPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(_loginPage));
    }
}