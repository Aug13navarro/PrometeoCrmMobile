using System;
using System.Threading.Tasks;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

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
                await navigationService.Navigate<MenuViewModel>();
                await navigationService.Navigate<LoginViewModel>();
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
