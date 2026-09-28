using InovaLog.Views;

namespace InovaLog.Views;

// Resolvendo o erro de modificadores conflitantes e parciais
public partial class TesteBancoPage : ContentPage
{
    public TesteBancoPage()
    {
        InitializeComponent();
    }

    // Função para o botão de voltar funcionar
    private async void Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}