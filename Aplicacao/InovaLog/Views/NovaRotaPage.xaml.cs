
namespace InovaLog.Views;

public partial class NovaRotaPage : ContentPage
{
    public NovaRotaPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Prévia da tela",
            "A tela Nova Rota está em modo de visualização. Nenhum dado foi salvo.",
            "OK");
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Modo de visualização",
            "A tela Nova Rota ainda não está conectada ao cadastro. Nenhum dado foi salvo.",
            "OK");
    }
}
