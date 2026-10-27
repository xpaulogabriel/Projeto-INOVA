
namespace InovaLog.Views;

public partial class NovoVeiculoPage : ContentPage
{
    public NovoVeiculoPage()
    {
        InitializeComponent();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Prévia do formulário",
            "O botão Cancelar está em modo de visualização. Nenhuma alteração foi salva.",
            "OK");
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Modo de visualização",
            "Esta tela ainda não está conectada ao cadastro de veículos. Nenhum dado foi salvo.",
            "OK");
    }
}
