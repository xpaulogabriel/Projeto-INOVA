using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace InovaLog.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string senha;

    [RelayCommand]
    private async Task EntrarAsync()
    {
        Console.WriteLine($"Tentando logar com: {Email}");
    }

    [RelayCommand]
    private async Task IrParaEsqueciSenhaAsync()
    {
        // Navegação moderna sem usar o MainPage
        var paginaAtual = Application.Current.Windows[0].Page;
        await paginaAtual.Navigation.PushAsync(new Views.EsqueciSenhaPage());
    }

    [RelayCommand]
    private async Task IrParaInformacoesAsync()
    {
        var paginaAtual = Application.Current.Windows[0].Page;
        await paginaAtual.Navigation.PushAsync(new Views.InformacoesLoginPage());
    }
}