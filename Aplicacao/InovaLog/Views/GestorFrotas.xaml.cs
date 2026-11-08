
namespace InovaLog.Views;

public partial class GestorFrotasPage : ContentPage
{
    public GestorFrotasPage()
    {
        InitializeComponent();
    }

    // =========================
    // ABAS
    // =========================

    private void OnVeiculosTapped(object? sender, TappedEventArgs e)
    {
        MostrarAba(0);
    }

    private void OnRotasTapped(object? sender, TappedEventArgs e)
    {
        MostrarAba(1);
    }

    private void OnPlanosTapped(object? sender, TappedEventArgs e)
    {
        MostrarAba(2);
    }

    private void MostrarAba(int aba)
    {
        ConteudoVeiculos.IsVisible = aba == 0;
        ConteudoRotas.IsVisible = aba == 1;
        ConteudoPlanos.IsVisible = aba == 2;

        ConfigurarAba(AbaVeiculos, aba == 0);
        ConfigurarAba(AbaRotas, aba == 1);
        ConfigurarAba(AbaPlanos, aba == 2);
    }

    private static void ConfigurarAba(Border aba, bool selecionada)
    {
        aba.BackgroundColor = selecionada
            ? Color.FromArgb("#1675F8")
            : Colors.White;

        aba.Stroke = selecionada
            ? Colors.Transparent
            : Color.FromArgb("#E2E8F0");

        if (aba.Content is Label texto)
        {
            texto.TextColor = selecionada
                ? Colors.White
                : Color.FromArgb("#64748B");

            texto.FontFamily = selecionada
                ? "PoppinsSemiBold"
                : "PoppinsMedium";
        }
    }

    // =========================
    // NOVO VEÍCULO
    // =========================

    private async void OnNovoVeiculoClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new NovoVeiculoPage());
    }

    // =========================
    // NOVA ROTA
    // =========================

    private async void OnNovaRotaClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new NovaRotaPage());
    }

    // =========================
    // REGISTRAR SERVIÇO
    // =========================

    private async void OnRegistrarServicoClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ManutencaoPlanoKmPage());
    }
}
