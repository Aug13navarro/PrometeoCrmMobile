using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Core.ViewModels
{
    public class CustomersViewModel : MvxViewModelResult<int>
    {
        // Properties
        private bool isSearchInProgress;
        public bool IsSearchInProgress
        {
            get => isSearchInProgress;
            private set => SetProperty(ref isSearchInProgress, value);
        }

        private bool error;
        public bool Error
        {
            get => error;
            private set => SetProperty(ref error, value);
        }

        private string clientsQuery;
        public string ClientsQuery
        {
            get => clientsQuery;
            set => SetProperty(ref clientsQuery, value);
        }

        public MvxObservableCollection<CustomerVm> Customers { get; } = new MvxObservableCollection<CustomerVm>();

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public IMvxAsyncCommand LoadMoreCustomersCommand { get; }
        public IMvxCommand<CustomerVm> ToggleContactsVisibilityCommand { get; }
        public IMvxAsyncCommand GoToCreateCustomerCommand { get; }
        public IMvxAsyncCommand NewClientsSearchCommand { get; }
        public IMvxAsyncCommand<CustomerVm> SelectCustomerCommand { get; }

        // Constants
        private const int PageSize = 10;

        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;

        public CustomersViewModel(IPrometeoApiService prometeoApiService, ApplicationData appData, IMvxNavigationService navigationService)
        {
            this.prometeoApiService = prometeoApiService;
            this.appData = appData;
            this.navigationService = navigationService;

            LoadMoreCustomersCommand = new MvxAsyncCommand(LoadMoreCustomersAsync);
            ToggleContactsVisibilityCommand = new MvxCommand<CustomerVm>(ToggleContactsVisibility);
            GoToCreateCustomerCommand = new MvxAsyncCommand(GoToCreateCustomerAsync);
            NewClientsSearchCommand = new MvxAsyncCommand(NewClientsSearchAsync);
            SelectCustomerCommand = new MvxAsyncCommand<CustomerVm>(SelectCustomerAsync);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new CustomersPaginatedRequest()
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                UserId = appData.LoggedUser.Id,
            };

            await SearchCustomersAsync(requestData);
        }

        private async Task SearchCustomersAsync(CustomersPaginatedRequest requestData, bool newSearch = false)
        {
            try
            {
                IsSearchInProgress = true;
                Error = false;

                var user = appData.LoggedUser;

                //PaginatedList<Customer> customers = await prometeoApiService.GetCustomers(requestData);

                //if (requestData.CurrentPage == 1)
                //{
                    var allCustomer = await prometeoApiService.GetAllCustomer(requestData.UserId, true, 3, user.Token);

                    Customers.Clear();

                    Customers.AddRange(allCustomer.Select(x => new CustomerVm() { Customer = x }));

                    isSearchInProgress = false;
                //}
                //else
                //{

                //    if (newSearch)
                //    {
                //        Customers.Clear();
                //    }

                //    Customers.AddRange(customers.Results.Select(c => new CustomerVm() { Customer = c }));

                //    CurrentPage = customers.CurrentPage;
                //    TotalPages = customers.TotalPages;

                //    IsSearchInProgress = false;
                //}
            }
            catch (Exception ex)
            {
                Error = true;
            }
            finally
            {
                IsSearchInProgress = false;
            }
        }

        private async Task LoadMoreCustomersAsync()
        {
            var requestData = new CustomersPaginatedRequest()
            {
                CurrentPage = CurrentPage + 1,
                PageSize = PageSize,
                UserId = appData.LoggedUser.Id,
                Query = ClientsQuery,
            };

            await SearchCustomersAsync(requestData);
        }

        private void ToggleContactsVisibility(CustomerVm customer)
        {
            customer.IsContactsVisible = !customer.IsContactsVisible;
        }

        private async Task GoToCreateCustomerAsync()
        {
            await navigationService.Navigate<CreateCustomerViewModel>();
        }

        private async Task NewClientsSearchAsync()
        {
            var requestData = new CustomersPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                UserId = appData.LoggedUser.Id,
                Query = ClientsQuery,
            };

            await SearchCustomersAsync(requestData, true);
        }

        private async Task SelectCustomerAsync(CustomerVm customer)
        {
            await navigationService.Close(this, customer.Customer.Id);
        }
    }
}
