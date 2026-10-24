
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace InovaLog.ViewModels;

public class FinanceiroViewModel
{
    // Dados temporários para visualizar o gráfico.
    // Depois, substitua pelos valores reais do financeiro.
    private readonly double _receitas = 18200;
    private readonly double _despesas = 5750;
    private readonly double _combustivel = 3120;

    public ISeries[] Series { get; }
    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }

    public FinanceiroViewModel()
    {
        Series = CriarSeries();
        XAxes = CriarEixoHorizontal();
        YAxes = CriarEixoVertical();
    }

    // Cria as três barras, uma para cada categoria.
    private ISeries[] CriarSeries()
    {
        return new ISeries[]
        {
            CriarBarra(
                "Receitas",
                new double?[] { _receitas, null, null },
                "#10B981"),

            CriarBarra(
                "Despesas Gerais",
                new double?[] { null, _despesas, null },
                "#EF4444"),

            CriarBarra(
                "Combustível",
                new double?[] { null, null, _combustivel },
                "#F59E0B")
        };
    }

    // Define a aparência de cada barra.
    private StackedColumnSeries<double?> CriarBarra(
        string nome,
        double?[] valores,
        string cor)
    {
        return new StackedColumnSeries<double?>
        {
            Name = nome,
            Values = valores,
            Fill = new SolidColorPaint(SKColor.Parse(cor)),
            Stroke = null,
            MaxBarWidth = 150,
            Rx = 10,
            Ry = 10
        };
    }

    // Configura as categorias na parte inferior do gráfico.
    private Axis[] CriarEixoHorizontal()
    {
        var culturaBrasileira =
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR");

        return new Axis[]
        {
        new Axis
        {
            Labels = new[]
            {
                _receitas.ToString("C0", culturaBrasileira),
                _despesas.ToString("C0", culturaBrasileira),
                _combustivel.ToString("C0", culturaBrasileira)
            },

            LabelsPaint = new SolidColorPaint(
                SKColor.Parse("#0F172A")),

            TextSize = 13,
            MinStep = 1,
            ForceStepToMin = true
        }
        };
    }


    // Configura a escala vertical e as linhas de referência.
    private Axis[] CriarEixoVertical()
    {
        double maiorValor = Math.Max(
            _receitas,
            Math.Max(_despesas, _combustivel));

        double limiteY = Math.Max(
            5000,
            Math.Ceiling(maiorValor / 5000) * 5000);

        double[] separadores = CriarSeparadores(limiteY);

        return new Axis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = limiteY,
                CustomSeparators = separadores,

                Labeler = valor =>
                    valor == 0
                        ? "0"
                        : $"{valor / 1000:0}k",

                TextSize = 15,

                LabelsPaint = new SolidColorPaint(
                    SKColor.Parse("#64748B")),

                SeparatorsPaint = new SolidColorPaint(
                    SKColor.Parse("#E2E8F0"))
                {
                    StrokeThickness = 1
                }
            }
        };
    }

    // Gera as marcações do eixo de R$ 5.000 em R$ 5.000.
    private double[] CriarSeparadores(double limiteY)
    {
        int quantidade = (int)(limiteY / 5000) + 1;

        double[] separadores = new double[quantidade];

        for (int i = 0; i < quantidade; i++)
        {
            separadores[i] = i * 5000;
        }

        return separadores;
    }
}