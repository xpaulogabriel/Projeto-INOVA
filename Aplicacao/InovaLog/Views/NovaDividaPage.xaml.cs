
namespace InovaLog.Views;

public partial class NovaDividaPage : ContentPage
{
    public NovaDividaPage()
    {
        InitializeComponent();
    }

    // Botão Cancelar: retorna para Financeiro.
    private async void OnCancelarClicked(
        object? sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
