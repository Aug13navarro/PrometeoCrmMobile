using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Core
{
    public class App : MvxApplication 
    {
        private OfflineDataService offlineDataService;


        public override void Initialize()
        {
            offlineDataService = new OfflineDataService();

            RegisterServices();
            RegisterAppStart<RootViewModel>();

            //CreatableTypes()
            //    .EndingWith("Service")
            //    .AsInterfaces()
            //    .RegisterAsLazySingleton();

            // Sobrescribir el método OnStart con la instancia de AppStart
            //RegisterAppStart<AppStart>();

            //DataOffline();
        }
        //private void DataOffline()
        //{
        //    if(offlineDataService.IsWifiConection)
        //    {
        //        var dia = DateTime.Now.ToString("dddd", CultureInfo.CreateSpecificCulture("es"));

        //        if(dia == "martes")
        //        {
        //        }
        //    }
        //}

        private void RegisterServices()
        {
            var appData = new ApplicationData();

            Mvx.IoCProvider.RegisterType(() =>
            {
                var client = new HttpClient()
                {
                    BaseAddress = EndpointURL.PrometeoApiEndPoint
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
