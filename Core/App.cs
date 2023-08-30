using Core.Data;
using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Core
{
    public class App : MvxApplication 
    {
        //private OfflineDataService offlineDataService;

        public override void Initialize()
        {
            //offlineDataService = new OfflineDataService();

            RegisterServices();
            RegisterAppStart<RootViewModel>();


          DataBaseHelper.CreateTables();
            //CreatableTypes()
            //    .EndingWith("Service")
            //    .AsInterfaces()
            //    .RegisterAsLazySingleton();

            // Sobrescribir el método OnStart con la instancia de AppStart
            //RegisterAppStart<AppStart>();

            // llamar a un método encargado de actualizar la data almacenada en offline una vez a la semana
        }

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
            Mvx.IoCProvider.ConstructAndRegisterSingleton<INotificationService, NotificationService>();

        }
    }
}
