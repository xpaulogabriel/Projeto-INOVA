using InovaLog.Services;
using InovaLog.Views;

namespace InovaLog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registo das rotas internas secundárias (formulários que abrem por cima das abas)
        Routing.RegisterRoute(nameof(NovaDespesaPag), typeof(NovaDespesaPag));
        Routing.RegisterRoute(nameof(NovaRotaPage), typeof(NovaRotaPage));
        Routing.RegisterRoute(nameof(TrocarContaPage), typeof(TrocarContaPage));
        Routing.RegisterRoute(nameof(CadastroUsuarioPage), typeof(CadastroUsuarioPage));

        // Esconde/mostra as abas conforme o perfil ativo.
        SessaoPerfil.PerfilAlterado += AplicarPermissoesNasAbas;
        AplicarPermissoesNasAbas();
    }

    // Soltamos o evento quando este Shell é descartado (ex.: ao terminar a sessão).
    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.NewHandler == null)
            SessaoPerfil.PerfilAlterado -= AplicarPermissoesNasAbas;
    }

    private void AplicarPermissoesNasAbas()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TabInicio.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Inicio);
            TabViagens.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Viagens);
            TabFrota.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Frota);
            TabFinanceiro.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Financeiro);
            TabConfig.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Configuracoes);
        });
    }
}
