using Core.Model;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class FilterOrdersViewModel : MvxViewModelResult<FilterOrderModel>
    {
        private ApplicationData data;

        private DateTime minimumDate;
        public DateTime MinimumDate
        {
            get => minimumDate;
            set => SetProperty(ref minimumDate, value);
        }

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

        //private MvxObservableCollection<Company> companies;
        public MvxObservableCollection<Company> Companies { get; set; } = new MvxObservableCollection<Company>();

        private double totalDesde;
        public double TotalDesde
        {
            get => totalDesde;
            set => SetProperty(ref totalDesde, value);
        }
        private double totalHasta;
        public double TotalHasta
        {
            get => totalHasta;
            set => SetProperty(ref totalHasta, value);
        }

        public ObservableCollection<OpportunityStatus> OrderStatuses { get; set; } = new ObservableCollection<OpportunityStatus>();

        public PedidosViewModel PedidosViewModel{ get; set; }

        //COMANDOS
        public Command ApplyFiltersCommand { get; }
        public Command LimpiarFiltroCommand { get; }

        //SERIVICIO
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        //public FilterOrdersViewModel(PedidosViewModel pedidosViewModel)
        //{
        //    this.PedidosViewModel = pedidosViewModel;
        //    //this.navigationService = Mvx.Resolve<IMvxNavigationService>();
        //    //this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
        //    //this.toastService = Mvx.Resolve<IToastService>();

        //    //SelectClientCommand = new Command(async () => await SelectClientAsync());

        //    BeginDate = DateTime.Now.Date;
        //    EndDate = DateTime.Now.Date;

        //    //OpportunityStatuses = new ObservableCollection<OpportunityStatus>();

        //    //CargarEstados();
        //}

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(false);
        }


        public FilterOrdersViewModel()
        {
            data = new ApplicationData();

            MinimumDate = DateTime.Now.Date.AddMonths(-6);
            MaximumDate = DateTime.Now.Date;


            this.navigationService = Mvx.Resolve<IMvxNavigationService>();
            this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
            this.toastService = Mvx.Resolve<IToastService>();

            ApplyFiltersCommand = new Command(async () => await ApplyFilters());
            LimpiarFiltroCommand = new Command(async () => await ClearFilter());

            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

            //OpportunityStatuses = new ObservableCollection<OpportunityStatus>();

            CargarEstados();
            CargarCompanies();
            //CargarFiltroGuardado();
        }


        private Task ClearFilter()
        {
            BeginDate = DateTime.Now.Date.AddMonths(-6);
            EndDate = DateTime.Now.Date;
            Status = null;
            Company = null;
            IndexStatus = -1;
            TotalDesde = 0;
            TotalHasta = 0;

            return Task.FromResult(0);
        }
        
        private void CargarEstados()
        {
            var user = data.LoggedUser;

            string lang = user.Language.ToLower();

            if (lang == "es" || lang.Contains("spanish"))
            {
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 1,
                    name = "Pendiente"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 2,
                    name = "Aprobado"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 3,
                    name = "Rechazado"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 4,
                    name = "Remitado"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 5,
                    name = "Despachado"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 6,
                    name = "Entregado"
                });
            }
            else
            {
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 1,
                    name = "Pending"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 2,
                    name = "Approved"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 3,
                    name = "Rejected"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 4,
                    name = "Forwarded"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 5,
                    name = "Dispatched"
                });
                OrderStatuses.Add(new OpportunityStatus
                {
                    Id = 6,
                    name = "Delivered"
                });
            }                
        }

        private async void CargarCompanies()
        {
            try
            {
                var user = data.LoggedUser;

                var d = await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token);

                Companies.AddRange(d);

                CargarFiltroGuardado();

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("e", $"{e.Message}", "aceptar"); return;
            }
        }

        private void CargarFiltroGuardado()
        {
            var filtroJson = data.InitialFilterOrder;

            if (filtroJson != null)
            {
                var filtro = JsonConvert.DeserializeObject<FilterOrderJson>(filtroJson);

                BeginDate = filtro.dateFrom;
                EndDate = filtro.dateTo;

                if (filtro.company != null) Company = Companies.ToList().FirstOrDefault(x => x.Id == filtro.company.Id);
                if (filtro.status != null) Status = OrderStatuses.ToList().FirstOrDefault(x => x.Id == filtro.status.Id);
                if (filtro.priceFrom != null) TotalDesde = Convert.ToDouble(filtro.priceFrom);
                if (filtro.priceTo != null) TotalHasta = Convert.ToDouble(filtro.priceTo);
            }
        }

        private async Task ApplyFilters()
        {
            var filtro = new FilterOrderModel
            {
                dateFrom = this.BeginDate,
                dateTo = this.endDate,
                priceFrom = TotalDesde,
                priceTo = TotalHasta
            };


            if (Status != null)
            {
                filtro.orderStatusId = Status.Id;
            }
            else
            {
                filtro.orderStatusId = null;
            }
            if (Company != null)
            {
                filtro.companyId = Company.Id;
            }
            //else
            //{
            //    filtro.companyId = 0;
            //}
            if (filtro.priceFrom == 0) filtro.priceFrom = null;
            if (filtro.priceTo == 0) filtro.priceTo = null;

            var filtroJson = new FilterOrderJson
            {
                dateFrom = BeginDate,
                dateTo = EndDate,
                priceFrom = TotalDesde,
                priceTo = TotalHasta
            };

            if (Company != null) filtroJson.company = Company;
            if (Status != null) filtroJson.status = Status;
            if (filtroJson.priceFrom == 0) filtroJson.priceFrom = null;
            if (filtroJson.priceTo == 0) filtroJson.priceTo = null;

            var filtroString = JsonConvert.SerializeObject(filtroJson);

            data.FilterOrder(filtroString);

            await navigationService.Close(this, filtro);
        }
    }
}
