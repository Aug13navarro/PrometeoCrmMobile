using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Helpers;
using Core.Model;
using Core.Model.Common;
using Core.Services;
using Core.Services.Contracts;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CustomersViewModel : MvxViewModelResult<Customer>
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

        public MvxObservableCollection<Customer> Customers { get; } = new MvxObservableCollection<Customer>();

        private int customerTypeId;
        public int CustomerTypeId
        {
            get => customerTypeId;
            set => SetProperty(ref customerTypeId, value);
        }

        private int companyId;
        public int CompanyId
        {
            get => companyId;
            set => SetProperty(ref companyId, value);
        }

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public IMvxAsyncCommand LoadMoreCustomersCommand { get; }
        public IMvxCommand<Customer> ToggleContactsVisibilityCommand { get; }
        public IMvxAsyncCommand GoToCreateCustomerCommand { get; }
        public IMvxAsyncCommand NewClientsSearchCommand { get; }
        public IMvxAsyncCommand<Customer> SelectCustomerCommand { get; }

        //private readonly IMapper mapper;

        // Constants
        private const int PageSize = 10;

        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        private readonly IOfflineDataService offlineDataService;

        public CustomersViewModel(IPrometeoApiService prometeoApiService, ApplicationData appData, IMvxNavigationService navigationService, IOfflineDataService offlineDataService)//, IMapper mapper
        {
            this.prometeoApiService = prometeoApiService;
            this.appData = appData;
            this.navigationService = navigationService;
            this.offlineDataService = offlineDataService;

            //mapper = Mvx.Resolve<IMapper>();
            //_mapper = mapper;

            LoadMoreCustomersCommand = new MvxAsyncCommand(LoadMoreCustomersAsync);
            ToggleContactsVisibilityCommand = new MvxCommand<Customer>(ToggleContactsVisibility);
            GoToCreateCustomerCommand = new MvxAsyncCommand(GoToCreateCustomerAsync);
            NewClientsSearchCommand = new MvxAsyncCommand(NewClientsSearchAsync);
            SelectCustomerCommand = new MvxAsyncCommand<Customer>(SelectCustomerAsync);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
             
            var red = await Connection.SeeConnection();

            if (red)
            {
                if (CustomerTypeId > 0)
                {
                    SearByType(CustomerTypeId);
                }
                else
                {

                    var requestData = new CustomersPaginatedRequest()
                    {
                        CurrentPage = CurrentPage,
                        PageSize = PageSize,
                        UserId = appData.LoggedUser.Id,
                    };

                    await SearchCustomersAsync(requestData);
                }
            }
            else
            {

                var mapperConfig = new MapperConfiguration(m =>
                {
                    m.AddProfile(new MappingProfile());
                });

                IMapper mapper = mapperConfig.CreateMapper();

                if (!offlineDataService.IsDataLoadedCustomer)
                {
                    await offlineDataService.LoadAllData();
                }

                var d = await offlineDataService.SearchCustomers();

                var r = mapper.Map<List<Customer>>(d);

                Customers.Clear();
                Customers.AddRange(r);
            }
        }

        private async void SearByType(int customerType)
        {
            try
            {
                var customers = await prometeoApiService.GetAllCustomer(appData.LoggedUser.Id, true, customerType, appData.LoggedUser.Token, CompanyId);

                if(customers.Count > 0)
                {
                    Customers.Clear();
                    Customers.AddRange(customers);
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("", $"{e.Message}", "Aceptar"); return;
            }
        }

        private async Task SearchCustomersAsync(CustomersPaginatedRequest requestData, bool newSearch = false)
        {
            try
            {
                IsSearchInProgress = true;
                Error = false;

                var user = appData.LoggedUser;

                PaginatedList<Customer> customers = await prometeoApiService.GetCustomers(requestData);

                if (customers.Results.Count() > 0)
                {
                    if (requestData.CurrentPage == 1)
                    {
                        //var allCustomer = await prometeoApiService.GetAllCustomer(requestData.UserId, true, 3, user.Token);

                        Customers.Clear();

                        Customers.AddRange(customers.Results);

                        IsSearchInProgress = false;
                    }
                    else
                    {

                        if (newSearch)
                        {
                            Customers.Clear();
                        }

                        Customers.AddRange(customers.Results);

                        CurrentPage = customers.CurrentPage;
                        TotalPages = customers.TotalPages;

                        IsSearchInProgress = false;
                    }
                }
                else
                {
                    Customers.Clear();
                }

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
            var red = await Connection.SeeConnection();

            if (red)
            {
                var requestData = new CustomersPaginatedRequest()
                {
                    CurrentPage = CurrentPage + 1,
                    PageSize = PageSize,
                    UserId = appData.LoggedUser.Id,
                    Query = ClientsQuery,
                };

                if(customerTypeId > 0) await SearchCustomersAsync(requestData);
            }
        }

        private void ToggleContactsVisibility(Customer customer)
        {
            customer.IsContactsVisible = !customer.IsContactsVisible;
        }

        private async Task GoToCreateCustomerAsync()
        {
            await navigationService.Navigate<CreateCustomerViewModel>();
        }

        private async Task NewClientsSearchAsync()
        {
            var red = await Connection.SeeConnection();

            if (red)
            {
                var requestData = new CustomersPaginatedRequest()
                {
                    CurrentPage = 1,
                    PageSize = PageSize,
                    UserId = appData.LoggedUser.Id,
                    Query = ClientsQuery,
                };

                if (customerTypeId > 0)
                {
                    await SearchCustomersAsync(requestData, true);
                }
                else
                {
                    SearByType(CustomerTypeId);
                }
            }
        }

        private async Task SelectCustomerAsync(Customer customer)
        {
            await navigationService.Close(this, customer);
        }
    }
}
