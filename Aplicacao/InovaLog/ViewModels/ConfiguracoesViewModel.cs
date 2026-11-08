using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Services;
using InovaLog.Views;
using Microsoft.Maui.Graphics;

namespace InovaLog.ViewModels;

// ==================================================================
// ITENS DAS LISTAS DE CONFIGURAÇÕES
// ==================================================================

/// <summary>Cartão da aba "Usuários".</summary>
public class UsuarioItem
{
    private static readonly Color Azul = Color.FromArgb("#2C63B1");
    private static readonly Color AzulClaro = Color.FromArgb("#E0F2FE");

    public PerfilUsuario Perfil { get; init; }
    public string NomeCurto { get; init; } = string.Empty;      // usado no cabeçalho da Home
    public string NomeExibicao { get; init; } = string.Empty;   // ex.: "Pedro Santos (Gestor Frota)"
    public string Email { get; init; } = string.Empty;
    public string Documentos { get; init; } = string.Empty;     // CNH / CPF (só motoristas)
    public string RotuloPerfil { get; init; } = string.Empty;   // ADMINISTRADOR, GESTOR...
    public bool Ativo { get; init; } = true;
    public bool EhContaAtual { get; init; }

    public bool TemDocumentos => !string.IsNullOrWhiteSpace(Documentos);

    // Botão "Conta Atual" / "Usar Conta"
    public string TextoBotao => EhContaAtual ? "Conta Atual" : "Usar Conta";
    public Color CorFundoBotao => EhContaAtual ? Azul : AzulClaro;
    public Color CorTextoBotao => EhContaAtual ? Colors.White : Azul;
    public double OpacidadeBotao => Ativo ? 1 : 0.5;

    // Selo "Ativo" / "Bloqueado"
    public string TextoStatus => Ativo ? "Ativo" : "Bloqueado";
    public Color CorFundoStatus => Ativo ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoStatus => Ativo ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    // Destaque do cartão da conta em uso
    public Brush Borda => new SolidColorBrush(EhContaAtual ? Azul : Color.FromArgb("#E2E8F0"));
    public double EspessuraBorda => EhContaAtual ? 2 : 1;
}

/// <summary>Linha da aba "Categorias".</summary>
public class CategoriaItem : ObservableObject
{
    private bool _ativa = true;

    public string Nome { get; init; } = string.Empty;
    public bool Entrada { get; init; }

    public bool Ativa
    {
        get => _ativa;
        set
        {
            if (SetProperty(ref _ativa, value))
            {
                OnPropertyChanged(nameof(TextoBotao));
                OnPropertyChanged(nameof(CorFundoBotao));
                OnPropertyChanged(nameof(CorTextoBotao));
                OnPropertyChanged(nameof(OpacidadeLinha));
            }
        }
    }

    public string TipoTexto => Entrada ? "ENTRADA" : "SAÍDA";
    public Color CorFundoTipo => Entrada ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoTipo => Entrada ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    public string TextoBotao => Ativa ? "Inativar" : "Reativar";
    public Color CorFundoBotao => Ativa ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#D1FAE5");
    public Color CorTextoBotao => Ativa ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;
}

/// <summary>Linha da aba "Pagamentos".</summary>
public class FormaPagamentoItem : ObservableObject
{
    private bool _ativa = true;

    public string Nome { get; init; } = string.Empty;

    public bool Ativa
    {
        get => _ativa;
        set
        {
            if (SetProperty(ref _ativa, value))
            {
                OnPropertyChanged(nameof(TextoStatus));
                OnPropertyChanged(nameof(CorFundoStatus));
                OnPropertyChanged(nameof(CorTextoStatus));
                OnPropertyChanged(nameof(OpacidadeLinha));
            }
        }
    }

    public string TextoStatus => Ativa ? "Ativo" : "Inativo";
    public Color CorFundoStatus => Ativa ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#F1F5F9");
    public Color CorTextoStatus => Ativa ? Color.FromArgb("#10B981") : Color.FromArgb("#64748B");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;
}

// ==================================================================
// VIEW MODEL DA TELA DE CONFIGURAÇÕES
// ==================================================================

public partial class ConfiguracoesViewModel : ObservableObject
{
    // ---------------- USUÁRIOS ----------------
    public ObservableCollection<UsuarioItem> Usuarios { get; } = new();

    // ---------------- CATEGORIAS (dados fictícios) ----------------
    public ObservableCollection<CategoriaItem> Categorias { get; } = new()
    {
        new CategoriaItem { Nome = "Fretes", Entrada = true },
        new CategoriaItem { Nome = "Combustível" },
        new CategoriaItem { Nome = "Pedágio" },
        new CategoriaItem { Nome = "Salário Motorista" },
        new CategoriaItem { Nome = "Financiamento" },
        new CategoriaItem { Nome = "Manutenção" }
    };

    [ObservableProperty] private string novaCategoria = string.Empty;

    // 0 = Entrada, 1 = Saída (padrão do Picker)
    [ObservableProperty] private int tipoCategoriaIndex = 1;

    // ---------------- FORMAS DE PAGAMENTO (dados fictícios) ----------------
    public ObservableCollection<FormaPagamentoItem> FormasPagamento { get; } = new()
    {
        new FormaPagamentoItem { Nome = "PIX" },
        new FormaPagamentoItem { Nome = "Boleto" },
        new FormaPagamentoItem { Nome = "Cartão Frota" },
        new FormaPagamentoItem { Nome = "Dinheiro" }
    };

