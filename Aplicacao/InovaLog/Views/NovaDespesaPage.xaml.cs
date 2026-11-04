namespace InovaLog.Views;

public partial class NovaDespesaPag : ContentPage
{
    public NovaDespesaPag()
    {
        InitializeComponent();
    }

    //BOTÃO CANCELAR QUE VOLTA PARA RESUMO & EXTRATO
    private async void OnCancelarClicked(object? sender, EventArgs e) {await Navigation.PopAsync();}
}