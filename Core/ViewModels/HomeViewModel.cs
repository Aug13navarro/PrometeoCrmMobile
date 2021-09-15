using System;
using System.Threading.Tasks;
using Core.Model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        // Properties
        public User LoggedUser { get; }

        // Commands
        public IMvxAsyncCommand GoToOpportunitiesCommand { get; }
        public IMvxAsyncCommand GoToOrderCommand { get; }
        public IMvxAsyncCommand GoToCustomersCommand { get; }
        public IMvxAsyncCommand GoToContactsCommand { get; }

        // Fields
        private readonly IMvxNavigationService navigationService;

        public HomeViewModel(IMvxNavigationService navigationService, ApplicationData appData)
        {
            this.navigationService = navigationService;

            LoggedUser = appData.LoggedUser;

            GoToOpportunitiesCommand = new MvxAsyncCommand(GoToOpportunitiesAsync);
            GoToOrderCommand = new MvxAsyncCommand(GoToOrderAsync);
            GoToCustomersCommand = new MvxAsyncCommand(GoToCustomersAsync);
            GoToContactsCommand = new MvxAsyncCommand(GoToContactsAsync);
        }

        private async Task GoToOrderAsync()
        {
            await navigationService.Navigate<PedidosViewModel>();
        }

        private async Task GoToCustomersAsync()
        {
            await navigationService.Navigate<CustomersViewModel>();
        }

        private async Task GoToContactsAsync()
        {
            await navigationService.Navigate<ContactsViewModel>();
        }

        private async Task GoToOpportunitiesAsync()
        {
            await navigationService.Navigate<OpportunitiesViewModel>();
        }
    }
}
