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

namespace Core
{
    public class App : MvxApplication
    {
        public static Uri PrometeoApiEndPoint { get; } = new Uri("https://neophos-testing-api.azurewebsites.net/");

        public override void Initialize()
        {
            RegisterServices();
            RegisterAppStart<RootViewModel>();
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
