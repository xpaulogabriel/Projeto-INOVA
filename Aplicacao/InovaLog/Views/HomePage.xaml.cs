using InovaLog.Services;
using InovaLog.ViewModels;
using Microsoft.Maui.Networking;

namespace InovaLog.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // Sempre que a tela aparece, relê o perfil ativo e o status de conexão.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AtualizarSessao();
        OrganizarAtalhos();
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => _viewModel.AtualizarSessao());
    }

    // Mostra só os atalhos que o perfil pode usar e arruma em 2 colunas, sem buracos.
    private void OrganizarAtalhos()
    {
        AtalhoViagens.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Viagens);
        AtalhoFrota.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Frota);
        AtalhoFinanceiro.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Financeiro);
        AtalhoConfig.IsVisible = SessaoPerfil.TemAcesso(ModuloApp.Configuracoes);

        int posicao = 0;

        foreach (var atalho in new[] { AtalhoViagens, AtalhoFrota, AtalhoFinanceiro, AtalhoConfig })
        {
            if (!atalho.IsVisible)
                continue;

            Grid.SetRow(atalho, posicao / 2);
            Grid.SetColumn(atalho, posicao % 2);
            posicao++;
        }
    }

    // Quando existir uma foto real do usuário, chame este método.
    // Exemplo: DefinirFotoPerfil(ImageSource.FromFile("foto.png"));
    // Passando null, a tela volta a mostrar as iniciais.
    public void DefinirFotoPerfil(ImageSource? foto)
    {
        FotoPerfil.Source = foto;
        FotoPerfil.IsVisible = foto != null;
    }

    // Toque em um resultado da pesquisa rápida.
    private void OnResultadoTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is ResultadoBusca resultado)
            _viewModel.AbrirResultadoCommand.Execute(resultado);
    }

    // ==============================================================
    // MÉTODOS ORIGINAIS DA HOMEPAGE (mantidos)
    // ==============================================================

    private async void NovaRota_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovaRotaPage));
    }

    private async void NovaDespesa_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NovaDespesaPag));
    }

    private void Sair_Clicked(object sender, EventArgs e)
    {
        App.VoltarParaLogin();
    }
}