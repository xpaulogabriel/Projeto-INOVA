using System.Text.Json;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;

namespace InovaLog.Services;

public class AuthService
{
    private readonly Supabase.Client _supabaseClient;

    public AuthService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // Verifica se o aparelho tem conexão com a internet
    public bool TemInternet()
    {
        return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }

    // 1. Login Tradicional (E-mail e Senha)
    public async Task<(bool Sucesso, string Mensagem)> LoginComEmailAsync(string email, string senha)
    {
        if (!TemInternet())
            return (false, "Sem conexão com a internet para realizar o login.");

        try
        {
            var sessao = await _supabaseClient.Auth.SignIn(email, senha);

            if (sessao != null && !string.IsNullOrEmpty(sessao.AccessToken))
            {
                await SalvarSessaoLocalAsync(sessao);
                return (true, string.Empty);
            }

            return (false, "E-mail ou senha inválidos.");
        }
        catch (Exception)
        {
            return (false, "Credenciais incorretas ou servidor indisponível.");
        }
    }

    // 2. Login via Google (OAuth2 + Deep Link)
    public async Task<(bool Sucesso, string Mensagem)> LoginComGoogleAsync()
    {
        if (!TemInternet())
            return (false, "Conecte-se à internet para entrar com o Google.");

        try
        {
            var providerAuth = await _supabaseClient.Auth.SignIn(Provider.Google);

            var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                providerAuth.Uri,
                new Uri("inovalog://login-callback"));

            if (authResult.Properties.TryGetValue("access_token", out var accessToken) &&
                authResult.Properties.TryGetValue("refresh_token", out var refreshToken))
            {
                var sessao = await _supabaseClient.Auth.SetSession(accessToken, refreshToken);
                if (sessao != null)
                {
                    await SalvarSessaoLocalAsync(sessao);
                    return (true, string.Empty);
                }
            }

            return (false, "Não foi possível validar a permissão do Google.");
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

    // 3. Validação de Sessão Existente (Funciona Online e Offline)
    public async Task<bool> ValidarSessaoExistenteAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return false;

            // Verifica localmente se o Token JWT ainda está dentro do prazo de validade
            if (TokenJwtAindaValido(token))
            {
                var refreshToken = await SecureStorage.Default.GetAsync("refresh_token");
                if (TemInternet() && !string.IsNullOrEmpty(refreshToken))
                {
                    try { await _supabaseClient.Auth.SetSession(token, refreshToken); } catch { }
                }
                return true;
            }

            Logout();
            return false;
        }
        catch
        {
            return false;
        }
    }

    // Lê a data de expiração (exp) do JWT nativamente sem depender de bibliotecas externas
    private static bool TokenJwtAindaValido(string token)
    {
        try
        {
            var partes = token.Split('.');
            if (partes.Length != 3) return false;

            var payload = partes[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            using var doc = JsonDocument.Parse(jsonBytes);

            if (doc.RootElement.TryGetProperty("exp", out var expElement))
            {
                var expSeconds = expElement.GetInt64();
                var dataExpiracao = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                return DateTime.UtcNow < dataExpiracao;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static async Task SalvarSessaoLocalAsync(Session sessao)
    {
        if (!string.IsNullOrEmpty(sessao.AccessToken))
            await SecureStorage.Default.SetAsync("auth_token", sessao.AccessToken);

        if (!string.IsNullOrEmpty(sessao.RefreshToken))
            await SecureStorage.Default.SetAsync("refresh_token", sessao.RefreshToken);

        if (!string.IsNullOrEmpty(sessao.User?.Email))
            Preferences.Default.Set("user_email", sessao.User.Email);
    }

    public void Logout()
    {
        try { _supabaseClient.Auth.SignOut(); } catch { }
        SecureStorage.Default.Remove("auth_token");
        SecureStorage.Default.Remove("refresh_token");
        Preferences.Default.Remove("user_email");
    }
}