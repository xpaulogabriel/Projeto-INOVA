using Microsoft.Maui.Controls;

namespace InovaLog.Views;

public partial class GastoEstradaPage : ContentPage
{
    private const double LarguraMenu = 280;
    private bool _animandoMenu;

    public GastoEstradaPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

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
            PainelMenu.TranslateToAsync(0, 0, 250, Easing.CubicOut),
            FundoMenu.FadeToAsync(0.4, 250));

        _animandoMenu = false;
    }

    private async Task FecharMenuAsync()
    {
        if (_animandoMenu || !OverlayMenu.IsVisible) return;
        _animandoMenu = true;

        await Task.WhenAll(
            PainelMenu.TranslateToAsync(-LarguraMenu, 0, 220, Easing.CubicIn),
            FundoMenu.FadeToAsync(0, 220));

        OverlayMenu.IsVisible = false;
        _animandoMenu = false;
    }

    // Minhas Viagens: volta para a MotoristasPage
    private async void OnMenuMinhasViagensTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await Navigation.PopAsync();
    }

    // Gasto de Estrada: já está nesta tela, só fecha o menu
    private async void OnMenuGastoEstradaTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
    }

    // Nova Viagem
    private async void OnMenuNovaViagemTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await Navigation.PushAsync(new NovaViagemPage());
    }

    // Botão voltar do Android fecha o menu primeiro
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