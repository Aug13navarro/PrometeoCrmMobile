using Core.ViewModels;
using System.Globalization;
using System.Linq;
using System.Threading;
using UI.LangResources;
using UI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace UI
{
    public partial class FormsApp : Application
    {
        public static RootPage RootPage
        {
            get
            {
                Application app = Current;
                return (RootPage)app.MainPage;
            }
        }

        public FormsApp()
        {
            MessagingCenter.Subscribe<LoginViewModel, CultureInfo>(this, "LangChanged", (sender, currentCulture) =>
            {
                AppResources.Culture = currentCulture;
            });

            if (CultureInfo.InstalledUICulture.EnglishName.Contains("English"))
            {
                AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("English"));
            }
            else
            {
                AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("Spanish"));
            }

            Thread.CurrentThread.CurrentUICulture = AppResources.Culture;

            InitializeComponent();
        }
    }
}
