
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Graphics;

namespace InovaLog.ViewModels;

public class GestorFrotasViewModel
{
    public ObservableCollection<VeiculoFrotaItem> Veiculos { get; } = new()
    {
        new VeiculoFrotaItem(
            "NV-2026",
            "Volvo FH 540 6x4",
            "32 t",
            "158400 KM",
            "Em viagem",
            Color.FromArgb("#F59E0B")),

        new VeiculoFrotaItem(
            "DAF-7788",
            "DAF XF 530",
            "30 t",
            "59200 KM",
            "Disponível",
            Color.FromArgb("#10B981"))
    };

    public ObservableCollection<RotaPadraoItem> Rotas { get; } = new()
    {
        new RotaPadraoItem("Sete Lagoas - MG → Santana - BA", "980 KM"),
        new RotaPadraoItem("Belo Horizonte - MG → São Paulo - SP", "586 KM"),
        new RotaPadraoItem("Varginha - MG → Rio de Janeiro - RJ", "335 KM")
    };

    public ObservableCollection<PlanoRevisaoItem> Planos { get; } = new()
    {
        new PlanoRevisaoItem(
            "Revisão Pesada de 60 mil KM",
            "DAF-7788",
            "59200",
            "60000",
            "800"),

        new PlanoRevisaoItem(
            "Troca de Óleo e Filtros (160 mil KM)",
            "NV-2026",
            "158400",
            "160000",
            "1600")
    };

    public ObservableCollection<HistoricoServicoItem> Historico { get; } = new()
    {
        new HistoricoServicoItem(
            "Alinhamento Eixo Livre Carreta + Direção",
            "INV-2026 • Realizado aos 150000 KM • Oficina LP Sul",
            "R$ 689,74")
    };
}

public class VeiculoFrotaItem
{
    public string Codigo { get; }
    public string Modelo { get; }
    public string Capacidade { get; }
    public string Hodometro { get; }
    public string Status { get; }
    public Color StatusCor { get; }

    public VeiculoFrotaItem(
        string codigo,
        string modelo,
        string capacidade,
        string hodometro,
        string status,
        Color statusCor)
    {
        Codigo = codigo;
        Modelo = modelo;
        Capacidade = capacidade;
        Hodometro = hodometro;
        Status = status;
        StatusCor = statusCor;
    }
}

public class RotaPadraoItem : INotifyPropertyChanged
{
    public string Trecho { get; }
    public string Distancia { get; }

    private bool _ativa = true;

    public bool Ativa
    {
        get => _ativa;
        private set
        {
            if (_ativa == value) return;
            _ativa = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusTexto));
            OnPropertyChanged(nameof(StatusCor));
            OnPropertyChanged(nameof(AcaoTexto));
            OnPropertyChanged(nameof(AcaoCor));
        }
    }

    public string StatusTexto => Ativa ? "Ativa" : "Inativa";
    public Color StatusCor => Color.FromArgb(Ativa ? "#10B981" : "#EF4444");
    public string AcaoTexto => Ativa ? "Inativar" : "Ativar";
    public Color AcaoCor => Color.FromArgb(Ativa ? "#EF4444" : "#10B981");

    public ICommand AlternarCommand { get; }

    public RotaPadraoItem(string trecho, string distancia)
    {
        Trecho = trecho;
        Distancia = distancia;
        AlternarCommand = new Command(() => Ativa = !Ativa);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? nome = null)
    {
        PropertyChanged?.Invoke(
            this, new PropertyChangedEventArgs(nome));
    }
}

public class PlanoRevisaoItem
{
    public string Titulo { get; }
    public string Veiculo { get; }
    public string KmAtual { get; }
    public string KmAlvo { get; }
    public string KmFaltantes { get; }

    public PlanoRevisaoItem(
        string titulo,
        string veiculo,
        string kmAtual,
        string kmAlvo,
        string kmFaltantes)
    {
        Titulo = titulo;
        Veiculo = veiculo;
        KmAtual = kmAtual;
        KmAlvo = kmAlvo;
        KmFaltantes = kmFaltantes;
    }
}

public class HistoricoServicoItem
{
    public string Titulo { get; }
    public string Descricao { get; }
    public string Valor { get; }

    public HistoricoServicoItem(
        string titulo,
        string descricao,
        string valor)
    {
        Titulo = titulo;
        Descricao = descricao;
        Valor = valor;
    }
}
