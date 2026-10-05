using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace InovaLog
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.ScreenSize |
                                     ConfigChanges.Orientation |
                                     ConfigChanges.UiMode |
                                     ConfigChanges.ScreenLayout |
                                     ConfigChanges.SmallestScreenSize |
                                     ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            WindowCompat.SetDecorFitsSystemWindows(Window!, false);

            base.OnCreate(savedInstanceState);

            Window!.SetStatusBarColor(Android.Graphics.Color.Transparent);
        }
    }
}