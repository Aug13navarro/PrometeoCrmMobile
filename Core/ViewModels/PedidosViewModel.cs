using Core.Model;
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
using Xamarin.Forms;

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
            if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
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
        private const int PageSize = 20;

        public Command NuevaNotaPedidoCommand { get; }
        public Command FilterOrdersCommand { get; }
        public Command OpenOrderNoteCommand { get; }
        public Command SearchQueryCommand { get; }
        public Command RefreshListCommand { get; }


        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        //private readonly IToastService toastService;

        public PedidosViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService)//,IToastService toastService
        {
            data = new ApplicationData();

            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            //this.toastService = toastService;

            NuevaNotaPedidoCommand = new Command(async () => await NuevaNotaPedido());
            FilterOrdersCommand = new Command(async () => await FilterOrders());
            OpenOrderNoteCommand = new Command<OrderNote>(async o => await AbrirNota(o));
            SearchQueryCommand = new Command(async () => await SearchQuery());
            RefreshListCommand = new Command(async () => await RefreshList());

            OrdersNote.CollectionChanged += (sender, arg) =>
            {
                Total = OrdersNote.Sum(x => x.total);
            };

            if (data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
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
                PageSize = 20,
                query = Query
            };

            await GetOrdersNoteAsync(requestData, true);
        }

        private async Task SearchQuery()
        {
            var requestData = new OrdersNotesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = 20,
                query = Query
            };

            await GetOrdersNoteAsync(requestData, true);
        }

        private async Task NuevaNotaPedido()
        {
            var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderViewModel>();
            var order = new OrderNote() { orderStatus = 1, fecha = DateTime.Now };

            createViewModel.NewOrderCreated += async (sender, args) => await NewOrderSearchAsync();
            await navigationService.Navigate(createViewModel, order);
        }

        private Task NewOrderSearchAsync()
        {
            throw new NotImplementedException();
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

                //PaginatedList<Opportunity> opportunities = await prometeoApiService.GetOpportunities(requestData);//"https://neophos-testing-api.azurewebsites.net/api/Opportunity/GetListByCustomerIdAsync", ,user.Token
                var ordersnote = await prometeoApiService.GetOrderNote(requestData, user.Token);

                if (newSearch)
                {
                    OrdersNote.Clear();
                }

                var ordenNotes = new MvxObservableCollection<OrderNote>(ordersnote.Results.OrderByDescending(x => x.fecha));
                OrdersNote.AddRange(ordenNotes);

                CurrentPage = CurrentPage++;
                IsLoading = false;
                //TotalPages = opportunities.TotalPages;

            }
            catch (Exception ex)
            {
                //toastService.ShowError($"{ex.Message}");
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

                    var orders = await prometeoApiService.GetOrdersByfilter(filtro, user.Token);

                    FechaInicioFiltro = filtro.dateFrom.ToString("d");
                    FechaFinFiltro = filtro.dateTo.ToString("d");

                    OrdersNote.Clear();

                    var ordenOportunidad = new MvxObservableCollection<OrderNote>(orders.OrderByDescending(x => x.fecha));
                    OrdersNote.AddRange(ordenOportunidad);
                }
            }
            catch (Exception ex)
            {
                //toastService.ShowError($"{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AbrirNota(OrderNote orderNote)
        {
            await navigationService.Navigate<CreateOrderViewModel, OrderNote>(orderNote);
        }
    }
}
