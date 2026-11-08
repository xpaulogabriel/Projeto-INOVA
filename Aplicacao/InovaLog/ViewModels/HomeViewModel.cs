using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Services;
using InovaLog.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace InovaLog.ViewModels;

// ==================================================================
// ITENS EXIBIDOS NA TELA INICIAL
// ==================================================================

/// <summary>Linha de "Últimas Atividades".</summary>
public class AtividadeViagem
{
    public string Codigo { get; init; } = string.Empty;
    public string Rota { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Color CorFundoStatus { get; init; } = Colors.Transparent;
    public Color CorTextoStatus { get; init; } = Colors.Black;
    public string CustoPorKm { get; init; } = string.Empty;
    public string Liquido { get; init; } = string.Empty;
    public Color CorLiquido { get; init; } = Colors.Black;
}

/// <summary>Linha do "Top 3 Rotas Mais Lucrativas".</summary>
public class RotaLucrativa
{
    public int Posicao { get; init; }
    public string PosicaoTexto => $"{Posicao}º";
    public string Rota { get; init; } = string.Empty;
    public decimal ValorLiquido { get; init; }
    public string ValorTexto { get; init; } = string.Empty;
    public Color CorValor { get; init; } = Colors.Black;
    public Color CorFundoPosicao { get; init; } = Colors.Transparent;
    public Color CorTextoPosicao { get; init; } = Colors.Black;
}

/// <summary>Resultado da pesquisa rápida.</summary>
public class ResultadoBusca
{
    public string Titulo { get; init; } = string.Empty;
    public string Detalhe { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public ModuloApp Modulo { get; init; }
}

// ==================================================================
// VIEW MODEL DA TELA INICIAL
// ==================================================================

public partial class HomeViewModel : ObservableObject
{
    private static readonly CultureInfo Pt = new("pt-BR");

    // ---------------- DADOS FICTÍCIOS ----------------
    // Troque pelos valores reais quando o back-end estiver pronto.
    private const decimal MetaMensalFretes = 65000m;
    private const decimal ValorFaturadoMes = 32500m;

    private readonly List<ResultadoBusca> _indiceBusca;

    // ---------------- CABEÇALHO ----------------
    [ObservableProperty] private string nomeUsuario = string.Empty;
    [ObservableProperty] private string rotuloPerfil = string.Empty;
    [ObservableProperty] private string iniciais = string.Empty;
    [ObservableProperty] private string textoStatus = "ONLINE";
    [ObservableProperty] private Color corStatus = Color.FromArgb("#10B981");

    // ---------------- PERMISSÕES ----------------
    [ObservableProperty] private bool acessoViagens;
    [ObservableProperty] private bool acessoFrota;
    [ObservableProperty] private bool acessoFinanceiro;
    [ObservableProperty] private bool mostrarAtividades;
    [ObservableProperty] private bool mostrarFinanceiro;

    // ---------------- PESQUISA RÁPIDA ----------------
    [ObservableProperty] private string textoBusca = string.Empty;
    [ObservableProperty] private bool mostrarResultados;
    [ObservableProperty] private bool semResultados;

    public ObservableCollection<ResultadoBusca> Resultados { get; } = new();

    // ---------------- RESUMO DO DIA ----------------
    public string DataHoje { get; }
    public int ViagensEmAndamento { get; } = 3;
    public int VeiculosEmRota { get; } = 4;
    public int VeiculosNaGaragem { get; } = 6;

    // ---------------- ÚLTIMAS ATIVIDADES / TOP 3 ----------------
    public IReadOnlyList<AtividadeViagem> UltimasAtividades { get; }
    public IReadOnlyList<RotaLucrativa> TopRotas { get; }

    // ---------------- META MENSAL ----------------
    public string MesMeta { get; }
    public string MetaTexto { get; private set; } = string.Empty;
    public string FaturadoTexto { get; private set; } = string.Empty;
    public string RestanteTexto { get; private set; } = string.Empty;
    public string PercentualTexto { get; private set; } = string.Empty;
    public double ProgressoMeta { get; private set; }

    public HomeViewModel()
    {
        var agora = DateTime.Now;

        DataHoje = $"Hoje, {agora.ToString("dd/MM", Pt)}";

        var mes = Pt.DateTimeFormat.GetMonthName(agora.Month);
        MesMeta = char.ToUpper(mes[0], Pt) + mes[1..];

        UltimasAtividades = CriarAtividades();
        TopRotas = CriarTopRotas();
        _indiceBusca = CriarIndiceBusca();

        CalcularMeta();
        AtualizarSessao();
    }

