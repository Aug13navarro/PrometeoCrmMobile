using System;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using Core.Services.Contracts;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class ProductsViewModel : MvxViewModelResult<OpportunityProducts>
    {
        private ApplicationData data;

        #region PROPIEDADES

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
                if(this.query == "")
                {
                    SearchQueryAsync();
                }
            }
        }

        private int customerTypeId;
        public int CustomerTypeId
        {
            get => customerTypeId;
            set => SetProperty(ref customerTypeId, value);
        }

        public MvxObservableCollection<Product> Products { get; } = new MvxObservableCollection<Product>();

        #endregion

        // Events
        public event EventHandler<Product> ShowSelectProductPopup;

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public Command LoadMorePruductsCommand { get; }
        public Command SelectProductCommand { get; }
        public Command SearchQueryCommand { get; }


        //Constant
        private const int PageSize = 30;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;

        private int companyId;

        public ProductsViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService)
        {
            try
            {
                data = new ApplicationData();

                companyId = 7;

                Query = "";
                this.prometeoApiService = prometeoApiService;
                this.navigationService = navigationService;

                LoadMorePruductsCommand = new Command(async () => await LoadMoreProductsAsync());
                SelectProductCommand = new Command<Product>(SelectProduct);
                SearchQueryCommand = new Command(async () => await SearchQueryAsync());
            }
            catch ( Exception e)
            {
                var s = e.Message;
            }
        }

        private async Task SearchQueryAsync()
        {
            if (Query != null)
            {
                CurrentPage = 1;

                var requestData = new ProductList
                {
                    companyId = companyId,
                    currentPage = CurrentPage,
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
                companyId = companyId,
                currentPage = CurrentPage,
                pageSize = PageSize,
                query = Query,
            };

            await SearchProductsAsync(requestData);
        }

        public async Task Close(OpportunityProducts opportunityDetail)
        {
            await navigationService.Close(this, opportunityDetail);
        }

        private void SelectProduct(Product product)
        {
            ShowSelectProductPopup?.Invoke(this, product);
        }
        private async Task LoadMoreProductsAsync()
        {
            var requestData = new ProductList
            {
                companyId = companyId,
                currentPage = CurrentPage,
                pageSize = PageSize,
                query = Query,
            };

            await SearchProductsAsync(requestData);
        }
        private async Task SearchProductsAsync(ProductList requestData, bool newSearch = false)
        {
            try
            {
                var red = await Connection.SeeConnection();
                if (red)
                {
                    IsSearchInProgress = true;
                    Error = false;

                    var products = await prometeoApiService.GetAvailableProducts(requestData, data.LoggedUser.Token);

                    if (products.Results.Count > 0)
                    {
                        CurrentPage++;
                    }

                    if (newSearch)
                    {
                        Products.Clear();
                    }

                    Products.AddRange(products.Results);
                }
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
