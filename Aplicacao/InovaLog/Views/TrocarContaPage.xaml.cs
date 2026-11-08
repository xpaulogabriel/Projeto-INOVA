using InovaLog.Services;

namespace InovaLog.Views;

public partial class TrocarContaPage : ContentPage
{
    public TrocarContaPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarSelecao();
    }

    // Destaca o perfil que está em uso (borda azul + selo ATUAL).
    private void AtualizarSelecao()
    {
        var atual = SessaoPerfil.PerfilAtual;

        MarcarCard(CardMotorista, BadgeMotorista, atual == PerfilUsuario.Motorista);
        MarcarCard(CardGestorRotas, BadgeGestorRotas, atual == PerfilUsuario.GestorRotas);
        MarcarCard(CardFinanceiro, BadgeFinanceiro, atual == PerfilUsuario.Financeiro);
        MarcarCard(CardAdmin, BadgeAdmin, atual == PerfilUsuario.Admin);
    }

    private static void MarcarCard(Border card, Border badge, bool atual)
    {
        card.Stroke = atual
            ? Color.FromArgb("#2C63B1")
            : Color.FromArgb("#E2E8F0");

        card.StrokeThickness = atual ? 2 : 1;
        badge.IsVisible = atual;
    }

    // Toque em um dos perfis: troca e volta para o Início.
    private async void OnPerfilTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not string nome || !Enum.TryParse(nome, out PerfilUsuario perfil))
            return;

        SessaoPerfil.DefinirPorPerfil(perfil);

        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void OnFecharTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // TERMINAR SESSÃO: encerra e volta para a tela de Login.
    private async void OnTerminarSessaoClicked(object? sender, EventArgs e)
    {
        bool confirmar = await DisplayAlertAsync(
            "Terminar sessão",
            "Deseja realmente sair da sua conta?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        SessaoPerfil.Reiniciar();
        App.VoltarParaLogin();
    }
}
