
namespace InovaLog.Views;

public partial class NovoVeiculoPage : ContentPage
{
    public NovoVeiculoPage()
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
            "Esta tela ainda não está conectada ao cadastro de veículos. Nenhum dado foi salvo.",
            "OK");
    }
}
