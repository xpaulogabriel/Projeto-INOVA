namespace InovaLog.Services;

/// <summary>Perfis de acesso do aplicativo.</summary>
public enum PerfilUsuario
{
    Motorista,
    GestorRotas,
    Financeiro,
    Admin
}

/// <summary>Módulos que aparecem na navegação inferior.</summary>
public enum ModuloApp
{
    Inicio,
    Viagens,
    Frota,
    Financeiro,
    Configuracoes
}

/// <summary>
/// Estado de sessão SOMENTE DO FRONT-END (simulação).
/// Guarda qual perfil está ativo e centraliza as regras de permissão em um lugar só.
/// Quando o back-end estiver pronto, o perfil real virá do login (Usuario.PerfilAcesso)
/// e as permissões devem ser validadas no servidor também.
/// </summary>
public static class SessaoPerfil
{
    public static PerfilUsuario PerfilAtual { get; private set; } = PerfilUsuario.Admin;
    public static string NomeUsuario { get; private set; } = "Maria Eduarda";
    public static string EmailUsuario { get; private set; } = "maria.eduarda@inova.com.br";

    /// <summary>Disparado sempre que o perfil/conta ativa muda.</summary>
    public static event Action? PerfilAlterado;

    // ==============================================================
    // TROCA DE PERFIL
    // ==============================================================

    public static void Definir(PerfilUsuario perfil, string nome, string email)
    {
        PerfilAtual = perfil;
        NomeUsuario = nome;
        EmailUsuario = email;
        PerfilAlterado?.Invoke();
    }

    /// <summary>Troca para a conta fictícia padrão do perfil escolhido.</summary>
    public static void DefinirPorPerfil(PerfilUsuario perfil)
    {
        var (nome, email) = ContaPadrao(perfil);
        Definir(perfil, nome, email);
    }

    /// <summary>Volta para o Admin (usado ao terminar a sessão).</summary>
    public static void Reiniciar() => DefinirPorPerfil(PerfilUsuario.Admin);

    /// <summary>Contas fictícias usadas só para visualização.</summary>
    public static (string Nome, string Email) ContaPadrao(PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Motorista => ("Carlos Silva", "carlos.motorista@inova.com.br"),
        PerfilUsuario.GestorRotas => ("Pedro Santos", "pedro.frota@inova.com.br"),
        PerfilUsuario.Financeiro => ("Juliana Mendes", "juliana.fin@inova.com.br"),
        _ => ("Maria Eduarda", "maria.eduarda@inova.com.br")
    };

    // ==============================================================
    // PERMISSÕES (ajuste aqui se as regras mudarem)
    // ==============================================================

    public static bool TemAcesso(ModuloApp modulo) => modulo switch
    {
        ModuloApp.Inicio => true,
        ModuloApp.Configuracoes => true,
        ModuloApp.Viagens => PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.GestorRotas or PerfilUsuario.Motorista,
        ModuloApp.Frota => PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.GestorRotas,
        ModuloApp.Financeiro => PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.Financeiro,
        _ => false
    };

    /// <summary>Card "Últimas Atividades" (ciclos de viagem e custo/km).</summary>
    public static bool PodeVerAtividades() =>
        PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.GestorRotas or PerfilUsuario.Financeiro;

    /// <summary>Top 3 rotas lucrativas e meta mensal de fretes.</summary>
    public static bool PodeVerFinanceiroNaHome() =>
        PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.Financeiro;

    /// <summary>Aba de usuários em Configurações.</summary>
    public static bool PodeGerenciarUsuarios() => PerfilAtual == PerfilUsuario.Admin;

    /// <summary>Abas de categorias e formas de pagamento em Configurações.</summary>
    public static bool PodeGerenciarCadastrosFinanceiros() =>
        PerfilAtual is PerfilUsuario.Admin or PerfilUsuario.Financeiro;

    // ==============================================================
    // APOIO
    // ==============================================================

    /// <summary>Rotas do AppShell.xaml (devem ser iguais ao atributo Route de cada aba).</summary>
    public static string RotaDe(ModuloApp modulo) => modulo switch
    {
        ModuloApp.Viagens => "ViagensPage",
        ModuloApp.Frota => "FrotaPage",
        ModuloApp.Financeiro => "FinanceiroPage",
        ModuloApp.Configuracoes => "ConfiguracoesPage",
        _ => "HomePage"
    };

    public static string RotuloPerfil(PerfilUsuario perfil) => perfil switch
    {
        PerfilUsuario.Motorista => "MOTORISTA",
        PerfilUsuario.GestorRotas => "GESTOR DE ROTAS",
        PerfilUsuario.Financeiro => "FINANCEIRO",
        _ => "ADMIN"
    };

    /// <summary>Duas primeiras iniciais do nome (usado no lugar da foto de perfil).</summary>
    public static string Iniciais(string nome)
    {
        var partes = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (partes.Length == 0)
            return "?";

        if (partes.Length == 1)
            return partes[0][..1].ToUpperInvariant();

        return (partes[0][..1] + partes[1][..1]).ToUpperInvariant();
    }
}