    // ==============================================================
    // SESSÃO / PERMISSÕES  (chamado no OnAppearing da página)
    // ==============================================================
    public void AtualizarSessao()
    {
        NomeUsuario = SessaoPerfil.NomeUsuario;
        RotuloPerfil = SessaoPerfil.RotuloPerfil(SessaoPerfil.PerfilAtual);
        Iniciais = SessaoPerfil.Iniciais(NomeUsuario);

        AcessoViagens = SessaoPerfil.TemAcesso(ModuloApp.Viagens);
        AcessoFrota = SessaoPerfil.TemAcesso(ModuloApp.Frota);
        AcessoFinanceiro = SessaoPerfil.TemAcesso(ModuloApp.Financeiro);
        MostrarAtividades = SessaoPerfil.PodeVerAtividades();
        MostrarFinanceiro = SessaoPerfil.PodeVerFinanceiroNaHome();

        var online = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        TextoStatus = online ? "ONLINE" : "OFFLINE";
        CorStatus = online ? Color.FromArgb("#10B981") : Color.FromArgb("#F59E0B");
    }

    // ==============================================================
    // META MENSAL: o progresso é calculado a partir do valor faturado
    // ==============================================================
    private void CalcularMeta()
    {
        var restante = Math.Max(0m, MetaMensalFretes - ValorFaturadoMes);
        var percentual = MetaMensalFretes > 0
            ? (double)(ValorFaturadoMes / MetaMensalFretes)
            : 0d;

        MetaTexto = Brl(MetaMensalFretes);
        FaturadoTexto = Brl(ValorFaturadoMes);
        RestanteTexto = Brl(restante);
        PercentualTexto = $"{Math.Round(percentual * 100, MidpointRounding.AwayFromZero):0}%";
        ProgressoMeta = Math.Clamp(percentual, 0d, 1d);
    }

    // ==============================================================
    // PESQUISA RÁPIDA
    // ==============================================================
    partial void OnTextoBuscaChanged(string value) => Buscar(value);

    private void Buscar(string? texto)
    {
        Resultados.Clear();

        var termo = (texto ?? string.Empty).Trim();

        if (termo.Length < 2)
        {
            MostrarResultados = false;
            SemResultados = false;
            return;
        }

        var termoNormalizado = Normalizar(termo);

        var encontrados = _indiceBusca
            .Where(item => SessaoPerfil.TemAcesso(item.Modulo))
            .Where(item => Normalizar($"{item.Titulo} {item.Detalhe}").Contains(termoNormalizado))
            .Take(6);

        foreach (var item in encontrados)
            Resultados.Add(item);

        SemResultados = Resultados.Count == 0;
        MostrarResultados = true;
    }

    private static string Normalizar(string texto)
    {
        var decomposto = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().ToLowerInvariant();
    }

    // ==============================================================
    // COMANDOS
    // ==============================================================

    [RelayCommand]
    private async Task TrocarContaAsync()
    {
        await Shell.Current.GoToAsync(nameof(TrocarContaPage));
    }

    // CommandParameter: Viagens, Frota, Financeiro ou Configuracoes
    [RelayCommand]
    private async Task AbrirModuloAsync(string? nome)
    {
        if (Enum.TryParse<ModuloApp>(nome, out var modulo))
            await IrParaModuloAsync(modulo);
    }

    [RelayCommand]
    private async Task AbrirResultadoAsync(ResultadoBusca? resultado)
    {
        if (resultado == null)
            return;

        TextoBusca = string.Empty;
        await IrParaModuloAsync(resultado.Modulo);
    }

    [RelayCommand]
    private async Task ExportarCsvAsync()
    {
        try
        {
            var csv = new StringBuilder();
            csv.AppendLine("Posição;Rota;Líquido (R$)");

            foreach (var rota in TopRotas)
            {
                csv.AppendLine(
                    $"{rota.Posicao};{Csv(rota.Rota)};{rota.ValorLiquido.ToString("N2", Pt)}");
            }

            var nomeArquivo = $"top3_rotas_lucrativas_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            var caminho = Path.Combine(FileSystem.CacheDirectory, nomeArquivo);

            // UTF-8 com BOM: o Excel abre com os acentos corretos.
            await File.WriteAllTextAsync(caminho, csv.ToString(), new UTF8Encoding(true));

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Exportar CSV - Top 3 Rotas Mais Lucrativas",
                File = new ShareFile(caminho)
            });
        }
        catch (Exception erro)
        {
            await ExibirAlerta("Erro ao exportar", erro.Message);
        }
    }

    // ==============================================================
    // APOIO
    // ==============================================================

