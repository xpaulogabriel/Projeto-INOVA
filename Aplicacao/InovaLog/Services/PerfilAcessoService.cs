using Microsoft.Maui.Storage;

namespace InovaLog.Services;

/// <summary>
/// Controle LOCAL de perfil para demonstração da interface.
/// Não substitui autenticação nem autorização no servidor.
/// </summary>
public static class PerfilAcessoService
{
    private const string ChavePerfilAutenticado =
        "inovalog_demo_perfil_autenticado";

    private const string ChavePerfilAtivo =
        "inovalog_demo_perfil_ativo";

    private const string PerfilPadrao = "Administrador";

    private static string _perfilAutenticado =
        Preferences.Default.Get(
            ChavePerfilAutenticado,
            PerfilPadrao);

    private static string _perfilAtivo =
        Preferences.Default.Get(
            ChavePerfilAtivo,
            PerfilPadrao);

    public static event Action? PerfilAlterado;

    public static string PerfilAutenticado =>
        _perfilAutenticado;

    public static string PerfilAtivo =>
        _perfilAtivo;

    // O administrador pode alternar entre os perfis.
    public static bool PodeAlternarPerfil =>
        Normalizar(_perfilAutenticado) == "administrador";

    // Permissão para gerenciar utilizadores.
    public static bool PodeGerenciarUsuarios =>
        Normalizar(_perfilAtivo) == "administrador";

    // Permissão para acessar viagens.
    public static bool PodeAcessarViagens =>
        Normalizar(_perfilAtivo) is
            "administrador" or "gestor" or "motorista";

    // Permissão para acessar frota.
    public static bool PodeAcessarFrota =>
        Normalizar(_perfilAtivo) is
            "administrador" or "gestor";

    // Permissão para acessar financeiro.
    public static bool PodeAcessarFinanceiro =>
        Normalizar(_perfilAtivo) is
            "administrador" or "financeiro";

    public static string NomeUsuarioAtual =>
        Normalizar(_perfilAtivo) switch
        {
            "gestor" => "Pedro Santos",
            "financeiro" => "Juliana Mendes",
            "motorista" => "Carlos Silva",
            _ => "Maria Eduarda"
        };

    public static string PerfilCurto =>
        Normalizar(_perfilAtivo) switch
        {
            "gestor" => "GESTOR",
            "financeiro" => "FINANCEIRO",
            "motorista" => "MOTORISTA",
            _ => "ADMIN"
        };

    /// <summary>
    /// Altera somente o perfil exibido no Front-end.
    /// A sessão de origem precisa ser administrativa.
    /// </summary>
    public static bool TrocarPerfilAtivo(string perfil)
    {
        if (!PodeAlternarPerfil || !PerfilValido(perfil))
            return false;

        _perfilAtivo =
            NormalizarParaExibicao(perfil);

        Preferences.Default.Set(
            ChavePerfilAtivo,
            _perfilAtivo);

        PerfilAlterado?.Invoke();

        return true;
    }

    /// <summary>
    /// Preparado para integração futura com o perfil real
    /// retornado pela autenticação.
    /// </summary>
    public static void DefinirPerfilAutenticado(string perfil)
    {
        if (!PerfilValido(perfil))
            perfil = PerfilPadrao;

        _perfilAutenticado =
            NormalizarParaExibicao(perfil);

        _perfilAtivo = _perfilAutenticado;

        Preferences.Default.Set(
            ChavePerfilAutenticado,
            _perfilAutenticado);

        Preferences.Default.Set(
            ChavePerfilAtivo,
            _perfilAtivo);

        PerfilAlterado?.Invoke();
    }

    public static void LimparSessao()
    {
        Preferences.Default.Remove(
            ChavePerfilAutenticado);

        Preferences.Default.Remove(
            ChavePerfilAtivo);

        _perfilAutenticado = PerfilPadrao;
        _perfilAtivo = PerfilPadrao;

        PerfilAlterado?.Invoke();
    }

    private static bool PerfilValido(string? perfil)
    {
        return Normalizar(perfil) is
            "administrador" or
            "gestor" or
            "financeiro" or
            "motorista";
    }

    private static string NormalizarParaExibicao(string perfil)
    {
        return Normalizar(perfil) switch
        {
            "gestor" => "Gestor",
            "administrador" => "Administrador",
            "financeiro" => "Financeiro",
            _ => "Motorista"
        };
    }

    private static string Normalizar(string? valor)
    {
        return (valor ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
    }
}