using Microsoft.Extensions.Logging;
using InovaLog.Views;
using InovaLog.ViewModels;
using InovaLog.Services;

namespace InovaLog;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
            });

        // 1. Configuração do Supabase e Serviços
        var supabaseUrl = "https://projeto-placeholder.supabase.co"; // Trocaremos na Fase B
        var supabaseKey = "chave-placeholder"; // Trocaremos na Fase B

        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false
        };

        builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseKey, options));
        builder.Services.AddSingleton<AuthService>();

        // 2. Registo de ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EsqueciSenhaViewModel>();
        builder.Services.AddTransient<InformacoesLoginViewModel>();
        builder.Services.AddTransient<FinanceiroViewModel>();

        // 3. Registo de Views (Pré-Login)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EsqueciSenhaPage>();
        builder.Services.AddTransient<InformacoesLoginPage>();

        // 4. Registo de Views (Sistema Interno Pós-Login)
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<FinanceiroPage>();
        builder.Services.AddTransient<CadastroUsuarioPage>();
        builder.Services.AddTransient<NovaDespesaPag>();
        builder.Services.AddTransient<NovaRotaPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}