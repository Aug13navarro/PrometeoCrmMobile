using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using System;

namespace PrometeoCrmMobile.Droid
{
    [Activity(
        Label = "PrometeoCRM",
        Icon = "@drawable/icono",
        Theme = "@style/splashscreen",
        MainLauncher = true,
        NoHistory = true)]
        //ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        //LaunchMode = LaunchMode.SingleTask)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                StartActivity(typeof(MainActivity));
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
        }
    }
}
