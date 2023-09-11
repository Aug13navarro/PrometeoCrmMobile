using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Data;
using Core.Helpers;
using Core.Model;
using Core.Model.Common;
using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CustomersViewModel : MvxViewModel<DataExport,Customer>
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

        private bool whitExternal;
        public bool WhitExternal
        {
            get => this.whitExternal;
            set => SetProperty(ref whitExternal, value);
        }

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public IMvxAsyncCommand LoadMoreCustomersCommand { get; }
        public IMvxCommand<Customer> ToggleContactsVisibilityCommand { get; }
        public IMvxAsyncCommand GoToCreateCustomerCommand { get; }
        public IMvxAsyncCommand NewClientsSearchCommand { get; }
        public IMvxAsyncCommand<Customer> SelectCustomerCommand { get; }


        // Constants
        private const int PageSize = 30;

        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        //private readonly IOfflineDataService offlineDataService;

        IMapper mapper;

        public CustomersViewModel(IPrometeoApiService prometeoApiService, ApplicationData appData, IMvxNavigationService navigationService)//, IMapper mapper, IOfflineDataService offlineDataService
        {
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            this.prometeoApiService = prometeoApiService;
            this.appData = appData;
            this.navigationService = navigationService;
            //this.offlineDataService = offlineDataService;

            LoadMoreCustomersCommand = new MvxAsyncCommand(LoadMoreCustomersAsync);
            ToggleContactsVisibilityCommand = new MvxCommand<Customer>(ToggleContactsVisibility);
            GoToCreateCustomerCommand = new MvxAsyncCommand(GoToCreateCustomerAsync);
            NewClientsSearchCommand = new MvxAsyncCommand(NewClientsSearchAsync);
            SelectCustomerCommand = new MvxAsyncCommand<Customer>(SelectCustomerAsync);
        }

        public override void Prepare(DataExport parameter)
        {
            CompanyId = parameter.CompanyId;
            CustomerTypeId = parameter.CustomerTypeId;
            WhitExternal = parameter.WhitExternal;
        }

        public override async Task Initialize()
        {
            try
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
                    var customers = OfflineDatabase.GetCustomers();
                    if (customers != null)
                    {
                        var d = customers.Where(x => x.CompanyId == appData.LoggedUser.CompanyId).ToList();
                        d = d.Skip((CurrentPage - 1) * PageSize)
                            .Take(PageSize).ToList();
                        var r = mapper.Map<List<Customer>>(d);

                        Customers.Clear();
                        Customers.AddRange(r);
                    }
                }
            }
            catch(Exception e)
            {
                var m = e.Message;
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

                if (requestData.CurrentPage <= customers.TotalPages)
                {

                    if (customers.Results.Count() > 0)
                    {
                        if (newSearch)
                        {
                            Customers.Clear();
                        }

                        if (WhitExternal)
                        {
                            Customers.AddRange(customers.Results.Where(x => x.externalCustomerId != null));
                        }
                        else
                        {
                            Customers.AddRange(customers.Results);
                        }

                        CurrentPage = customers.CurrentPage;
                        TotalPages = customers.TotalPages;

                        IsSearchInProgress = false;
                    }
                    else
                    {
                        Customers.Clear();
                    }
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

                if(CustomerTypeId == 0) await SearchCustomersAsync(requestData);
            }
        }

        private void ToggleContactsVisibility(Customer customer)
        {
            customer.IsContactsVisible = !customer.IsContactsVisible;
        }

        private async Task GoToCreateCustomerAsync()
        {
            var createCustomerViewMode = MvxIoCProvider.Instance.IoCConstruct<CreateCustomerViewModel>();

            createCustomerViewMode.NewCustomerCreated += async (sender, arge) => await NewClientsSearchAsync();
            await navigationService.Navigate(createCustomerViewMode,new Customer());
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

                if (CustomerTypeId > 0)
                {
                    SearByType(CustomerTypeId);
                }
                else
                {
                    await SearchCustomersAsync(requestData, true);
                }
            }
        }

        private async Task SelectCustomerAsync(Customer customer)
        {
            await navigationService.Close(this, customer);
        }
    }
}
