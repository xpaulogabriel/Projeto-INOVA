
namespace InovaLog.Views;

public partial class ManutencaoPlanoKmPage : ContentPage
{
    public ManutencaoPlanoKmPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Prévia da tela",
            "A tela Manutenção & Plano por KM está em modo de visualização. Nenhum dado foi salvo.",
            "OK");
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Modo de visualização",
            "A tela ainda não está conectada ao cadastro de manutenção. Nenhum dado foi salvo.",
            "OK");
    }
}
