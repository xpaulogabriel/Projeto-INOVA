using InovaLog.ViewModels;

namespace InovaLog.Views;

public partial class FinanceiroPage : ContentPage
{
    private int _abaSelecionada;

    // Títulos das abas do Financeiro.
    private readonly string[] _titulosAbas ={
        "Resumo & Extrato","Financiamentos","Projeções"
    };

    public FinanceiroPage()
    {
        InitializeComponent();
        // Mantém o ViewModel utilizado pelo gráfico.
        BindingContext = new FinanceiroViewModel();
        // Abre inicialmente no Resumo.
        AtualizarAba(0);
    }

    // VOLTAR UMA ABA.
    private void OnAbaAnteriorClicked (object? sender,EventArgs e) {
        if (_abaSelecionada > 0) { AtualizarAba(_abaSelecionada - 1);
        }
    }

    // AVANÇAR UMA ABA.
    private void OnProximaAbaClicked (object? sender,EventArgs e) {
        if (_abaSelecionada < _titulosAbas.Length - 1) {
            AtualizarAba(_abaSelecionada + 1);
        }
    }

    // ATUALIZA A ABA SELECIONADA.
    private void AtualizarAba(int indice)
    {
        if (indice < 0 || indice >= _titulosAbas.Length)
            return;

        _abaSelecionada = indice;

        // Atualiza o título central do carrossel.
        TextoAbaSelecionada.Text = _titulosAbas[indice];

        bool resumo = indice == 0;
        bool financiamentos = indice == 1;
        bool projecoes = indice == 2;

        // Exibe somente o conteúdo da aba selecionada.
        ConteudoResumo.IsVisible = resumo;
        ConteudoFinanciamentos.IsVisible = financiamentos;
        ConteudoProjecoes.IsVisible = projecoes;

        // Configura o botão inferior.
        BtnAcaoFinanceira.IsVisible = !projecoes;

        if (resumo) {
            BtnAcaoFinanceira.Text = "+ NOVA DESPESA";
        }
        else if (financiamentos) {
            BtnAcaoFinanceira.Text = "+ NOVA DÍVIDA";
        }

        // Desativa a seta esquerda na primeira aba.
        BtnAbaAnterior.IsEnabled = indice > 0;
        BtnAbaAnterior.Opacity = indice > 0 ? 1 : 0.3;

        // Desativa a seta direita na última aba.
        BtnProximaAba.IsEnabled =
            indice < _titulosAbas.Length - 1;

        BtnProximaAba.Opacity =
            indice < _titulosAbas.Length - 1 ? 1 : 0.3;
    }

    // NAVEGAÇÃO PARA NOVA DESPESA OU NOVA DÍVIDA.
    private async void OnAcaoFinanceiraClicked (object? sender,EventArgs e) {
        if (_abaSelecionada == 0) {
            await Navigation.PushAsync(new NovaDespesaPag());
        }
        else if (_abaSelecionada == 1) {
            await Navigation.PushAsync(new NovaDividaPage());
        }
    }
}