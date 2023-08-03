using AutoMapper;
using Core.Data;
using Core.Helpers;
using Core.Model;
using Core.Services;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.App.Assist.AssistStructure;

namespace Core.ViewModels
{
    public class PedidosViewModel : MvxViewModel
    {
        private readonly ApplicationData data;
        
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private string query;
        public string Query
        {
            get => query;
            set
            {
                SetProperty(ref query, value);
                if(string.IsNullOrWhiteSpace(query))
                {
                    var requestData = new OrdersNotesPaginatedRequest()
                    {
                        CurrentPage = 1,
                        PageSize = 20,
                    };

                    GetOrdersNoteAsync(requestData, true);
                }
            }
        }

        private decimal total;
        public decimal Total
        {
            get => OrdersNote.Sum( x => x.total);
            set
            {
                SetProperty(ref total, value);
                ConvertirTotalStr(this.total);
            }
        }
        private string totalOfOrderStr;
        public string TotalOfOrderStr
        {
            get => totalOfOrderStr;
            set
            {
                SetProperty(ref totalOfOrderStr, value);
            }
        }

        private void ConvertirTotalStr(decimal total)
        {
            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                TotalOfOrderStr = Total.ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                TotalOfOrderStr = Total.ToString("N2", new CultureInfo("en-US"));
            }

            var s = TotalOfOrderStr;
        }

        private string fechaInicioFiltro;
        public string FechaInicioFiltro
        {
            get => fechaInicioFiltro;
            set => SetProperty(ref fechaInicioFiltro, value);
        }

        private string fechaFinFiltro;
        public string FechaFinFiltro
        {
            get => fechaFinFiltro;
            set => SetProperty(ref fechaFinFiltro, value);
        }

        public MvxObservableCollection<OrderNote> OrdersNote { get; set; } = new MvxObservableCollection<OrderNote>();

        public int CurrentPage { get; private set; } = 1; 
        private const int PageSize = 40;

        public Command NuevaNotaPedidoCommand { get; }
        public Command FilterOrdersCommand { get; }
        public Command OpenOrderNoteCommand { get; }
        public Command SearchQueryCommand { get; }
        public Command RefreshListCommand { get; }

        //EVENTS
        public event EventHandler<Company> NewOrderPopup;


        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        //private readonly IOfflineDataService offlineDataService;

        IMapper mapper;

