using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class CadastroUsuarioPage : ContentPage
{
    private readonly CadastroUsuarioViewModel _viewModel;

    public CadastroUsuarioPage(CadastroUsuarioViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarPerfisAsync();
    }
}