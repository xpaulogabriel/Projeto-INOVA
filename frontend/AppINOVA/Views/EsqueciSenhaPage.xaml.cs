namespace AppINOVA.Views;

public partial class EsqueciSenhaPage : ContentPage
{
    public EsqueciSenhaPage()
    {
        InitializeComponent();
    }

    private async void VoltarLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}