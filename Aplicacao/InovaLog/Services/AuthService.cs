using InovaLog.Models;
using Supabase.Gotrue;
using System.Globalization;
using static Postgrest.Constants;
using static Supabase.Gotrue.Constants;

namespace InovaLog.Services;

public class AuthService
{
    private readonly Supabase.Client _supabaseClient;

    // Tempo máximo que o motorista pode usar o app sem internet após o último login online
    private const int DiasValidadeOffline = 7;

    // Chaves do cofre seguro (SecureStorage)
    private const string KeyAccessToken = "auth_token";
    private const string KeyRefreshToken = "refresh_token";
    private const string KeyUserEmail = "user_email";
    private const string KeyUserName = "user_name";
    private const string KeyUserRole = "user_role";
    private const string KeyOfflineExpires = "offline_expires_at";

    public AuthService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public bool TemInternet()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    // =========================================================================
    // 1. LOGIN COM E-MAIL E SENHA (ONLINE)
    // =========================================================================
    public async Task<(bool Sucesso, string Mensagem)> LoginComEmailAsync(string email, string senha)
    {
        if (!TemInternet())
            return (false, "Sem conexão com a internet para realizar o login.");

        try
        {
            var sessao = await _supabaseClient.Auth.SignIn(email, senha);

            if (sessao == null || string.IsNullOrEmpty(sessao.AccessToken) || string.IsNullOrEmpty(sessao.User?.Email))
                return (false, "E-mail ou senha inválidos.");

            return await FinalizarLoginOnlineAsync(sessao);
        }
        catch (Exception)
        {
            return (false, "Credenciais incorretas ou servidor indisponível.");
        }
    }

