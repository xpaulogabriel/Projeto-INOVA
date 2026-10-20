namespace InovaLog.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    // Chame este evento no botão "+ Nova Viagem / Rota" do HomePage.xaml
    private async void NovaRota_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovaRotaPage));
    }

    // Chame este evento no botão "Lançar Despesa" do HomePage.xaml
    private async void NovaDespesa_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovaDespesaPag));
    }

    // Caso queira adicionar um botão/toque para testar o Logout e voltar ao Login
    private void Sair_Clicked(object sender, EventArgs e)
    {
        App.VoltarParaLogin();
    }
}