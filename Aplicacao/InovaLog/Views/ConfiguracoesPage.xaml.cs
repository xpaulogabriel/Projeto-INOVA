using InovaLog.Services;
using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class ConfiguracoesPage : ContentPage
{
    private const int IndiceNenhuma = -1;
    private const int IndiceUsuarios = 0;
    private const int IndiceCategorias = 1;
    private const int IndicePagamentos = 2;
    private const int IndiceLogs = 3;

    private readonly ConfiguracoesViewModel _viewModel;
    private int _abaSelecionada = IndiceNenhuma;

    public ConfiguracoesPage(ConfiguracoesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        AplicarPermissoes();
        await _viewModel.CarregarDadosAsync();
    }

    private void AplicarPermissoes()
    {
        bool usuarios = SessaoPerfil.PodeGerenciarUsuarios();
        bool cadastros = SessaoPerfil.PodeGerenciarCadastrosFinanceiros();

        AbaUsuarios.IsVisible = usuarios;
        AbaCategorias.IsVisible = cadastros;
        AbaPagamentos.IsVisible = cadastros;
        AbaLogs.IsVisible = usuarios; // Apenas Admin
        BarraAbas.IsVisible = usuarios || cadastros;

        bool abaAtualPermitida =
            ((_abaSelecionada == IndiceUsuarios || _abaSelecionada == IndiceLogs) && usuarios) ||
            ((_abaSelecionada == IndiceCategorias || _abaSelecionada == IndicePagamentos) && cadastros);

        if (abaAtualPermitida)
            MostrarAba(_abaSelecionada);
        else if (usuarios)
            MostrarAba(IndiceUsuarios);
        else if (cadastros)
            MostrarAba(IndiceCategorias);
        else
            MostrarAba(IndiceNenhuma);
    }

    private void OnAbaUsuariosTapped(object? sender, TappedEventArgs e) => MostrarAba(IndiceUsuarios);
    private void OnAbaCategoriasTapped(object? sender, TappedEventArgs e) => MostrarAba(IndiceCategorias);
    private void OnAbaPagamentosTapped(object? sender, TappedEventArgs e) => MostrarAba(IndicePagamentos);
    private async void OnAbaLogsTapped(object? sender, TappedEventArgs e)
    {
        MostrarAba(IndiceLogs);
        await _viewModel.CarregarDadosAsync();
    }

    private void MostrarAba(int aba)
    {
        _abaSelecionada = aba;

        ConteudoUsuarios.IsVisible = aba == IndiceUsuarios;
        ConteudoCategorias.IsVisible = aba == IndiceCategorias;
        ConteudoPagamentos.IsVisible = aba == IndicePagamentos;
        ConteudoLogs.IsVisible = aba == IndiceLogs;
        ConteudoMinhaConta.IsVisible = aba == IndiceNenhuma;

        ConfigurarAba(AbaUsuarios, aba == IndiceUsuarios);
        ConfigurarAba(AbaCategorias, aba == IndiceCategorias);
        ConfigurarAba(AbaPagamentos, aba == IndicePagamentos);
        ConfigurarAba(AbaLogs, aba == IndiceLogs);
    }

    private static void ConfigurarAba(Border aba, bool selecionada)
    {
        aba.BackgroundColor = selecionada ? Color.FromArgb("#1EA1E8") : Colors.White;
        aba.Stroke = selecionada ? Colors.Transparent : Color.FromArgb("#E2E8F0");
        aba.StrokeThickness = selecionada ? 0 : 1;

        if (aba.Content is Label texto)
        {
            texto.TextColor = selecionada ? Colors.White : Color.FromArgb("#64748B");
            texto.FontFamily = selecionada ? "PoppinsSemiBold" : "PoppinsMedium";
        }
    }

    private void OnEditarUsuarioClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is UsuarioItem usuario)
            _viewModel.EditarUsuarioCommand.Execute(usuario);
    }

    private void OnInspecionarUsuarioClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is UsuarioItem usuario)
            _viewModel.InspecionarUsuarioCommand.Execute(usuario);
    }

    private void OnAlternarStatusUsuarioClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is UsuarioItem usuario)
            _viewModel.AlternarStatusUsuarioCommand.Execute(usuario);
    }

    private void OnExcluirUsuarioClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is UsuarioItem usuario)
            _viewModel.ExcluirUsuarioCommand.Execute(usuario);
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
    private void OnEditarCategoriaClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is CategoriaItem categoria)
            _viewModel.EditarCategoriaCommand.Execute(categoria);
    }

    private void OnExcluirCategoriaClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is CategoriaItem categoria)
            _viewModel.ExcluirCategoriaCommand.Execute(categoria);
    }

    private void OnEditarPagamentoClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is FormaPagamentoItem forma)
            _viewModel.EditarFormaPagamentoCommand.Execute(forma);
    }

    private void OnExcluirPagamentoClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is FormaPagamentoItem forma)
            _viewModel.ExcluirFormaPagamentoCommand.Execute(forma);
    }
}