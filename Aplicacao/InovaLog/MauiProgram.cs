using Microsoft.Extensions.Logging;
using InovaLog.Views;
using InovaLog.ViewModels;

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
            });

        // 1. Registro de Serviços (Supabase e SQLite virão aqui depois)

        // 2. Registro de ViewModels (O Cérebro)
        builder.Services.AddTransient<LoginViewModel>();

        // 3. Registro de Views (As Telas)
        builder.Services.AddTransient<LoginPage>();

        // Registrando os ViewModels
        builder.Services.AddTransient<EsqueciSenhaViewModel>();
        builder.Services.AddTransient<InformacoesLoginViewModel>();

        // Registrando as Views
        builder.Services.AddTransient<EsqueciSenhaPage>();
        builder.Services.AddTransient<InformacoesLoginPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}