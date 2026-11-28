using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Services;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Networking;

namespace InovaLog.ViewModels;

public partial class MinhaContaViewModel : ObservableObject
{
    private readonly Supabase.Client _supabaseClient;

    [ObservableProperty] private string iniciais = string.Empty;
    [ObservableProperty] private string nomeUsuario = string.Empty;
    [ObservableProperty] private string emailUsuario = string.Empty;
    [ObservableProperty] private string rotuloPerfil = string.Empty;
    [ObservableProperty] private string textoStatus = "ONLINE";
    [ObservableProperty] private Color corStatus = Color.FromArgb("#10B981");
    [ObservableProperty] private Color corFundoStatus = Color.FromArgb("#D1FAE5");
    [ObservableProperty] private bool saindo;

    public MinhaContaViewModel(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public void CarregarDados()
    {
        NomeUsuario = SessaoPerfil.NomeUsuario;
        EmailUsuario = SessaoPerfil.EmailUsuario;
        RotuloPerfil = SessaoPerfil.RotuloPerfil(SessaoPerfil.PerfilAtual);
        Iniciais = SessaoPerfil.Iniciais(NomeUsuario);

        AtualizarConexao();
    }

    public void AtualizarConexao()
    {
        var online = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        TextoStatus = online ? "ONLINE" : "OFFLINE";
        CorStatus = online ? Color.FromArgb("#10B981") : Color.FromArgb("#F59E0B");
        CorFundoStatus = online ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEF3C7");
    }

    [RelayCommand]
    private async Task SairAsync()
    {
        if (Saindo) return;

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina != null)
        {
            bool confirmar = await pagina.DisplayAlertAsync(
                "Sair da Conta",
                "Deseja realmente encerrar sua sessão no InovaLog?",
                "Sair",
                "Cancelar");

            if (!confirmar) return;
        }

        Saindo = true;
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _supabaseClient.Auth.SignOut();
            }
        }
        catch
        {
            // Mesmo se falhar na rede (offline), encerra a sessão local normalmente
        }
        finally
        {
            Saindo = false;
            App.VoltarParaLogin();
        }
    }

    [RelayCommand]
    private async Task VoltarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}