    // =========================================================================
    // 2. LOGIN COM GOOGLE / GMAIL (ONLINE)
    // =========================================================================
    public async Task<(bool Sucesso, string Mensagem)> LoginComGoogleAsync()
    {
        if (!TemInternet())
            return (false, "Conecte-se à internet para entrar com o Google.");

        try
        {
            var providerAuth = await _supabaseClient.Auth.SignIn(
                Provider.Google,
                new SignInOptions
                {
                    RedirectTo = "inovalog://login-callback",
                    FlowType = OAuthFlowType.PKCE
                });

            var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                providerAuth.Uri,
                new Uri("inovalog://login-callback"));

            if (authResult.Properties.TryGetValue("error_description", out var descricaoErro))
            {
                authResult.Properties.TryGetValue("error_code", out var codigoErro);
                var textoLimpo = Uri.UnescapeDataString(descricaoErro).Replace("+", " ");
                return (false, $"Erro do Supabase ({codigoErro}): {textoLimpo}");
            }

            Session? sessao = null;

            // Fluxo PKCE (padrão mobile: retorna ?code=...)
            if (authResult.Properties.TryGetValue("code", out var code) &&
                !string.IsNullOrEmpty(providerAuth.PKCEVerifier))
            {
                sessao = await _supabaseClient.Auth.ExchangeCodeForSession(
                    providerAuth.PKCEVerifier,
                    code);
            }
            // Fallback para fluxo Implicit (retorna #access_token=...)
            else if (authResult.Properties.TryGetValue("access_token", out var accessToken) &&
                     authResult.Properties.TryGetValue("refresh_token", out var refreshToken))
            {
                sessao = await _supabaseClient.Auth.SetSession(accessToken, refreshToken);
            }

            if (sessao != null && !string.IsNullOrEmpty(sessao.User?.Email))
            {
                return await FinalizarLoginOnlineAsync(sessao);
            }

            var chavesRetornadas = string.Join(", ", authResult.Properties.Keys);
            return (false, $"Não foi possível validar a permissão do Google. Chaves recebidas: [{chavesRetornadas}]");
        }
        catch (TaskCanceledException)
        {
            return (false, "Login com Google cancelado.");
        }
        catch (Exception erro)
        {
            return (false, $"Falha ao autenticar com Google: {erro.Message}");
        }
    }

    // =========================================================================
    // 3. VALIDAÇÃO DE SESSÃO NO BOOT (ONLINE E OFFLINE COM TEMPO)
    // =========================================================================
    public async Task<bool> ValidarSessaoExistenteAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(KeyAccessToken);
            var refreshToken = await SecureStorage.Default.GetAsync(KeyRefreshToken);
            var email = await SecureStorage.Default.GetAsync(KeyUserEmail);
            var nome = await SecureStorage.Default.GetAsync(KeyUserName);
            var roleTexto = await SecureStorage.Default.GetAsync(KeyUserRole);
            var expiresTexto = await SecureStorage.Default.GetAsync(KeyOfflineExpires);

            // Se faltar qualquer dado essencial no cofre, exige novo login
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(roleTexto) || string.IsNullOrEmpty(expiresTexto))
            {
                return false;
            }

            // Verifica se o tempo de login offline expirou
            if (!DateTime.TryParse(expiresTexto, null, DateTimeStyles.RoundtripKind, out var dataLimiteOffline) ||
                DateTime.UtcNow > dataLimiteOffline)
            {
                Logout();
                return false;
            }

            // CENÁRIO A: TEM INTERNET -> Tenta renovar sessão e revalidar papel/status no Supabase
            if (TemInternet() && !string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    var sessaoRenovada = await _supabaseClient.Auth.SetSession(token, refreshToken);
                    if (sessaoRenovada != null)
                    {
                        var validacao = await FinalizarLoginOnlineAsync(sessaoRenovada);
                        if (validacao.Sucesso)
                            return true;

                        // Se o Admin desativou o usuário no banco, derruba o acesso
                        Logout();
                        return false;
                    }
                }
                catch
                {
                    // Se o servidor oscilar, cai para a validação offline abaixo
                }
            }

            // CENÁRIO B: SEM INTERNET (OU SERVIDOR INSTÁVEL) -> Libera usando os dados do cofre seguro
            var perfil = SessaoPerfil.ConverterPerfil(roleTexto);
            SessaoPerfil.Definir(perfil, nome ?? email, email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // =========================================================================
    // MÉTODOS INTERNOS: CONSULTA DE PAPEL NO SUPABASE + GRAVAÇÃO NO COFRE
    // =========================================================================
    private async Task<(bool Sucesso, string Mensagem)> FinalizarLoginOnlineAsync(Session sessao)
    {
        var emailAutenticado = sessao.User!.Email!.Trim().ToLowerInvariant();

        // 1. Busca o usuário na tabela 'usuarios'
        var respostaUsuario = await _supabaseClient
            .From<Usuario>()
            .Filter("email", Operator.ILike, emailAutenticado)
            .Get();

        var usuarioBanco = respostaUsuario.Models.FirstOrDefault();

        if (usuarioBanco == null)
        {
            Logout();
            return (false, "Este e-mail não está cadastrado no InovaLog. Solicite acesso ao administrador.");
        }

        // Ativo é bool?, então validamos se é explicitamente true
        if (usuarioBanco.Ativo != true)
        {
            Logout();
            return (false, "Seu acesso está desativado. Procure o administrador.");
        }

        // 2. Busca o nome do papel na tabela 'perfis' usando o PerfilId
        string? nomePerfilBanco = null;
        if (usuarioBanco.PerfilId.HasValue)
        {
            var respostaPerfil = await _supabaseClient
                .From<Perfil>()
                .Filter("id", Operator.Equals, usuarioBanco.PerfilId.Value)
                .Get();

            // Ajuste para .Nome se a propriedade na sua Model Perfil.cs se chamar Nome
            nomePerfilBanco = respostaPerfil.Models.FirstOrDefault()?.Nome;
        }

        var perfilEnum = SessaoPerfil.ConverterPerfil(nomePerfilBanco);
        var nomeUsuario = string.IsNullOrWhiteSpace(usuarioBanco.Nome)
            ? emailAutenticado
            : usuarioBanco.Nome;

        // 3. Hidrata o SessaoPerfil e grava no cofre seguro (SecureStorage)
        SessaoPerfil.Definir(perfilEnum, nomeUsuario, emailAutenticado);
        await SalvarSessaoLocalAsync(sessao, nomeUsuario, emailAutenticado, perfilEnum);

        return (true, string.Empty);
    }

    private static async Task SalvarSessaoLocalAsync(
        Session sessao,
        string nome,
        string email,
        PerfilUsuario perfil)
    {
        if (!string.IsNullOrEmpty(sessao.AccessToken))
            await SecureStorage.Default.SetAsync(KeyAccessToken, sessao.AccessToken);

        if (!string.IsNullOrEmpty(sessao.RefreshToken))
            await SecureStorage.Default.SetAsync(KeyRefreshToken, sessao.RefreshToken);

        await SecureStorage.Default.SetAsync(KeyUserEmail, email);
        await SecureStorage.Default.SetAsync(KeyUserName, nome);
        await SecureStorage.Default.SetAsync(KeyUserRole, perfil.ToString());

        // Grava a data limite de uso offline em formato ISO 8601 UTC
        var dataLimite = DateTime.UtcNow.AddDays(DiasValidadeOffline).ToString("O");
        await SecureStorage.Default.SetAsync(KeyOfflineExpires, dataLimite);
    }

    public void Logout()
    {
        try { _supabaseClient.Auth.SignOut(); } catch { }
        SecureStorage.Default.Remove(KeyAccessToken);
        SecureStorage.Default.Remove(KeyRefreshToken);
        SecureStorage.Default.Remove(KeyUserEmail);
        SecureStorage.Default.Remove(KeyUserName);
        SecureStorage.Default.Remove(KeyUserRole);
        SecureStorage.Default.Remove(KeyOfflineExpires);
        SessaoPerfil.Reiniciar();
    }
}