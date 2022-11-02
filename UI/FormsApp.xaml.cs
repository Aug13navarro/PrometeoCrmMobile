using Core;
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
        private readonly ApplicationData appData;

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
            this.appData = new ApplicationData();

            MessagingCenter.Subscribe<LoginViewModel, CultureInfo>(this, "LangChanged", (sender, currentCulture) =>
            {
                AppResources.Culture = currentCulture;
            });

            if (this.appData != null && this.appData.LoggedUser != null)
            {
                if (this.appData.LoggedUser.Language.abbreviation.ToLower().Contains("es"))
                {
                    var idiomas = CultureInfo.GetCultures(CultureTypes.NeutralCultures).Where(x => x.DisplayName.ToLower().Contains("español")).ToList();
                    var lang = new CultureInfo("es-ES");
                    AppResources.Culture = new CultureInfo("es-ES");


                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                }
                else
                {
                    AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("English"));
                }
            }
            else
            {
                AppResources.Culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault(element => element.EnglishName.Contains("English"));
            }

            Thread.CurrentThread.CurrentUICulture = AppResources.Culture;

            InitializeComponent();
        }
    }
}
