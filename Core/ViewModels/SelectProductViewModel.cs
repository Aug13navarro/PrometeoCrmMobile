using Core.Model;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class SelectProductViewModel : MvxViewModel<DataExport,Product>
    {
        private ApplicationData data;

        private bool isSearchInProgress;
        public bool IsSearchInProgress
        {
            get => isSearchInProgress;
            private set => SetProperty(ref isSearchInProgress, value);
        }

        private bool error;
        public bool Error
        {
            get => error;
            private set => SetProperty(ref error, value);
        }

        private string query;
        public string Query
        {
            get => query;
            set
            {
                SetProperty(ref query, value);
                if (string.IsNullOrWhiteSpace(query))
                {
                    SearchQueryAsync();
                }
            }
        }

        private int companyId;
        public int CompanyId
        {
            get => companyId;
            set => SetProperty(ref companyId, value);
        }

        //private List<Product> products;
        //public List<Product> Products
        //{
        //    get => products;
        //    set => SetProperty(ref products, value);
        //}

        public MvxObservableCollection<Product> Products { get; } = new MvxObservableCollection<Product>();

        // Events
        //public event EventHandler<Product> ShowSelectProductPopup;

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public Command LoadMorePruductsCommand { get; }
        public IMvxAsyncCommand<Product> SelectProductCommand { get; }
        public Command SearchQueryCommand { get; }


        //Constant
        private const int PageSize = 30;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;

        public SelectProductViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService)
        {
            data = new ApplicationData();

            Query = "";
            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;

            LoadMorePruductsCommand = new Command(async () => await LoadMoreProductsAsync());
            SelectProductCommand = new MvxAsyncCommand<Product>(SelectProduct);
            SearchQueryCommand = new Command(async () => await SearchQueryAsync());
        }

        public override void Prepare(DataExport parameter)
        {
            CompanyId = parameter.CompanyId;
        }

        private async Task SearchQueryAsync()
        {
            if (Query != null)
            {
                var requestData = new ProductList
                {
                    companyId = CompanyId,
                    currentPage = 1,
                    pageSize = PageSize,
                    query = Query,
                    sort = null,
                };

                await SearchProductsAsync(requestData, true);
            }
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new ProductList
            {
                companyId = CompanyId,
                currentPage = CurrentPage,
                pageSize = PageSize,
                query = Query,
            };

            await SearchProductsAsync(requestData);

        }

        public async Task Close(Product opportunityDetail)
        {
            await navigationService.Close(this, opportunityDetail);
        }

        private async Task SelectProduct(Product product)
        {
            await navigationService.Close(this, product);
        }
        private async Task LoadMoreProductsAsync()
        {
            var requestData = new ProductList
            {
                companyId = CompanyId,
                currentPage = CurrentPage + 1,
                pageSize = PageSize,
                query = Query,
            };

            await SearchProductsAsync(requestData);
        }
        private async Task SearchProductsAsync(ProductList requestData, bool newSearch = false)
        {
            try
            {
                IsSearchInProgress = true;
                Error = false;

                var products = await prometeoApiService.GetAvailableProducts(requestData, data.LoggedUser.Token);

                if (newSearch)
                {
                    Products.Clear();
                }

                CurrentPage = products.CurrentPage;
                Products.AddRange(products.Results);

            }
            catch (Exception ex)
            {
                Error = true;
            }
            finally
            {
                IsSearchInProgress = false;
            }
        }
    }
}