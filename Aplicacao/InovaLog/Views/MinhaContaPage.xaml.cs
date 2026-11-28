using InovaLog.ViewModels;
using Microsoft.Maui.Networking;

namespace InovaLog.Views;

public partial class MinhaContaPage : ContentPage
{
    private readonly MinhaContaViewModel _viewModel;

    public MinhaContaPage(MinhaContaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CarregarDados();
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => _viewModel.AtualizarConexao());
    }
}