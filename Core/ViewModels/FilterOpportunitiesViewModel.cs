using Core.Model;
using MvvmCross.ViewModels;
using MvvmCross.Navigation;
using System;
using Core.Utils;
using Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MvvmCross;

namespace Core.ViewModels
{
    public class FilterOpportunitiesViewModel : MvxViewModel
    {
        private DateTime beginDate;
        public DateTime BeginDate
        {
            get => beginDate;
            set => SetProperty(ref beginDate, value);
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value);
        }

        private OpportunityStatus status;
        public OpportunityStatus Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private Customer customer;
        public Customer Customer
        {
            get => customer;
            set => SetProperty(ref customer, value);
        }

        private Product product;
        public Product Product
        {
            get => product;
            set => SetProperty(ref product, value);
        }

        private decimal totalDesde;
        public decimal TotalDesde
        {
            get => totalDesde;
            set => SetProperty(ref totalDesde, value);
        }
        private decimal totalHasta;
        public decimal TotalHasta
        {
            get => totalHasta;
            set => SetProperty(ref totalHasta, value);
        }

        public ObservableCollection<OpportunityStatus> OpportunityStatuses { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<double> Totals { get; set; }

        public OpportunitiesViewModel OpportunitiesViewModel { get; set; }

        //COMANDOS
        public Command SelectClientCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        //SERIVICIO
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public FilterOpportunitiesViewModel(OpportunitiesViewModel opportunitiesViewModel)
        {
            this.OpportunitiesViewModel = opportunitiesViewModel;
            this.navigationService = Mvx.Resolve<IMvxNavigationService>();
            this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
            this.toastService = Mvx.Resolve<IToastService>();

            SelectClientCommand = new Command(async () => await SelectClientAsync());

            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

            OpportunityStatuses = new ObservableCollection<OpportunityStatus>();
            
            CargarEstados();
            //CargarCustomers();
            CargarProdcutos();
        }

        private async Task SelectClientAsync()
        {
            int customerId = await navigationService.Navigate<CustomersViewModel, int>();

            try
            {
                //IsLoading = true;
                Customer = await prometeoApiService.GetCustomer(customerId);
            }
            catch (Exception ex)
            {
                toastService.ShowError("Ocurrió un error al obtener el cliente. Compruebe su conexión a internet.");
            }
            finally
            {
                //IsLoading = false;
            }
        }

        //private void CargarCustomers()
        //{
        //    throw new NotImplementedException();
        //}

        private void CargarProdcutos()
        {

        }

        private void CargarEstados()
        {
            OpportunityStatuses.Add(new OpportunityStatus
            {
                Id = 1,
                name = "Análiss"
            });
            OpportunityStatuses.Add(new OpportunityStatus
            {
                Id = 2,
                name = "Propuesta"
            }); OpportunityStatuses.Add(new OpportunityStatus
            {
                Id = 3,
                name = "Negociación"
            }); OpportunityStatuses.Add(new OpportunityStatus
            {
                Id = 4,
                name = "Cerrada Ganada"
            }); OpportunityStatuses.Add(new OpportunityStatus
            {
                Id = 5,
                name = "Cerrada Perdida"
            });

        }

        //private void ApplyFilters()
        //{
        //    List<Opportunity> Opportunities = OpportunitiesViewModel.Opportunities.ToList();

        //    if (Customer != null)
        //    {
        //        Opportunities = Opportunities.Where(x => x.Customer?.Id == Customer.Id).ToList();
        //        //OpportunitiesViewModel = Customer;
        //    }

        //    if (Status != null)
        //    {
        //        Opportunities = Opportunities.Where(x => x.Status == Status).ToList();
        //        //  ShipmentViewModel.Destination = Destination;
        //    }

        //    //if (Ship != null)
        //    //{
        //    //    shipments = shipments.Where(x => x.Ship?.Id == Ship.Id).ToList();
        //    //    ShipmentViewModel.Ship = Ship;
        //    //}


        //    OpportunitiesViewModel.Opportunities.Clear();
        //    OpportunitiesViewModel.Opportunities.AddRange(Opportunities);

        //    MessagingCenter.Send(this, "filtered", OpportunitiesViewModel);
        //}

        //private void ResetFilters()
        //{
        //    ShipmentViewModel.Shipments.Clear();
        //    // ShipmentViewModel.Shipments.AddRange(ShipmentViewModel.ShipmentListBackUp);

        //    if (Preferences.Get("customerId", 0) > 0)
        //        ShipmentViewModel.PrepareDisplayData(ShipmentViewModel.ShipmentListBackUp);
        //    else
        //        ShipmentViewModel.Shipments.AddRange(ShipmentViewModel.ShipmentListBackUp);

        //    ShipmentViewModel.Customer = null;
        //    ShipmentViewModel.Destination = null;
        //    ShipmentViewModel.Ship = null;
        //    ShipmentViewModel.Product = null;
        //    ShipmentViewModel.Week = 0;
        //    ShipmentViewModel.ContainerNumber = null;

        //    Customer = null;
        //    Destination = null;
        //    week = 0;
        //    Ship = null;
        //    Product = null;

        //    //   MessagingCenter.Send(this, "filtered", ShipmentViewModel);
        //}

    }
}
