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
        private ApplicationData data;

        private DateTime maximumDate;
        public DateTime MaximumDate
        {
            get => maximumDate;
            set => SetProperty(ref maximumDate, value);
        }
        private DateTime minimumDate;
        public DateTime MinimumDate
        {
            get => minimumDate;
            set => SetProperty(ref minimumDate, value);
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

        private int indexStatus;
        public int IndexStatus
        {
            get => indexStatus;
            set => SetProperty(ref indexStatus, value);
        }

        private Company company;
        public Company Company
        {
            get => company;
            set => SetProperty(ref company, value);
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
        public ObservableCollection<Company> Companies { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<double> Totals { get; set; }

        public OpportunitiesViewModel OpportunitiesViewModel { get; set; }

        //COMANDOS
        public Command SelectClientCommand { get; }
        public Command SelectProductCommand { get; }
        public Command ApplyFiltersCommand { get; }
        public Command LimpiarFiltroCommand { get; }

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
            //Companies = new ObservableCollection<Company>();
            
            CargarEstados();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(false);
        }


        public FilterOpportunitiesViewModel()
        {
            data = new ApplicationData();

            MinimumDate = DateTime.Now.Date.AddMonths(-6);
            MaximumDate = DateTime.Now.Date;

            this.navigationService = Mvx.Resolve<IMvxNavigationService>();
            this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
            this.toastService = Mvx.Resolve<IToastService>();

            SelectClientCommand = new Command(async () => await SelectClientAsync());
            SelectProductCommand = new Command(async () => await SelectProdcutoAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFilters());
            LimpiarFiltroCommand = new Command(async () => await ClearFilter());

            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

            OpportunityStatuses = new ObservableCollection<OpportunityStatus>();
            Companies = new ObservableCollection<Company>();

            CargarEstados();
            CargarCompanies();

            CargarFiltroGuardado();
        }

        private async void CargarCompanies()
        {
            try
            {
                var user = data.LoggedUser;

                var d = await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token);

                foreach (var item in d)
                {
                    Companies.Add(item);
                }

                //CargarFiltroGuardado();

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        private Task ClearFilter()
        {
            BeginDate = DateTime.Now.Date.AddMonths(-6);
            EndDate = DateTime.Now.Date;
            IndexStatus = -1;
            Status = null;
            Customer = null;
            Product = null;
            Company = null;
            TotalDesde = 0;
            TotalHasta = 0;

            return Task.FromResult(0);
        }

        private void CargarFiltroGuardado()
        {
            var filtroJson = data.InitialFilter;
            if (filtroJson != null)
            {
                var filtro = JsonConvert.DeserializeObject<FilterOpportunityJson>(filtroJson);

                BeginDate = filtro.dateFrom;
                EndDate = filtro.dateTo;

                if(filtro.customers.Count() > 0)
                {
                    Customer = filtro.customers.FirstOrDefault();
                }
                if (filtro.status.Count() > 0)
                {
                    var estado = filtro.status.FirstOrDefault();

                    Status = OpportunityStatuses.FirstOrDefault(x => x.Id == estado.Id);
                    IndexStatus = Status.Id - 1;
                }
                if(filtro.companies.Count() > 0)
                {
                    Company = filtro.companies.FirstOrDefault();
                }
                else
                {
                    IndexStatus = -1;
                }
                if (filtro.products.Count() > 0)
                {
                    Product = filtro.products.FirstOrDefault();
                }

                if (filtro.priceFrom != null) TotalDesde = filtro.priceFrom.Value;
                if (filtro.priceTo != null) TotalHasta = filtro.priceTo.Value;
            }
        }

        public override Task Initialize()
        {
            return base.Initialize();
        }

        private async Task SelectClientAsync()
        {
            var customer = await navigationService.Navigate<CustomersViewModel, Customer>();
            //int customerI = await popupnavigation
            try
            {
                //IsLoading = true;
                Customer = customer;
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
                Product product = await navigationService.Navigate<SelectProductViewModel, Product>();

                if (product != null)
                {
                    Product = product;

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

        private async void CargarEstados()
        {
            try
            {
                var status = await prometeoApiService.GetOpportunityStatus(data.LoggedUser.Language.ToLower(), data.LoggedUser.Token);

                foreach (var item in status)
                {
                    OpportunityStatuses.Add(new OpportunityStatus
                    {
                        Id = item.Id,
                        name = item.name,
                    });
                }
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
            //OpportunityStatuses.Add(new OpportunityStatus
            //{
            //    Id = 1,
            //    name = "Análisis"
            //});
            //OpportunityStatuses.Add(new OpportunityStatus
            //{
            //    Id = 2,
            //    name = "Propuesta"
            //}); OpportunityStatuses.Add(new OpportunityStatus
            //{
            //    Id = 3,
            //    name = "Negociación"
            //}); OpportunityStatuses.Add(new OpportunityStatus
            //{
            //    Id = 4,
            //    name = "Cerrada Ganada"
            //}); OpportunityStatuses.Add(new OpportunityStatus
            //{
            //    Id = 5,
            //    name = "Cerrada Perdida"
            //});

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
                companies = new List<comp>(),
                priceFrom = TotalDesde,
                priceTo = TotalHasta
            };

            if (customer != null) filtro.customers.Add(new cust { id = customer.Id});
            if (Status != null) filtro.status.Add(new oppSta { id = Status.Id });
            if (Product != null) filtro.products.Add(new prod { id = Product.Id });
            if (Company != null) filtro.companies.Add(new comp { id = Company.Id });
            if (filtro.priceFrom == 0) filtro.priceFrom = null;
            if (filtro.priceTo == 0) filtro.priceTo = null;

            var filtroJson = new FilterOpportunityJson
            {
                dateFrom = this.BeginDate,
                dateTo = this.endDate,
                customers = new List<Customer>(),
                status = new List<OpportunityStatus>(),
                products = new List<Product>(),
                priceFrom = TotalDesde,
                priceTo = TotalHasta,
                companies = new List<Company>(),
            };

            if (customer != null) filtroJson.customers.Add(customer);
            if (Status != null) filtroJson.status.Add(Status);
            if (Product != null) filtroJson.products.Add(Product);
            if (Company != null) filtroJson.companies.Add(Company);
            if (filtroJson.priceFrom == 0) filtroJson.priceFrom = null;
            if (filtroJson.priceTo == 0) filtroJson.priceTo = null;

            var filtroString = JsonConvert.SerializeObject(filtroJson);

            data.FilterOpportunity(filtroString);

            await navigationService.Close(this, filtro);
        }


    }
}