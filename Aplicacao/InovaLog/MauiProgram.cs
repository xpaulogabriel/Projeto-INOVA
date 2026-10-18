using CommunityToolkit.Maui;
using InovaLog.ViewModels;
using InovaLog.Views;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace InovaLog;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // StatusBarBehavior
            .UseSkiaSharp() // base do gráfico
            .UseLiveCharts() // gráfico LiveCharts
            .ConfigureFonts(fonts =>
            {
                // Open Sans
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                // Poppins
                fonts.AddFont("Poppins-Black.ttf", "PoppinsBlack");
                fonts.AddFont("Poppins-BlackItalic.ttf", "PoppinsBlackItalic");
                fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                fonts.AddFont("Poppins-BoldItalic.ttf", "PoppinsBoldItalic");
                fonts.AddFont("Poppins-ExtraBold.ttf", "PoppinsExtraBold");
                fonts.AddFont("Poppins-ExtraBoldItalic.ttf", "PoppinsExtraBoldItalic");
                fonts.AddFont("Poppins-ExtraLight.ttf", "PoppinsExtraLight");
                fonts.AddFont("Poppins-ExtraLightItalic.ttf", "PoppinsExtraLightItalic");
                fonts.AddFont("Poppins-Italic.ttf", "PoppinsItalic");
                fonts.AddFont("Poppins-Light.ttf", "PoppinsLight");
                fonts.AddFont("Poppins-LightItalic.ttf", "PoppinsLightItalic");
                fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                fonts.AddFont("Poppins-MediumItalic.ttf", "PoppinsMediumItalic");
                fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                fonts.AddFont("Poppins-SemiBoldItalic.ttf", "PoppinsSemiBoldItalic");
                fonts.AddFont("Poppins-Thin.ttf", "PoppinsThin");
                fonts.AddFont("Poppins-ThinItalic.ttf", "PoppinsThinItalic");
            });

        // Tira a linha embaixo dos campos de texto (Entry)
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SemLinha", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif WINDOWS
    var transparente = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);

    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
    handler.PlatformView.Resources["TextControlBorderBrush"] = transparente;
    handler.PlatformView.Resources["TextControlBorderBrushPointerOver"] = transparente;
    handler.PlatformView.Resources["TextControlBorderBrushFocused"] = transparente;
#endif
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