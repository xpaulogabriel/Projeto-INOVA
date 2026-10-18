using Supabase;
using Microsoft.Maui.Storage;

namespace InovaLog.Services;

public class AuthService
{
    private readonly Client _supabaseClient;

    public AuthService(Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    // 1. Login Tradicional (E-mail e Senha)
    public async Task<bool> LoginComEmailAsync(string email, string senha)
    {
        try
        {
            var sessao = await _supabaseClient.Auth.SignIn(email, senha);

            if (sessao != null && sessao.User != null)
            {
                // Salva o token de acesso de forma criptografada no Android
                await SecureStorage.Default.SetAsync("auth_token", sessao.AccessToken);
                Preferences.Default.Set("is_logged_in", true);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            // Aqui você pode logar o erro (ex: credenciais inválidas)
            return false;
        }
    }

    // 2. Login Offline (Verificação de Sessão)
    public async Task<bool> VerificarSessaoOfflineAsync()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        var isLogged = Preferences.Default.Get("is_logged_in", false);

        // Se tem token salvo e a preferência diz que está logado, libera o acesso offline
        return !string.IsNullOrEmpty(token) && isLogged;
    }

    // 3. Logout Completo
    public void Logout()
    {
        _supabaseClient.Auth.SignOut();
        SecureStorage.Default.Remove("auth_token");
        Preferences.Default.Remove("is_logged_in");
    }
}