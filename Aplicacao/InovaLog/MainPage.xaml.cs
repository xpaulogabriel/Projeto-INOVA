using System.Net.Http;
using System.Text.Json;

namespace InovaLog;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        InspecionarBancoDoBruno();
    }

    private async void InspecionarBancoDoBruno()
    {
        var urlBase = "https://lzrvywasypgmuembyaev.supabase.co"; // Use a SUPABASE_URL
        var anonKey = "sb_publishable_sdLo-P25G15MWXQf6sguaA_t2JcyG3A";         // Use a SUPABASE_PUBLISHABLE_KEY

        var endpoint = $"{urlBase}/rest/v1/";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("apikey", anonKey);
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {anonKey}");

        try
        {
            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var jsonBruto = await response.Content.ReadAsStringAsync();

                using var documento = JsonDocument.Parse(jsonBruto);
                var jsonFormatado = JsonSerializer.Serialize(documento, new JsonSerializerOptions { WriteIndented = true });

                // Aqui está a mágica: jogamos o JSON formatado direto na tela do celular!
                LblResultado.Text = jsonFormatado;
            }
            else
            {
                LblResultado.Text = $"Erro de Conexão: O Supabase recusou (Status {response.StatusCode})";
            }
        }
        catch (Exception ex)
        {
            LblResultado.Text = $"Falha Crítica: {ex.Message}";
        }
    }
}