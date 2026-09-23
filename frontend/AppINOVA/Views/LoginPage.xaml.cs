namespace AppINOVA.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void EsqueciSenha_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EsqueciSenhaPage());
    }

    private async void InformacoesLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InformacoesLoginPage());
    }
}