namespace InovaLog.Views;

public partial class GastoEstradaPage : ContentPage
{
	public GastoEstradaPage()
	{
		InitializeComponent();
	}

    private async void OnMinhasViagensClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}