
namespace InovaLog.Views;

public partial class NovaRotaPage : ContentPage
{
    public NovaRotaPage()
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
            "A tela Nova Rota ainda não está conectada ao cadastro. Nenhum dado foi salvo.",
            "OK");
    }
}
