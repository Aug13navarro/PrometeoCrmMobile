using AutoMapper;
using Core.Helpers;
using Core.Model;
using Core.Services;
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

        #region PROPIEDADES

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
            set
            {
                SetProperty(ref beginDate, value);
                VerificarMargenFecha();
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                SetProperty(ref endDate, value);
                VerificarMargenFecha();
            }
        }

        private bool VerificarMargenFecha()
        {
            var fechaLimite = BeginDate.AddMonths(+12);

            if (EndDate > fechaLimite)
            {
                if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
                {
                    Application.Current.MainPage.DisplayAlert("Atención", "Se puede aplicar el filtro hasta un año como maximo.", "Aceptar");
                    return false;
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Atention", "The filter can be applied up to a maximum of one year.", "Acept");
                    return false;
                }
            }
            else
            {
                return true;
            }
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
            set
            {
                SetProperty(ref company, value);
                CargarVendedores();
            }
        }

        private User seller;
        public User Seller
        {
            get => seller;
            set => SetProperty(ref seller, value);
        }

        private User sellerGuardado;
        public User SellerGuardado
        {
            get => sellerGuardado;
            set => SetProperty(ref sellerGuardado, value);
        }

        private bool isEnableSeller;
        public bool IsEnableSeller
        {
            get => isEnableSeller;
            set => SetProperty(ref isEnableSeller, value);
        }
        private Color enableColor;
        public Color EnableColor
        {
            get => enableColor;
            set => SetProperty(ref enableColor, value);
        }

        //private MvxObservableCollection<Company> companies;
        public MvxObservableCollection<Company> Companies { get; set; } = new MvxObservableCollection<Company>();
        public MvxObservableCollection<User> Vendors { get; set; } = new MvxObservableCollection<User>();

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

        #endregion


        //COMANDOS
        public Command ApplyFiltersCommand { get; }
        public Command LimpiarFiltroCommand { get; }
        public Command RestablecerFechaDesdeCommand { get; }
        public Command RestablecerFechaHastaCommand { get; }

        //SERIVICIO
        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IOfflineDataService offlineDataService;
        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(false);
        }


        public FilterOrdersViewModel()
        {
            try
            {
                data = new ApplicationData();

                MaximumDate = DateTime.Now.Date;


                this.navigationService = Mvx.Resolve<IMvxNavigationService>();
                this.prometeoApiService = Mvx.Resolve<IPrometeoApiService>();
                this.offlineDataService = Mvx.Resolve<IOfflineDataService>();

                ApplyFiltersCommand = new Command(async () => await ApplyFilters());
                LimpiarFiltroCommand = new Command(async () => await ClearFilter());
                RestablecerFechaDesdeCommand = new Command(async () => await ClearFechaDesde());
                RestablecerFechaHastaCommand = new Command(async () => await ClearFechaHasta());

                BeginDate = DateTime.Now.Date.AddMonths(-6);
                EndDate = DateTime.Now.Date;


                CargarEstados();
                CargarCompanies();

                IsEnableSeller = true;
                EnableColor = Color.White;

                VerificarRol(data.LoggedUser.RolesStr);
            }
            catch (Exception e)
            {
                Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar"); return;
            }
        }

        private void VerificarRol(string rolesJson)
        {
            var roles = JsonConvert.DeserializeObject<List<Role>>(rolesJson);

            foreach (var item in roles)
            {
                if (item.Name == "Vendedor")
                {
                    IsEnableSeller = false;
                    EnableColor = Color.LightGray;
                    break;
                }
            }
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
            Seller = null;

            return Task.FromResult(0);
        }
        private Task ClearFechaDesde()
        {
            BeginDate = DateTime.Now.Date.AddMonths(-6);
            return Task.FromResult(0);
        }
        private Task ClearFechaHasta()
        {
            EndDate = DateTime.Now.Date;
            return Task.FromResult(0);
        }

        private async void CargarVendedores()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if(red)
                {
                    if (Company != null)
                    {
                        var users = await prometeoApiService.GetUsersByRol(Company.Id, "vendedor");

                        if (users != null)
                        {
                            Vendors.Clear();
                            Vendors.AddRange(users);

                            if (SellerGuardado != null)
                            {
                                Seller = Vendors.FirstOrDefault(x => x.Id == SellerGuardado.Id);
                            }
                        }
                    }
                }
                else
                {
                    if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo cargar los vendedores, aseguresé de estar conectado a internet y vuelva a intentar.", "Aceptar"); return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attention", "Vendors could not be loaded, please make sure you are connected to the internet and try again.", "Acept"); return;
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar"); return;
            }
        }

        private void CargarEstados()
        {
            var user = data.LoggedUser;

            string lang = user.Language.abbreviation.ToLower();

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
                var red = await Connection.SeeConnection();

                if (red)
                {
                    var user = data.LoggedUser;

                    var d = await prometeoApiService.GetCompaniesByUserId(user.Id, user.Token);

                    Companies.AddRange(d);

                    CargarFiltroGuardado();
                }
                {
                    var mapperConfig = new MapperConfiguration(m =>
                    {
                        m.AddProfile(new MappingProfile());
                    });

                    IMapper mapper = mapperConfig.CreateMapper();

                    if (!offlineDataService.IsDataLoadedCompanies)
                    {
                        await offlineDataService.LoadCompanies();
                    }

                    var empresas = await offlineDataService.SearchCompanies();

                    var e = mapper.Map<List<Company>>(empresas);

                    Companies.AddRange(e);

                    CargarFiltroGuardado();
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"{e.Message}", "Aceptar"); return;
            }
        }

        private void CargarFiltroGuardado()
        {
            var filtroJson = data.InitialFilterOrder;

            if (filtroJson != null)
            {
                var filtro = JsonConvert.DeserializeObject<FilterOrderJson>(filtroJson);

                if (filtro.Seller != null) SellerGuardado = filtro.Seller;

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
            if (VerificarMargenFecha())
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
                if(Seller != null)
                {
                    filtro.userId = Seller.Id;
                }

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
                if (Seller != null) filtroJson.Seller = seller;
                if (Status != null) filtroJson.status = Status;
                if (filtroJson.priceFrom == 0) filtroJson.priceFrom = null;
                if (filtroJson.priceTo == 0) filtroJson.priceTo = null;

                var filtroString = JsonConvert.SerializeObject(filtroJson);

                data.FilterOrder(filtroString);

                await navigationService.Close(this, filtro);
            }
        }
    }
}
