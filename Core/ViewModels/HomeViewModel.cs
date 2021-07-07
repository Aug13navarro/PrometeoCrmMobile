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
        public IMvxAsyncCommand GoToCustomersCommand { get; }
        public IMvxAsyncCommand GoToContactsCommand { get; }
        public IMvxAsyncCommand GoToOpportunitiesCommand { get; }

        // Fields
        private readonly IMvxNavigationService navigationService;

        public HomeViewModel(IMvxNavigationService navigationService, ApplicationData appData)
        {
            this.navigationService = navigationService;

            LoggedUser = appData.LoggedUser;

            GoToCustomersCommand = new MvxAsyncCommand(GoToCustomersAsync);
            GoToContactsCommand = new MvxAsyncCommand(GoToContactsAsync);
            GoToOpportunitiesCommand = new MvxAsyncCommand(GoToOpportunitiesAsync);
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
