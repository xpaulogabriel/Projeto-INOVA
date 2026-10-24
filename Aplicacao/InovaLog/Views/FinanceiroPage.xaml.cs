
using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class FinanceiroPage : ContentPage
{
    public FinanceiroPage()
    {
        InitializeComponent();

        // Mantém o ViewModel utilizado pelo gráfico.
        BindingContext = new FinanceiroViewModel();

        // Abre a página inicialmente no Resumo.
        SelecionarAba("resumo");
    }

    // Clique na aba Resumo & Extrato.
    private void OnResumoTapped(object? sender, TappedEventArgs e)
    {
        SelecionarAba("resumo");
    }

    // Clique na aba Financiamentos.
    private void OnFinanciamentosTapped(
        object? sender,
        TappedEventArgs e)
    {
        SelecionarAba("financiamentos");
    }

    // Clique na aba Projeções.
    private void OnProjecoesTapped(
        object? sender,
        TappedEventArgs e)
    {
        SelecionarAba("projecoes");
    }

    // Exibe somente o conteúdo da aba selecionada.
    private void SelecionarAba(string abaSelecionada)
    {
        bool resumo = abaSelecionada == "resumo";
        bool financiamentos = abaSelecionada == "financiamentos";
        bool projecoes = abaSelecionada == "projecoes";

        ConteudoResumo.IsVisible = resumo;
        ConteudoFinanciamentos.IsVisible = financiamentos;
        ConteudoProjecoes.IsVisible = projecoes;

        // Mantém o rodapé original, ocultando somente
        // o botão de ação quando Projeções estiver aberta.
        BtnAcaoFinanceira.IsVisible = !projecoes;

        if (resumo)
        {
            BtnAcaoFinanceira.Text = "+ NOVA DESPESA";
        }
        else if (financiamentos)
        {
            BtnAcaoFinanceira.Text = "+ NOVA DÍVIDA";
        }

        AtualizarEstiloAba(AbaResumo, resumo);
        AtualizarEstiloAba(AbaFinanciamentos, financiamentos);
        AtualizarEstiloAba(AbaProjecoes, projecoes);
    }

    // Atualiza a aparência da aba selecionada.
    private static void AtualizarEstiloAba(
        Border aba,
        bool selecionada)
    {
        if (aba.Content is not Label texto)
            return;

        if (selecionada)
        {
            aba.BackgroundColor = Color.FromArgb("#1675F8");
            aba.Stroke = Colors.Transparent;
            aba.StrokeThickness = 0;

            texto.TextColor = Colors.White;
            texto.FontFamily = "PoppinsSemiBold";
        }
        else
        {
            aba.BackgroundColor = Colors.White;
            aba.Stroke = new SolidColorBrush(
                Color.FromArgb("#E2E8F0"));

            aba.StrokeThickness = 1;

            texto.TextColor = Color.FromArgb("#64748B");
            texto.FontFamily = "PoppinsMedium";
        }
    }
}
