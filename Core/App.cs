using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace Core
{
    public class App : MvxApplication 
    {
        private OfflineDataService offlineDataService;

        private OfflineCharging _offlineCharging;

        public static Uri PrometeoApiEndPoint { get; } = new Uri("https://neophos-testing-api.azurewebsites.net/");
        //public static Uri PrometeoApiEndPoint { get; } = new Uri("https://prometeo-produccion-api.azurewebsites.net/");

        public override void Initialize()
        {
            offlineDataService = new OfflineDataService();
            _offlineCharging = new OfflineCharging();

            RegisterServices();
            RegisterAppStart<RootViewModel>();
            DataOffline();
        }

        private void DataOffline()
        {
            if(offlineDataService.IsWifiConection)
            {
                var dia = DateTime.Now.ToString("dddd", CultureInfo.CreateSpecificCulture("es"));

                if(dia == "martes")
                {
                }
            }
        }

        private void RegisterServices()
        {
            var appData = new ApplicationData();

            Mvx.IoCProvider.RegisterType(() =>
            {
                var client = new HttpClient()
                {
                    BaseAddress = PrometeoApiEndPoint
                };

                if (!string.IsNullOrWhiteSpace(appData.LoggedUser?.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appData.LoggedUser.Token);
                }

                return client;
            });

            Mvx.IoCProvider.RegisterSingleton(() => appData);
            Mvx.IoCProvider.RegisterType<IPrometeoApiService, PrometeoApiService>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<IOfflineDataService, OfflineDataService>();
            Mvx.IoCProvider.ConstructAndRegisterSingleton<INotificationService, NotificationService>();
        }
    }
}
