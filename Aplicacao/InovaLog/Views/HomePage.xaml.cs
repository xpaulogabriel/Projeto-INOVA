using InovaLog.Services;
using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel = new();

    public HomePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    // Sempre que a tela aparece (ex.: depois de Trocar Conta), relê o perfil ativo.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AtualizarSessao();
        OrganizarAtalhos();
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
