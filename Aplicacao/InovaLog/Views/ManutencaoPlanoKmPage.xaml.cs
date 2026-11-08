
namespace InovaLog.Views;

public partial class ManutencaoPlanoKmPage : ContentPage
{
    public ManutencaoPlanoKmPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object? sender, EventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert(
                "Atenção",
                "Não existe uma página anterior para voltar.",
                "OK");
        }
    }



    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Modo de visualização",
            "A tela ainda não está conectada ao cadastro de manutenção. Nenhum dado foi salvo.",
            "OK");
    }
}
