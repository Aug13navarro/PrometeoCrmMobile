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
using Core.ViewModels.Model;
using Newtonsoft.Json;

namespace Core.ViewModels
{
    public class FilterOpportunitiesViewModel : MvxViewModelResult<FilterOportunityModel>
    {
        private DateTime maximumDate;
        public DateTime MaximumDate
        {
            get => maximumDate;
            set => SetProperty(ref maximumDate, value);
        }

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
        public Command SelectProductCommand { get; }
        public Command ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }

        //SERIVICIO
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public FilterOpportunitiesViewModel(OpportunitiesViewModel opportunitiesViewModel)
        {
            this.OpportunitiesViewModel = opportunitiesViewModel;
            //this.navigationService = Mvx.Resolve<IMvxNavigationService>();
            //this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
            //this.toastService = Mvx.Resolve<IToastService>();

            //SelectClientCommand = new Command(async () => await SelectClientAsync());

            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

            OpportunityStatuses = new ObservableCollection<OpportunityStatus>();
            
            CargarEstados();
        }

        public FilterOpportunitiesViewModel()
        {
            MaximumDate = DateTime.Now.Date;

            this.navigationService = Mvx.Resolve<IMvxNavigationService>();
            this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
            this.toastService = Mvx.Resolve<IToastService>();

            SelectClientCommand = new Command(async () => await SelectClientAsync());
            SelectProductCommand = new Command(async () => await SelectProdcutoAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFilters());

            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

            OpportunityStatuses = new ObservableCollection<OpportunityStatus>();

            CargarEstados();
        }

        public override Task Initialize()
        {
            return base.Initialize();
        }

        private async Task SelectClientAsync()
        {
            int customerId = await navigationService.Navigate<CustomersViewModel, int>();
            //int customerI = await popupnavigation
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

        private async Task SelectProdcutoAsync()
        {
            try
            {
                OpportunityProducts detail = await navigationService.Navigate<ProductsViewModel, OpportunityProducts>();

                if (detail != null)
                {
                    Product = detail.product;

                    //detail.product.Id = Opportunity.Details.Any() ? Opportunity.Details.Max(d => d.product.Id) + 1 : 1;
                    //Opportunity.Details.Add(detail);

                    //Total = Opportunity.ComputeTotal();
                }
            }
            catch (Exception e)
            {
                toastService.ShowError($"{e.Message}");
            }
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

        private async Task ApplyFilters()
        {
            var filtro = new FilterOportunityModel
            {
                dateFrom = this.BeginDate,
                dateTo = this.endDate,
                customers = new List<cust>(),
                status = new List<oppSta>(),
                products = new List<prod>(),
                priceFrom = TotalDesde,
                priceTo = TotalHasta
            };

            if (customer != null) filtro.customers.Add(new cust { id = customer.Id});
            if (Status != null) filtro.status.Add(new oppSta { id = Status.Id });
            if (Product != null) filtro.products.Add(new prod { id = Product.Id });
            if (filtro.priceFrom == 0) filtro.priceFrom = null;
            if (filtro.priceTo == 0) filtro.priceTo = null;

            await navigationService.Close(this, filtro);
        }

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