    private static async Task IrParaModuloAsync(ModuloApp modulo)
    {
        if (!SessaoPerfil.TemAcesso(modulo))
        {
            await ExibirAlerta("Acesso restrito", "Seu perfil não tem permissão para acessar este módulo.");
            return;
        }

        await Shell.Current.GoToAsync($"//{SessaoPerfil.RotaDe(modulo)}");
    }

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;

        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }

    private static string Csv(string texto) => $"\"{texto.Replace("\"", "\"\"")}\"";

    private static string Brl(decimal valor) => valor.ToString("C2", Pt);

    // ==============================================================
    // DADOS FICTÍCIOS
    // ==============================================================

    private static List<AtividadeViagem> CriarAtividades()
    {
        // Mesmas cores de status usadas na página de Viagens.
        var fundoConcluida = Color.FromArgb("#DCFCE7");
        var textoConcluida = Color.FromArgb("#166534");
        var fundoAndamento = Color.FromArgb("#FEF3C7");
        var textoAndamento = Color.FromArgb("#92400E");

        var verde = Color.FromArgb("#10B981");
        var vermelho = Color.FromArgb("#EF4444");
        var cinza = Color.FromArgb("#64748B");

        return new List<AtividadeViagem>
        {
            new()
            {
                Codigo = "VG-1300 (Ida)",
                Rota = "Sete Lagoas → Santana",
                Status = "CONCLUÍDA",
                CorFundoStatus = fundoConcluida,
                CorTextoStatus = textoConcluida,
                CustoPorKm = $"{9.52m.ToString("C2", Pt)}/km",
                Liquido = $"Líquido: {15881m.ToString("C2", Pt)}",
                CorLiquido = verde
            },
            new()
            {
                Codigo = "VG-1301 (Vazio)",
                Rota = "Sete Lagoas → Santana",
                Status = "CONCLUÍDA",
                CorFundoStatus = fundoConcluida,
                CorTextoStatus = textoConcluida,
                CustoPorKm = $"{1.62m.ToString("C2", Pt)}/km",
                Liquido = $"Líquido: {(-649m).ToString("C2", Pt)}",
                CorLiquido = vermelho
            },
            new()
            {
                Codigo = "VG-1302 (Ida)",
                Rota = "Varginha → Três Pontas",
                Status = "EM ANDAMENTO",
                CorFundoStatus = fundoAndamento,
                CorTextoStatus = textoAndamento,
                CustoPorKm = $"{0m.ToString("C2", Pt)}/km",
                Liquido = $"Líquido: {0m.ToString("C2", Pt)}",
                CorLiquido = cinza
            }
        };
    }

    private static List<RotaLucrativa> CriarTopRotas()
    {
        var verde = Color.FromArgb("#10B981");
        var cinza = Color.FromArgb("#64748B");

        return new List<RotaLucrativa>
        {
            new()
            {
                Posicao = 1,
                Rota = "Sete Lagoas - MG → Santana - BA",
                ValorLiquido = 15881m,
                ValorTexto = Brl(15881m),
                CorValor = verde,
                CorFundoPosicao = Color.FromArgb("#FEF3C7"),
                CorTextoPosicao = Color.FromArgb("#92400E")
            },
            new()
            {
                Posicao = 2,
                Rota = "São Bernardo do Campo - SP → Betim - MG",
                ValorLiquido = 0m,
                ValorTexto = Brl(0m),
                CorValor = cinza,
                CorFundoPosicao = Color.FromArgb("#F1F5F9"),
                CorTextoPosicao = Color.FromArgb("#475569")
            },
            new()
            {
                Posicao = 3,
                Rota = "Varginha - MG → Três Pontas - MG",
                ValorLiquido = 0m,
                ValorTexto = Brl(0m),
                CorValor = cinza,
                CorFundoPosicao = Color.FromArgb("#FFEDD5"),
                CorTextoPosicao = Color.FromArgb("#C2410C")
            }
        };
    }

    private static List<ResultadoBusca> CriarIndiceBusca()
    {
        return new List<ResultadoBusca>
        {
            new() { Titulo = "VG-1300 (Ida)", Detalhe = "Sete Lagoas → Santana", Tipo = "VIAGEM", Modulo = ModuloApp.Viagens },
            new() { Titulo = "VG-1301 (Vazio)", Detalhe = "Sete Lagoas → Santana", Tipo = "VIAGEM", Modulo = ModuloApp.Viagens },
            new() { Titulo = "VG-1302 (Ida)", Detalhe = "Varginha → Três Pontas", Tipo = "VIAGEM", Modulo = ModuloApp.Viagens },
            new() { Titulo = "Scania R450", Detalhe = "Placa QWE-1A23", Tipo = "VEÍCULO", Modulo = ModuloApp.Frota },
            new() { Titulo = "Volvo FH 540", Detalhe = "Placa RTY-4B56", Tipo = "VEÍCULO", Modulo = ModuloApp.Frota },
            new() { Titulo = "Carreta Baú", Detalhe = "Placa JKL-7C89", Tipo = "VEÍCULO", Modulo = ModuloApp.Frota },
            new() { Titulo = "Sete Lagoas - MG → Santana - BA", Detalhe = "Rota padrão", Tipo = "ROTA", Modulo = ModuloApp.Frota },
            new() { Titulo = "São Bernardo do Campo - SP → Betim - MG", Detalhe = "Rota padrão", Tipo = "ROTA", Modulo = ModuloApp.Frota },
            new() { Titulo = "Varginha - MG → Três Pontas - MG", Detalhe = "Rota padrão", Tipo = "ROTA", Modulo = ModuloApp.Frota }
        };
    }
}
