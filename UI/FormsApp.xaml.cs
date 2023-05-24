using Core;
using Core.Notification;
using Core.ViewModels;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
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

        public INotificationManager notificationManager;

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
            notificationManager = DependencyService.Get<INotificationManager>();

            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
        }
        protected override void OnResume()
        {
            base.OnResume();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            CrossConnectivity.Current.ConnectivityChanged += HendleConnectivityChanged;
        }

        private async void HendleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                if (e.IsConnected)
                {
                    //ConnectionRed.SeeAlertConnection(e.IsConnected);

                    string title = $"Aviso de Conexión";
                    string message = $"'Prometeo Suite' se conectó a internet.";
                    notificationManager.SendNotification(title, message, 0, false);

                    //var s = new SincronizacionService();

                    //await s.Sincronizar();
                }
                else
                {
                    //ConnectionRed.SeeAlertConnection(e.IsConnected);

                    string title = $"Aviso de Conexión";
                    string message = $"'Prometeo Suite' perdió conexión a internet, seguira trabajando de manera Offline.";
                    notificationManager.SendNotification(title, message, 0, false);
                }
            }
            catch (Exception ex)
            {
                string title = $"Aviso de Conexión";
                string message = $"Sincronización interrumpida.";
                notificationManager.SendNotification(title, message, 10, true);

                await Application.Current.MainPage.DisplayAlert("Sincronizando", $"Ocurrio un error al sincronizar los dotos locales con el Servicio - {ex.Message}", "Aceptar"); return;
            }
        }
    }
}
