using Core.Model;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class SalesViewModel : MvxViewModel
    {
        private readonly ApplicationData data;

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }
        


        public int CurrentPage { get; private set; } = 1;
        private const int PageSize = 10;

        public Command FilterOrdersCommand { get; }
        public Command NuevaNotaPedidoCommand { get; }

        private readonly IMvxNavigationService navigationService;
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IToastService toastService;

        public SalesViewModel(IMvxNavigationService navigationService, IPrometeoApiService prometeoApiService,
                                          IToastService toastService)
        {
            data = new ApplicationData();

            this.navigationService = navigationService;
            this.prometeoApiService = prometeoApiService;
            this.toastService = toastService;

            FilterOrdersCommand = new Command(async () => await FilterOrders());
            NuevaNotaPedidoCommand = new Command(async () => NuevaNotaPedido());

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
                //var salesList = await prometeoApiService.GetSales(requestData, user.Token);

                if (newSearch)
                {
                    
                }

                //var sales = new MvxObservableCollection<Sale>(salesList.Results.OrderByDescending(x => x.fecha));
                //Sales.AddRange(sales);

                //CurrentPage = opportunities.CurrentPage;
                //TotalPages = opportunities.TotalPages;

            }
            catch (Exception ex)
            {
                toastService.ShowError($"{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterOrders()
        {
            var filtro = await navigationService.Navigate<FilterOrdersViewModel, FilterOrderModel>();
        }

        private async void NuevaNotaPedido()
        {
            var createViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOrderViewModel>();
            var order = new OrderNote() { orderStatus = 1, fecha= DateTime.Now };

            createViewModel.NewOrderCreated += NewSalesSearchAsync;
            await navigationService.Navigate(createViewModel, order);
            //await navigationService.Navigate<CreateOrderViewModel, Opportunity>(order);
        }
        private async void NewSalesSearchAsync(bool created)
        {
            var requestData = new OrdersNotesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                //Query = OpportunitiesQuery,
            };

            await GetOrdersNoteAsync(requestData, true);
        }
    }
}
