using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace InovaLog.ViewModels;

// Cada linha da lista "Últimas Transações"
public class TransacaoItem
{
    public string Titulo { get; set; } = "";
    public string Data { get; set; } = "";
    public string Valor { get; set; } = "";
    public string Icone { get; set; } = "";
    public string CorFundoIcone { get; set; } = "";
    public string CorValor { get; set; } = "";
}

public partial class FinanceiroViewModel : ObservableObject
{
    private static readonly CultureInfo Br = new("pt-BR");

    [ObservableProperty] private string mesReferencia = "";
    [ObservableProperty] private string saldoTexto = "";
    [ObservableProperty] private string receitasTexto = "";
    [ObservableProperty] private string despesasTexto = "";
    [ObservableProperty] private string combustivelTexto = "";

    [ObservableProperty] private ISeries[] series = [];
    [ObservableProperty] private Axis[] xAxes = [];
    [ObservableProperty] private Axis[] yAxes = [];

    public ObservableCollection<TransacaoItem> Transacoes { get; } = new();

    public FinanceiroViewModel()
    {
        // TROCAR PELO BACK-END: estes valores fixos serão substituídos pelos dados reais
        CarregarDados(
            mes: "Setembro",
            saldo: 12450,
            receitas: 18200,
            despesas: 5750,
            combustivel: 3120,
            transacoes: new[]
            {
                new TransacaoItem { Titulo = "Frete - Varginha → São P ..", Data = "Ontem",
                    Valor = "+ R$ 4.200,00", Icone = "fileinvoicedollarsolidfull.png",
                    CorFundoIcone = "#D1FAE5", CorValor = "#10B981" },
                new TransacaoItem { Titulo = "Abastecimento - Posto BR", Data = "24/09/2026",
                    Valor = "- R$ 680,00", Icone = "gaspumpsolidfull.png",
                    CorFundoIcone = "#FEF3C7", CorValor = "#F59E0B" },
                new TransacaoItem { Titulo = "Pedágio - Via Dutra", Data = "23/09/2026",
                    Valor = "- R$ 680,00", Icone = "dollarsignsolidfull.png",
                    CorFundoIcone = "#FCA5A5", CorValor = "#EF4444" }
            });
    }

    // Único ponto que o back-end vai alimentar
    public void CarregarDados(string mes, double saldo, double receitas, double despesas,
                              double combustivel, IEnumerable<TransacaoItem> transacoes)
    {
        MesReferencia = mes;
        SaldoTexto = saldo.ToString("C", Br);
        ReceitasTexto = receitas.ToString("C", Br);
        DespesasTexto = despesas.ToString("C", Br);
        CombustivelTexto = combustivel.ToString("C", Br);

        Transacoes.Clear();
        foreach (var t in transacoes)
            Transacoes.Add(t);

        AtualizarGrafico(receitas, despesas, combustivel);
    }

    private static ISeries CriarBarra(string nome, string cor, double?[] valores)
    {
        return new StackedColumnSeries<double?>
        {
            Name = nome,
            Values = valores,
            Fill = new SolidColorPaint(SKColor.Parse(cor)),
            MaxBarWidth = 150,
            Rx = 10,
            Ry = 10
        };
    }

    private void AtualizarGrafico(double receitas, double despesas, double combustivel)
    {
        // Topo do eixo: próximo múltiplo de 5 mil
        double maximo = Math.Max(receitas, Math.Max(despesas, combustivel));
        double limiteY = Math.Max(5000, Math.Ceiling(maximo / 5000) * 5000);

        int quantidade = (int)(limiteY / 5000) + 1;
        double[] separadores = new double[quantidade];
        for (int i = 0; i < quantidade; i++)
            separadores[i] = i * 5000;

        Series = new ISeries[]
        {
            CriarBarra("Receitas",        "#10B981", new double?[] { receitas, null, null }),
            CriarBarra("Despesas Gerais", "#EF4444", new double?[] { null, despesas, null }),
            CriarBarra("Combustível",     "#F59E0B", new double?[] { null, null, combustivel })
        };

        XAxes = new Axis[]
        {
            new Axis
            {
                Labels = new[]
                {
                    receitas.ToString("C", Br),
                    despesas.ToString("C", Br),
                    combustivel.ToString("C", Br)
                },
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#0F172A")),
                TextSize = 13,
                MinStep = 1,
                ForceStepToMin = true
            }
        };

        YAxes = new Axis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = limiteY,
                CustomSeparators = separadores,
                Labeler = valor => valor == 0 ? "0" : $"{valor / 1000:0}k",
                TextSize = 15,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B")),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E2E8F0"))
                {
                    StrokeThickness = 1
                }
            }
        };
    }
}