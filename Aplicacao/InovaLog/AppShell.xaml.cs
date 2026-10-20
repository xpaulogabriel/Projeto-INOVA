using InovaLog.Views;

namespace InovaLog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registo das rotas internas secundárias (Formulários que abrem por cima das abas)
        Routing.RegisterRoute(nameof(NovaDespesaPag), typeof(NovaDespesaPag));
        Routing.RegisterRoute(nameof(NovaRotaPage), typeof(NovaRotaPage));
    }
}