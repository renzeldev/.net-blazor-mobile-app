using Android.App;
using Android.Content.PM;

namespace UmfaApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //protected override void AttachBaseContext(Context @base)
        //{
        //    Configuration configuration = new(@base.Resources.Configuration)
        //    {
        //        FontScale = 1.0f
        //    };
        //    ApplyOverrideConfiguration(configuration);
        //    base.AttachBaseContext(@base);
        //}
    }

}