        public PedidosViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService)//,IToastService toastService, IOfflineDataService offlineDataService
        {
            data = new ApplicationData();
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            //this.offlineDataService = offlineDataService;

            NuevaNotaPedidoCommand = new Command(async () => await NuevaNotaPedido());
            FilterOrdersCommand = new Command(async () => await FilterOrders());
            OpenOrderNoteCommand = new Command<OrderNote>(async o => await AbrirNota(o));
            SearchQueryCommand = new Command(async () => await SearchQuery());
            RefreshListCommand = new Command(async () => await RefreshList());

            OrdersNote.CollectionChanged += (sender, arg) =>
            {
                Total = OrdersNote.Sum(x => x.total);
            };

            if (data.LoggedUser.Language.abbreviation.ToLower() == "es" || data.LoggedUser.Language.abbreviation.Contains("spanish"))
            {
                FechaInicioFiltro = DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy");
                FechaFinFiltro = DateTime.Now.ToString("dd/MM/yyyy");
            }
            else
            {
                FechaInicioFiltro = DateTime.Now.AddMonths(-6).ToString("MM/dd/yyyy");
                FechaFinFiltro = DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        private async Task RefreshList()
        {
            var requestData = new OrdersNotesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                query = Query
            };

            await GetOrdersNoteAsync(requestData, true);

        }

        private async Task SearchQuery()
        {
            var red = await Connection.SeeConnection();

            if (red)
            {
                var requestData = new OrdersNotesPaginatedRequest()
                {
                    CurrentPage = 1,
                    PageSize = 30,
                    query = Query
                };


                await GetOrdersNoteAsync(requestData, true);
            }
            else
            {
                var d = OfflineDatabase.GetOrderNotes();

                var orderNotes = new MvxObservableCollection<OrderNote>(mapper.Map<List<OrderNote>>(d));

                OrdersNote.Clear();

                OrdersNote.AddRange(orderNotes.Where(x => x.customer.CompanyName.ToLower().Contains(Query.ToLower())).ToList());

            }
        }

        private async Task NuevaNotaPedido()
        {
            try
            {
                var red = await Connection.SeeConnection();

                if (red)
                {
                    var empresas = await prometeoApiService.GetCompaniesByUserId(data.LoggedUser.Id, data.LoggedUser.Token);

                    var company = empresas.FirstOrDefault(x => x.Id == data.LoggedUser.CompanyId.Value);

                    if (company.ExportPv.HasValue)
                    {
                        NewOrderPopup?.Invoke(this, company);
                    }
                    else
                    {
                        await IrNuevaNotaPedido(company, false);
                    }
                }
                else
                {
                    var empresas = OfflineDatabase.GetCompanies();

                    var company = mapper.Map<Company>(empresas.FirstOrDefault(x => x.Id == data.LoggedUser.CompanyId.Value));

                    if (company.ExportPv.HasValue)
                    {
                        NewOrderPopup?.Invoke(this, company);
                    }
                    else
                    {
                        await IrNuevaNotaPedido(company, false);
                    }
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error",e.Message,"Aceptar"); return;
            }
        }

        public async Task IrNuevaNotaPedido(Company company, bool export)
        {
            if(export)
            {
                var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderExportViewModel>();
                var order = new OrderNote() { orderStatus = 1, fecha = DateTime.Now, company = company };

                createViewModel.NewOrderCreatedd += CreateViewModel_NewOrderCreated;
                await navigationService.Navigate(createViewModel, order);
                //var s = await navigationService.Navigate<, OrderNote>(order);
            }
            else
            {
                var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderViewModel>();
                var order = new OrderNote() { orderStatus = 1, fecha = DateTime.Now , company = company};

                createViewModel.NewOrderCreated += CreateViewModel_NewOrderCreated;
                await navigationService.Navigate(createViewModel, order);
            }
        }

        private async void CreateViewModel_NewOrderCreated(bool created)
        {
            var requestData = new OrdersNotesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
            };

            await GetOrdersNoteAsync(requestData, true);
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            var requestData = new OrdersNotesPaginatedRequest()
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
            };

            await GetOrdersNoteAsync(requestData);
        }

        private async Task GetOrdersNoteAsync(OrdersNotesPaginatedRequest requestData, bool newSearch = false)
        {
            try
            {
                var user = data.LoggedUser;

                IsLoading = true;

                requestData.userId = user.Id;

                var red = await Connection.SeeConnection();

                if (red)
                {
                    var ordersnote = await prometeoApiService.GetOrderNote(requestData, user.Token);

                    if (newSearch)
                    {
                        OrdersNote.Clear();
                    }

                    var ordenNotes = new MvxObservableCollection<OrderNote>(ordersnote.Results.OrderByDescending(x => x.fecha));
                    OrdersNote.AddRange(ordenNotes);

                    CurrentPage = CurrentPage++;
                    IsLoading = false;
                }
                else
                {
                    OrdersNote.Clear();

                    var d = OfflineDatabase.GetOrderNotes();

                    if (d != null)
                    {
                        var orderNotes = new MvxObservableCollection<OrderNote>(mapper.Map<List<OrderNote>>(d));
                        OrdersNote.AddRange(orderNotes);
                    }
                    IsLoading = false;
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Aceptar"); return;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterOrders()
        {
            var filtro = await navigationService.Navigate<FilterOrdersViewModel, FilterOrderModel>();

            try
            {
                var user = data.LoggedUser;

                if (filtro != null)
                {
                    IsLoading = true;

                    FechaInicioFiltro = filtro.dateFrom.ToString("d");
                    FechaFinFiltro = filtro.dateTo.ToString("d");

                    var red = await Connection.SeeConnection();

                    if (red)
                    {
                        var orders = await prometeoApiService.GetOrdersByfilter(filtro, user.Token);

                        OrdersNote.Clear();

                        var ordenOportunidad = new MvxObservableCollection<OrderNote>(orders.OrderByDescending(x => x.fecha));
                        OrdersNote.AddRange(ordenOportunidad);
                    }
                    else
                    {
                        #region MODO OFFLINE

                        if (user.Language.abbreviation.ToLower() == "es" || user.Language.abbreviation.Contains("spanish"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Atención", "Revise su conexión a internet.", "Aceptar");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Attention", "Check your internet connection.", "Acept");
                            return;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, "Aceptar");
                return;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AbrirNota(OrderNote orderNote)
        {
            if (orderNote.IsExport)
            {
                var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderExportViewModel>();
                createViewModel.NewOrderCreatedd += CreateViewModel_NewOrderCreated;
                await navigationService.Navigate(createViewModel, orderNote);
            }
            else
            {
                var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderViewModel>();
                createViewModel.NewOrderCreated += CreateViewModel_NewOrderCreated;
                await navigationService.Navigate(createViewModel, orderNote);
            }
        }
    }
}
