using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Model;
using Core.Services.Contracts;
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
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IOfflineDataService offlineDataService;

        public HomeViewModel(IMvxNavigationService navigationService, ApplicationData appData, IPrometeoApiService _prometeoApiService, IOfflineDataService offlineData)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = _prometeoApiService;
            offlineDataService = offlineData;

            LoggedUser = appData.LoggedUser;

            GoToOpportunitiesCommand = new MvxAsyncCommand(GoToOpportunitiesAsync);
            GoToOrderCommand = new MvxAsyncCommand(GoToOrderAsync);
            GoToCustomersCommand = new MvxAsyncCommand(GoToCustomersAsync);
            GoToContactsCommand = new MvxAsyncCommand(GoToContactsAsync);

            CargarObtenerDatos();
        }

        private async void CargarObtenerDatos()
        {
            if (offlineDataService.IsWifiConection)
            {
                var empresas = await prometeoApiService.GetCompaniesByUserId(LoggedUser.Id, LoggedUser.Token);

                offlineDataService.UnloadAllData("Company");
                offlineDataService.UnloadAllData("Customer");
                offlineDataService.UnloadAllData("Presentation");

                offlineDataService.SaveCompanySearch(empresas);

                var clientes = new List<Customer>();
                foreach (var item in empresas)
                {
                    var allCustomer = await prometeoApiService.GetAllCustomer(LoggedUser.Id, true, 3, LoggedUser.Token, item.Id);

                    clientes.AddRange(allCustomer);
                }

                offlineDataService.SaveCustomerSearch(clientes);
            }
            else
            {
                await offlineDataService.LoadCompanies();
                await offlineDataService.LoadAllData();
            }

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
