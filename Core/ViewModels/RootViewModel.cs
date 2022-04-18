using System;
using System.Threading.Tasks;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class RootViewModel : MvxViewModel
    {
        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly INotificationService notificationService;

        public RootViewModel(IMvxNavigationService navigationService, ApplicationData appData, INotificationService notificationService)
        {
            this.navigationService = navigationService;
            this.appData = appData;
            this.notificationService = notificationService;
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            MvxNotifyTask.Create(async () => await InitializeViewModels());
        }

        private async Task InitializeViewModels()
        {
            if (appData.LoggedUser == null || string.IsNullOrWhiteSpace(appData.LoggedUser.Token))
            {
                /* LA LINEA N° 39 FUE COMENTADA YA QUE PRETENDIA ENTRAR EN EL MENU
                 * SI NO EXISTIA UN USUARIO REGISTRADO
                 */

                //await navigationService.Navigate<MenuViewModel>();
                await navigationService.Navigate<LoginViewModel>();

                var disp = Device.RuntimePlatform;

                if (Device.RuntimePlatform == "iOs")
                {
                    await navigationService.Navigate<MenuViewModel>();//se necesita para que ande en iOs, probar si va antes o despues de Login
                }
            }
            else
            {
                DateTime? tokenExpirationDate = appData.LoggedUser.Expiration;
                if (tokenExpirationDate.HasValue && DateTime.Now >= tokenExpirationDate.Value)
                {
                    appData.ClearLoggedUser();
                    //notificationService.StartListeningNotifications();

                    await navigationService.Navigate<MenuViewModel>();
                    await navigationService.Navigate<LoginViewModel>();
                }
                else
                {
                    //notificationService.StartListeningNotifications();

                    await navigationService.Navigate<MenuViewModel>();
                    await navigationService.Navigate<HomeViewModel>();
                }
            }
        }
    }
}
