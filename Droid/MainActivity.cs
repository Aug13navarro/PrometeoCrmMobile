using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Core;
using Core.Services.Contracts;
using MvvmCross;
using MvvmCross.Forms.Platforms.Android.Core;
using MvvmCross.Forms.Platforms.Android.Views;
using PrometeoCrmMobile.Droid.Services;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using UI;
using UI.Popups;
using Xamarin.Forms;

namespace PrometeoCrmMobile.Droid
{
    [Activity(
        Label = "PrometeoCRM",
        Icon = "@drawable/icono",
        RoundIcon = "@drawable/ic_launcher",
        Theme = "@style/MainTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : MvxFormsAppCompatActivity<MvxFormsAndroidSetup<App, FormsApp>, App, FormsApp>
    {
        private void initFontScale()
        {
            Configuration configuration = Resources.Configuration;
            configuration.FontScale = (float)1;
            //0.85 small, 1 standard, 1.15 big，1.3 more bigger ，1.45 supper big 
            DisplayMetrics metrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(metrics);
            metrics.ScaledDensity = configuration.FontScale * metrics.Density;
            BaseContext.Resources.UpdateConfiguration(configuration, metrics);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {

                initFontScale();

                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

                base.OnCreate(savedInstanceState);

                Mvx.IoCProvider.RegisterType<IToastService>(() => new ToastDroidService());

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            }
            catch (System.Exception e)
            {
                var s = e.Message;
                throw;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override async void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                var popupPage = (BasePopupPage)PopupNavigation.Instance.PopupStack.Last();
                await PopupNavigation.Instance.PopAsync();

                popupPage.NotifyDismiss();
            }
        }
    }
}