    [ObservableProperty] private string novaFormaPagamento = string.Empty;

    // ---------------- MINHA CONTA (perfis sem abas administrativas) ----------------
    [ObservableProperty] private string nomeConta = string.Empty;
    [ObservableProperty] private string emailConta = string.Empty;
    [ObservableProperty] private string rotuloPerfilConta = string.Empty;

    public ConfiguracoesViewModel()
    {
        AtualizarSessao();
    }

    // Chamado no OnAppearing da página.
    public void AtualizarSessao()
    {
        NomeConta = SessaoPerfil.NomeUsuario;
        EmailConta = SessaoPerfil.EmailUsuario;
        RotuloPerfilConta = SessaoPerfil.RotuloPerfil(SessaoPerfil.PerfilAtual);

        Usuarios.Clear();

        foreach (var usuario in CriarUsuarios())
            Usuarios.Add(usuario);
    }

    // ==============================================================
    // USUÁRIOS
    // ==============================================================

    [RelayCommand]
    private async Task NovoUsuarioAsync()
    {
        // Reaproveita a página de cadastro que já existe.
        await Shell.Current.GoToAsync(nameof(CadastroUsuarioPage));
    }

    [RelayCommand]
    private async Task UsarContaAsync(UsuarioItem? usuario)
    {
        if (usuario == null || usuario.EhContaAtual)
            return;

        if (!usuario.Ativo)
        {
            await ExibirAlerta("Conta bloqueada", "Esta conta está bloqueada e não pode ser utilizada.");
            return;
        }

        SessaoPerfil.Definir(usuario.Perfil, usuario.NomeCurto, usuario.Email);

        await Shell.Current.GoToAsync("//HomePage");
    }

    // ==============================================================
    // CATEGORIAS
    // ==============================================================

    [RelayCommand]
    private async Task IncluirCategoriaAsync()
    {
        var nome = (NovaCategoria ?? string.Empty).Trim();

        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe o nome da categoria.");
            return;
        }

        if (Categorias.Any(c => string.Equals(c.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa categoria já existe.");
            return;
        }

        Categorias.Add(new CategoriaItem { Nome = nome, Entrada = TipoCategoriaIndex == 0 });
        NovaCategoria = string.Empty;
    }

    [RelayCommand]
    private void AlternarCategoria(CategoriaItem? categoria)
    {
        if (categoria != null)
            categoria.Ativa = !categoria.Ativa;
    }

    // ==============================================================
    // FORMAS DE PAGAMENTO
    // ==============================================================

    [RelayCommand]
    private async Task IncluirFormaPagamentoAsync()
    {
        var nome = (NovaFormaPagamento ?? string.Empty).Trim();

        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe a forma de pagamento.");
            return;
        }

        if (FormasPagamento.Any(f => string.Equals(f.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa forma de pagamento já existe.");
            return;
        }

        FormasPagamento.Add(new FormaPagamentoItem { Nome = nome });
        NovaFormaPagamento = string.Empty;
    }

    [RelayCommand]
    private void AlternarFormaPagamento(FormaPagamentoItem? forma)
    {
        if (forma != null)
            forma.Ativa = !forma.Ativa;
    }

    // ==============================================================
    // APOIO
    // ==============================================================

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;

        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }

    // Dados fictícios. Troque pela lista vinda do back-end depois.
    private static IEnumerable<UsuarioItem> CriarUsuarios()
    {
        var emailAtual = SessaoPerfil.EmailUsuario;

        UsuarioItem Criar(
            PerfilUsuario perfil,
            string nomeCurto,
            string nomeExibicao,
            string email,
            string rotulo,
            string documentos = "",
            bool ativo = true) => new()
            {
                Perfil = perfil,
                NomeCurto = nomeCurto,
                NomeExibicao = nomeExibicao,
                Email = email,
                RotuloPerfil = rotulo,
                Documentos = documentos,
                Ativo = ativo,
                EhContaAtual = string.Equals(email, emailAtual, StringComparison.OrdinalIgnoreCase)
            };

        var admin = SessaoPerfil.ContaPadrao(PerfilUsuario.Admin);
        var gestor = SessaoPerfil.ContaPadrao(PerfilUsuario.GestorRotas);
        var financeiro = SessaoPerfil.ContaPadrao(PerfilUsuario.Financeiro);
        var motorista = SessaoPerfil.ContaPadrao(PerfilUsuario.Motorista);

        yield return Criar(PerfilUsuario.Admin, admin.Nome, admin.Nome, admin.Email, "ADMINISTRADOR");

        yield return Criar(PerfilUsuario.GestorRotas, gestor.Nome, $"{gestor.Nome} (Gestor Frota)", gestor.Email, "GESTOR");

        yield return Criar(PerfilUsuario.Financeiro, financeiro.Nome, $"{financeiro.Nome} (Financeiro)", financeiro.Email, "FINANCEIRO");

        yield return Criar(
            PerfilUsuario.Motorista,
            motorista.Nome,
            $"{motorista.Nome} (Motorista)",
            motorista.Email,
            "OPERADOR",
            "CNH: 04928172611 (Cat E) • CPF: 123.456.789-00");

        yield return Criar(
            PerfilUsuario.Motorista,
            "Roberto Alves",
            "Roberto Alves (Inativo)",
            "bloqueado@inova.com.br",
            "OPERADOR",
            "CNH: 1122334455 (Cat D) • CPF: 999.888.777-66",
            ativo: false);
    }
}
