using Android.App;
using Android.Support.V7.App;
using System;

namespace PrometeoCrmMobile.Droid
{
    [Activity(
        Label = "PrometeoCRM",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/splashscreen",
        MainLauncher = true,
        NoHistory = true)]
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
