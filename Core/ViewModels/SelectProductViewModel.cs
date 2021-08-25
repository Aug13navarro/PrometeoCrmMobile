using Core.Model;
using Core.Services.Contracts;
using MvvmCross.Commands;
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
    public class SelectProductViewModel : MvxViewModelResult<Product>
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
                if (this.query == "")
                {
                    SearchQueryAsync();
                }
            }
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
        private const int PageSize = 10;

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

        private async Task SearchQueryAsync()
        {
            if (Query != null)
            {
                var requestData = new ProductList
                {
                    companyId = 7,
                    currentPage = 1,
                    pageSize = 50,
                    query = Query,
                    sort = null,
                };

                //if (Query == "")
                //{
                //    await SearchProductsAsync(requestData);
                //}
                //else
                //{
                await SearchProductsAsync(requestData, true);
                //}
            }
        }

        //private async void SearchQueryAsync()
        //{
        //    if (Query != null)
        //    {
        //        var requestData = new ProductList
        //        {
        //            companyId = 7,
        //            currentPage = 50,
        //            pageSize = 1,
        //            query = Query,
        //            sort = null,
        //        };

        //        if (Query == "")
        //        {
        //            await SearchProductsAsync(requestData);
        //        }
        //        else
        //        {
        //            await SearchProductsAsync(requestData, true);
        //        }
        //    }
        //}

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new ProductList
            {
                companyId = 7,
                currentPage = CurrentPage,
                pageSize = PageSize,
                query = Query,
            };

            await SearchProductsAsync(requestData);
            //var listProducts = await prometeoApiService.GetAvailableProducts(requestData);
            //Products = new MvxObservableCollection<Product>(listProducts.results);
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
                companyId = 7,
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

                Products.AddRange(products.ToList());

                //CurrentPage = contacts.currentPage;
                //TotalPages = contacts.totalPages;
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