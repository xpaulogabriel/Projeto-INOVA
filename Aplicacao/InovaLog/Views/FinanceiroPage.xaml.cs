using InovaLog.ViewModels;
using LiveChartsCore.Measure;

namespace InovaLog.Views;

public partial class FinanceiroPage : ContentPage
{
    public FinanceiroPage()
    {
        InitializeComponent();
        BindingContext = new FinanceiroViewModel();

        // A legenda é a do XAML (com as bolinhas), então esconde a do gráfico
        GraficoFinanceiro.LegendPosition = LegendPosition.Hidden;
    }
}