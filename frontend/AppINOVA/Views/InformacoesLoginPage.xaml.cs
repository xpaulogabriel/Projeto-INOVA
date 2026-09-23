namespace AppINOVA.Views;

public partial class InformacoesLoginPage : ContentPage
{
    public InformacoesLoginPage()
    {
        InitializeComponent();
    }

    private async void VoltarLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}