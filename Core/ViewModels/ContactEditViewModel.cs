using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Services.Contracts;
using Core.Services.Exceptions;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Core.ViewModels
{
    public class ContactEditViewModel : MvxViewModel<Customer>
    {
        // Properties
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private bool isCreatingFromNewClient;
        public bool IsCreatingFromNewClient
        {
            get => isCreatingFromNewClient;
            private set => SetProperty(ref isCreatingFromNewClient, value);
        }

        private Customer customer;
        public Customer Customer
        {
            get => customer;
            set => SetProperty(ref customer, value);
        }

        public CustomerContact NewContact { get; } = new CustomerContact();

        public string[] Provinces { get; } =
        {
            "Buenos Aires", "Catamarca", "Chaco", "Chubut", "Córdoba", "Corrientes", "Entre Ríos", "Formosa",
            "Jujuy", "La Pampa", "La Rioja", "Mendoza", "Misiones", "Neuquén", "Río Negro", "Salta", "San Juan",
            "San Luis", "Santa Cruz", "Santa Fé", "Santiago del Estero", "Tierra del Fuego", "Tucumán"
        };

        // Commands
        public Command SaveContactCommand { get; }
        public Command SelectClientCommand { get; }

        // Services
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public ContactEditViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService, IToastService toastService)
        {
            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            SaveContactCommand = new Command(async () => await SaveContactAsync());
            SelectClientCommand = new Command(async () => await SelectClientAsync());
        }

        public override void Prepare(Customer theCustomer)
        {
            Customer = theCustomer;
            IsCreatingFromNewClient = customer.Id == 0;
        }

        private async Task SaveContactAsync()
        {
            Customer.Contacts.Add(NewContact);

            try
            {
                IsLoading = true;

                if (!IsCreatingFromNewClient)
                {
                    foreach (TypeOfCustomer typeOfCustomer in Customer.CustomersTypes)
                    {
                        typeOfCustomer.Id = 0;
                        typeOfCustomer.CustomerType = null;
                    }

                    await prometeoApiService.UpdateCustomer(Customer);
                    toastService.ShowOk("Contacto guardado.");
                }

                await navigationService.Close(this);
            }
            catch (ServiceException ex)
            {
                toastService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                toastService.ShowError("Error guardando contacto. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SelectClientAsync()
        {
            var customer = await navigationService.Navigate<CustomersViewModel, Customer>();

            try
            {
                IsLoading = true;
                Customer = customer;
            }
            catch (Exception ex)
            {
                toastService.ShowError("Ocurrió un error al obtener el cliente. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
