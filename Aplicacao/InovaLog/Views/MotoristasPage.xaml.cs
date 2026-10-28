using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace InovaLog.Views;

public partial class MotoristasPage : ContentPage
{
    private int _abaSelecionada;

    private bool _viagemConcluida;

    private readonly string[] _titulosAbas =
    {
        "Viagens",
        "Em Andamento",
        "Concluídas",
        "Retornos Vazios"
    };

    public MotoristasPage()
    {
        InitializeComponent();
        AtualizarAba(0);
    }

    private void OnAbaAnteriorClicked(object sender, EventArgs e)
    {
        if (_abaSelecionada > 0)
        {
            AtualizarAba(_abaSelecionada - 1);
        }
    }

    private void OnProximaAbaClicked(object sender, EventArgs e)
    {
        if (_abaSelecionada < _titulosAbas.Length - 1)
        {
            AtualizarAba(_abaSelecionada + 1);
        }
    }

    private void AtualizarAba(int indice)
    {
        if (indice < 0 || indice >= _titulosAbas.Length)
            return;

        _abaSelecionada = indice;

        // Atualiza o nome da aba central e o título da seção.
        TextoAbaSelecionada.Text = _titulosAbas[indice];
        TituloSecao.Text = indice == 0
            ? "Todas as Viagens"
            : _titulosAbas[indice];

        // Desativa a seta esquerda na primeira aba.
        BtnAbaAnterior.IsEnabled = indice > 0;
        BtnAbaAnterior.Opacity = indice > 0 ? 1 : 0.3;

        // Desativa a seta direita na última aba.
        BtnProximaAba.IsEnabled = indice < _titulosAbas.Length - 1;
        BtnProximaAba.Opacity = indice < _titulosAbas.Length - 1 ? 1 : 0.3;

        // Mostra os cards correspondentes à aba selecionada.
        CardRetorno.IsVisible = indice == 0 || indice == 3;
        CardViagem.IsVisible = indice == 0 || indice == 1;
        CardViagemConcluida.IsVisible = indice == 0 || indice == 2;

        AtualizarStatusViagem();
    }

    private void AtualizarStatusViagem()
    {
        if (_viagemConcluida)
        {
            TextoStatus.Text = "Concluída";
            StatusViagem.BackgroundColor = Color.FromArgb("#DCFCE7");
            TextoStatus.TextColor = Color.FromArgb("#166534");

            BotaoGasto.IsVisible = true;
            BotaoFinalizar.IsVisible = false;
        }
        else
        {
            TextoStatus.Text = "Em andamento";
            StatusViagem.BackgroundColor = Color.FromArgb("#FEF3C7");
            TextoStatus.TextColor = Color.FromArgb("#92400E");

            BotaoGasto.IsVisible = true;
            BotaoFinalizar.IsVisible = true;
        }
    }

    private async void OnNovaViagemClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Nova Viagem",
            "A tela de cadastro de viagem será desenvolvida em uma próxima etapa.",
            "OK");
    }

    private async void OnLancarGastoClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Gasto de Estrada",
            "O lançamento de gastos e notinhas será desenvolvido posteriormente.",
            "OK");
    }

    private async void OnFinalizarViagemClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Finalizar Viagem",
            "Deseja marcar a viagem VG-1300 como concluída nesta prévia?",
            "Finalizar",
            "Cancelar");

        if (!confirmar)
            return;

        _viagemConcluida = true;
        AtualizarStatusViagem();

        // Atualiza a visualização para a aba Concluídas.
        AtualizarAba(2);

        await DisplayAlert(
            "Viagem concluída",
            "O status foi atualizado apenas nesta prévia. Nenhum dado foi salvo no sistema.",
            "OK");
    }

    private void OnMinhasViagensClicked(object sender, EventArgs e)
    {
        AtualizarAba(0);
    }

    private async void OnGastoEstradaClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Gasto de Estrada",
            "Esta seção será desenvolvida posteriormente.",
            "OK");
    }
}