using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace InovaLog.Views;

public partial class MotoristasPage : ContentPage
{
    private int _abaSelecionada;
    private bool _viagemConcluida;

    private const double LarguraMenu = 280;
    private bool _animandoMenu;

    private readonly string[] _titulosAbas =
    {
        "Viagens",
        "Em Andamento",
        "Concluídas",
        "Retornos Vazios"
    };

    public MotoristasPage()
    {
        InitializeComponent();
        AtualizarAba(0);
    }

    // ======================================
    // ABAS SUPERIORES
    // ======================================
    private void OnAbaAnteriorClicked(object sender, EventArgs e)
    {
        if (_abaSelecionada > 0)
            AtualizarAba(_abaSelecionada - 1);
    }

    private void OnProximaAbaClicked(object sender, EventArgs e)
    {
        if (_abaSelecionada < _titulosAbas.Length - 1)
            AtualizarAba(_abaSelecionada + 1);
    }

    private void AtualizarAba(int indice)
    {
        if (indice < 0 || indice >= _titulosAbas.Length)
            return;

        _abaSelecionada = indice;

        TextoAbaSelecionada.Text = _titulosAbas[indice];
        TituloSecao.Text = indice == 0 ? "Todas as Viagens" : _titulosAbas[indice];

        BtnAbaAnterior.IsEnabled = indice > 0;
        BtnAbaAnterior.Opacity = indice > 0 ? 1 : 0.3;

        BtnProximaAba.IsEnabled = indice < _titulosAbas.Length - 1;
        BtnProximaAba.Opacity = indice < _titulosAbas.Length - 1 ? 1 : 0.3;

        CardRetorno.IsVisible = indice == 0 || indice == 3;
        CardViagem.IsVisible = indice == 0 || indice == 1;
        CardViagemConcluida.IsVisible = indice == 0 || indice == 2;

        AtualizarStatusViagem();
    }

    private void AtualizarStatusViagem()
    {
        if (_viagemConcluida)
        {
            TextoStatus.Text = "Concluída";
            StatusViagem.BackgroundColor = Color.FromArgb("#DCFCE7");
            TextoStatus.TextColor = Color.FromArgb("#166534");

            BotaoGasto.IsVisible = true;
            BotaoFinalizar.IsVisible = false;
        }
        else
        {
            TextoStatus.Text = "Em andamento";
            StatusViagem.BackgroundColor = Color.FromArgb("#FEF3C7");
            TextoStatus.TextColor = Color.FromArgb("#92400E");

            BotaoGasto.IsVisible = true;
            BotaoFinalizar.IsVisible = true;
        }
    }

    // ======================================
    // AÇÕES DA TELA (botões dos cards)
    // ======================================
    private async void OnNovaViagemClicked(object sender, EventArgs e)
    {
        await IrParaNovaViagemAsync();
    }

    private async void OnLancarGastoClicked(object sender, EventArgs e)
    {
        await IrParaGastoEstradaAsync();
    }

    private async void OnFinalizarViagemClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Finalizar viagem",
            "Deseja realmente finalizar esta viagem?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        _viagemConcluida = true;
        AtualizarStatusViagem();
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

    private async void OnMenuMinhasViagensTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        AtualizarAba(0);
    }

    private async void OnMenuGastoEstradaTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await IrParaGastoEstradaAsync();
    }

    private async void OnMenuNovaViagemTapped(object sender, TappedEventArgs e)
    {
        await FecharMenuAsync();
        await IrParaNovaViagemAsync();
    }

    // Fecha o menu com o botão voltar do Android
    protected override bool OnBackButtonPressed()
    {
        if (OverlayMenu.IsVisible)
        {
            _ = FecharMenuAsync();
            return true;
        }

        return base.OnBackButtonPressed();
    }

    // ======================================
    // NAVEGAÇÃO
    // ======================================
    private async Task IrParaGastoEstradaAsync()
    {
        // Ajuste para o nome real da sua página, se for diferente.
        await Navigation.PushAsync(new GastoEstradaPage());
    }

    private async Task IrParaNovaViagemAsync()
    {
        await Navigation.PushAsync(new NovaViagemPage());
    }


}