using InovaLog.Services;
using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class ConfiguracoesPage : ContentPage
{
    private const int IndiceNenhuma = -1;
    private const int IndiceUsuarios = 0;
    private const int IndiceCategorias = 1;
    private const int IndicePagamentos = 2;

    private readonly ConfiguracoesViewModel _viewModel = new();
    private int _abaSelecionada = IndiceNenhuma;

    public ConfiguracoesPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.AtualizarSessao();
        AplicarPermissoes();
    }

    // ==============================================================
    // PERMISSÕES: cada perfil vê somente as abas que pode usar
    // ==============================================================
    private void AplicarPermissoes()
    {
        bool usuarios = SessaoPerfil.PodeGerenciarUsuarios();
        bool cadastros = SessaoPerfil.PodeGerenciarCadastrosFinanceiros();

        AbaUsuarios.IsVisible = usuarios;
        AbaCategorias.IsVisible = cadastros;
        AbaPagamentos.IsVisible = cadastros;
        BarraAbas.IsVisible = usuarios || cadastros;

        // Mantém a aba atual se ela ainda for permitida; senão vai para a primeira permitida.
        bool abaAtualPermitida =
            (_abaSelecionada == IndiceUsuarios && usuarios) ||
            ((_abaSelecionada == IndiceCategorias || _abaSelecionada == IndicePagamentos) && cadastros);

        if (abaAtualPermitida)
            MostrarAba(_abaSelecionada);
        else if (usuarios)
            MostrarAba(IndiceUsuarios);
        else if (cadastros)
            MostrarAba(IndiceCategorias);
        else
            MostrarAba(IndiceNenhuma);   // mostra "Minha Conta"
    }

    // ==============================================================
    // ABAS
    // ==============================================================
    private void OnAbaUsuariosTapped(object? sender, TappedEventArgs e) => MostrarAba(IndiceUsuarios);

    private void OnAbaCategoriasTapped(object? sender, TappedEventArgs e) => MostrarAba(IndiceCategorias);

    private void OnAbaPagamentosTapped(object? sender, TappedEventArgs e) => MostrarAba(IndicePagamentos);

    private void MostrarAba(int aba)
    {
        _abaSelecionada = aba;

        ConteudoUsuarios.IsVisible = aba == IndiceUsuarios;
        ConteudoCategorias.IsVisible = aba == IndiceCategorias;
        ConteudoPagamentos.IsVisible = aba == IndicePagamentos;
        ConteudoMinhaConta.IsVisible = aba == IndiceNenhuma;

        ConfigurarAba(AbaUsuarios, aba == IndiceUsuarios);
        ConfigurarAba(AbaCategorias, aba == IndiceCategorias);
        ConfigurarAba(AbaPagamentos, aba == IndicePagamentos);
    }

    // Mesmo padrão visual das abas do Gestor de Frotas / Financeiro.
    private static void ConfigurarAba(Border aba, bool selecionada)
    {
        aba.BackgroundColor = selecionada
            ? Color.FromArgb("#1EA1E8")
            : Colors.White;

        aba.Stroke = selecionada
            ? Colors.Transparent
            : Color.FromArgb("#E2E8F0");

        aba.StrokeThickness = selecionada ? 0 : 1;

        if (aba.Content is Label texto)
        {
            texto.TextColor = selecionada
                ? Colors.White
                : Color.FromArgb("#64748B");

            texto.FontFamily = selecionada
                ? "PoppinsSemiBold"
                : "PoppinsMedium";
        }
    }

    // ==============================================================
    // CLIQUES DAS LISTAS (o item vem do BindingContext do controle)
    // ==============================================================
    private void OnUsarContaClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is UsuarioItem usuario)
            _viewModel.UsarContaCommand.Execute(usuario);
    }

    private void OnAlternarCategoriaClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is CategoriaItem categoria)
            _viewModel.AlternarCategoriaCommand.Execute(categoria);
    }

    private void OnAlternarPagamentoTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is FormaPagamentoItem forma)
            _viewModel.AlternarFormaPagamentoCommand.Execute(forma);
    }
}
