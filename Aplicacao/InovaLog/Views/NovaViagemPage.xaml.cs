
namespace InovaLog.Views;

public partial class NovaViagemPage : ContentPage
{
    private const double LarguraMenu = 280;
    private bool _animandoMenu;

    public NovaViagemPage()
    {
        InitializeComponent();
    }

    // ======================================
    // AÇÕES DO FORMULÁRIO
    // ======================================
    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnIniciarViagemClicked(object sender, EventArgs e)
    {
        if (PckVeiculo.SelectedIndex < 0 ||
            PckRotaPadrao.SelectedIndex < 0 ||
            PckMotorista.SelectedIndex < 0 ||
            PckTipoViagem.SelectedIndex < 0 ||
            string.IsNullOrWhiteSpace(TxtHodometroInicial.Text))
        {
            await DisplayAlert("Campos obrigatórios", "Preencha todos os campos.", "OK");
            return;
        }

        // Aqui entra o salvamento da viagem com status EM_ANDAMENTO.
        await Navigation.PopAsync();
    }

    // ======================================
    // MENU LATERAL
    // ======================================
    private async void OnMenuClicked(object sender, TappedEventArgs e)
    {
        await AbrirMenuAsync();
    }

    private async void OnFecharMenuTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
    }

    private async Task AbrirMenuAsync()
    {
        if (_animandoMenu || OverlayMenu.IsVisible) return;
        _animandoMenu = true;

        PainelMenu.TranslationX = -LarguraMenu;
        FundoMenu.Opacity = 0;
        OverlayMenu.IsVisible = true;

        await Task.WhenAll(
            PainelMenu.TranslateTo(0, 0, 250, Easing.CubicOut),
            FundoMenu.FadeTo(0.4, 250));

        _animandoMenu = false;
    }

    private async Task FecharMenuAsync()
    {
        if (_animandoMenu || !OverlayMenu.IsVisible) return;
        _animandoMenu = true;

        await Task.WhenAll(
            PainelMenu.TranslateTo(-LarguraMenu, 0, 220, Easing.CubicIn),
            FundoMenu.FadeTo(0, 220));

        OverlayMenu.IsVisible = false;
        _animandoMenu = false;
    }

    // Minhas Viagens: volta para a MotoristasPage
    private async void OnMenuMinhasViagensTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await Navigation.PopAsync();
    }

    // Gasto de Estrada: troca esta tela pela de gasto
    private async void OnMenuGastoEstradaTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await Navigation.PushAsync(new GastoEstradaPage());
    }

    // Nova Viagem: já está nesta tela, só fecha o menu
    private async void OnMenuNovaViagemTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        if (OverlayMenu.IsVisible)
        {
            _ = FecharMenuAsync();
            return true;
        }

        return base.OnBackButtonPressed();
    }
}
