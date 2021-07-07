using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Model;
using Core.Services.Contracts;
using Core.Services.Exceptions;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class CreateCustomerViewModel : MvxViewModel
    {
        // Properties
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private List<CustomerType> customerTypes;
        public List<CustomerType> CustomerTypes
        {
            get => customerTypes;
            private set => SetProperty(ref customerTypes, value);
        }

        private List<DocumentType> documentTypes;
        public List<DocumentType> DocumentTypes
        {
            get => documentTypes;
            private set => SetProperty(ref documentTypes, value);
        }

        private List<Customer> corporativeCustomers;
        public List<Customer> CorporativeCustomers
        {
            get => corporativeCustomers;
            private set => SetProperty(ref corporativeCustomers, value);
        }

        private List<TaxCondition> taxConditions;
        public List<TaxCondition> TaxConditions
        {
            get => taxConditions;
            private set => SetProperty(ref taxConditions, value);
        }

        private List<Company> companies;
        public List<Company> Companies
        {
            get => companies;
            private set => SetProperty(ref companies, value);
        }

        private CustomerType selectedCustomerType;
        public CustomerType SelectedCustomerType
        {
            get => selectedCustomerType;
            set => SetProperty(ref selectedCustomerType, value);
        }

        public Customer NewCustomer { get; } = new Customer();
        public string AccountOwner { get; }
        public DocumentType SelectedDocumentType { get; set; }
        public Customer SelectedCorporativeCustomer { get; set; }
        public TaxCondition SelectedTaxCondition { get; set; }
        public Company SelectedCompany { get; set; }
        public MvxObservableCollection<CustomerType> SelectedCustomerTypes { get; } = new MvxObservableCollection<CustomerType>();

        // Commands
        public IMvxAsyncCommand SaveCustomerCommand { get; }
        public IMvxAsyncCommand AddAddressCommand { get; }
        public IMvxCommand<CustomerAddress> RemoveAddressCommand { get; }
        public IMvxAsyncCommand AddContactCommand { get; }
        public IMvxCommand<CustomerContact> RemoveContactCommand { get; }
        public ICommand AddCustomerTypeCommand { get; }
        public ICommand RemoveCustomerTypeCommand { get; }

        // Fields
        private readonly ApplicationData appData;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;
        private readonly IMvxNavigationService navigationService;

        public CreateCustomerViewModel(IPrometeoApiService prometeoApiService,
                                       IToastService toastService,
                                       ApplicationData appData,
                                       IMvxNavigationService navigationService)
        {
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;
            this.appData = appData;
            this.navigationService = navigationService;

            SaveCustomerCommand = new MvxAsyncCommand(SaveCustomerAsync);
            AddAddressCommand = new MvxAsyncCommand(AddAddressAsync);
            RemoveAddressCommand = new MvxCommand<CustomerAddress>(RemoveAddress);
            AddContactCommand = new MvxAsyncCommand(AddContactAsync);
            RemoveContactCommand = new MvxCommand<CustomerContact>(RemoveContact);
            AddCustomerTypeCommand = new Command(AddCustomerType);
            RemoveCustomerTypeCommand = new Command<CustomerType>(RemoveCustomerType);

            AccountOwner = appData.LoggedUser.FullName;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            try
            {
                IsLoading = true;

                Task<List<CustomerType>> customerTypesTask = prometeoApiService.GetCustomerTypes();
                Task<List<DocumentType>> documentTypesTask = prometeoApiService.GetDocumentTypes();
                Task<List<TaxCondition>> taxConditionsTask = prometeoApiService.GetTaxConditions();
                Task<List<Customer>> corporativeCustomersTask = prometeoApiService.GetCustomersOld(new CustomersOldRequest()
                {
                    UserId = appData.LoggedUser.Id,
                    CompanyId = 0,
                    IsParentCustomer = true,
                });
                Task<List<Company>> companiesTask = prometeoApiService.GetCompaniesByUserId(appData.LoggedUser.Id);

                await Task.WhenAll(customerTypesTask, documentTypesTask, corporativeCustomersTask, taxConditionsTask, companiesTask);

                CustomerTypes = await customerTypesTask;
                DocumentTypes = await documentTypesTask;
                CorporativeCustomers = await corporativeCustomersTask;
                TaxConditions = await taxConditionsTask;
                Companies = await companiesTask;
            }
            catch (Exception ex)
            {
                toastService.ShowError("Error inicializando datos. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveCustomerAsync()
        {
            try
            {
                IsLoading = true;

                NewCustomer.AccountOwnerId = appData.LoggedUser.Id;
                NewCustomer.TypeId = SelectedDocumentType?.Id;
                NewCustomer.IdParentCustomer = SelectedCorporativeCustomer?.Id;
                NewCustomer.TaxCondition = SelectedTaxCondition?.Id;

                if (SelectedCompany != null)
                {
                    NewCustomer.CompanyUserId.Add(SelectedCompany.Id);
                }

                await prometeoApiService.CreateCustomer(NewCustomer);
                toastService.ShowOk("Cliente guardado.");

                await navigationService.Close(this);
            }
            catch (ServiceException ex)
            {
                toastService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                toastService.ShowError("Error guardando cliente. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddAddressAsync()
        {
            await navigationService.Navigate<AddressEditViewModel, Customer>(NewCustomer);
        }

        private void RemoveAddress(CustomerAddress address)
        {
            NewCustomer.Addresses.Remove(address);
        }

        private async Task AddContactAsync()
        {
            await navigationService.Navigate<ContactEditViewModel, Customer>(NewCustomer);
        }

        private void RemoveContact(CustomerContact contact)
        {
            NewCustomer.Contacts.Remove(contact);
        }

        private void AddCustomerType()
        {
            if (SelectedCustomerType != null)
            {
                TypeOfCustomer typeOfCustomer = NewCustomer.CustomersTypes.SingleOrDefault(t => t.CustomerTypeId == SelectedCustomerType.Id);
                if (typeOfCustomer == null)
                {
                    NewCustomer.CustomersTypes.Add(new TypeOfCustomer()
                    {
                        CustomerTypeId = SelectedCustomerType.Id,
                    });

                    SelectedCustomerTypes.Add(SelectedCustomerType);
                    SelectedCustomerType = null;
                }
            }
        }

        private void RemoveCustomerType(CustomerType customerType)
        {
            TypeOfCustomer typeOfCustomer = NewCustomer.CustomersTypes.Single(t => t.CustomerTypeId == customerType.Id);
            NewCustomer.CustomersTypes.Remove(typeOfCustomer);

            SelectedCustomerTypes.Remove(customerType);
        }
    }
}
