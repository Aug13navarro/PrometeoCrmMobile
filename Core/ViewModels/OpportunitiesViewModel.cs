using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class OpportunitiesViewModel : MvxViewModel
    {
        private readonly IOfflineDataService offlineDataService;
        private ApplicationData data;
        // Properties
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private bool searchBarVisible;
        public bool SearchBarVisible
        {
            get => searchBarVisible;
            private set => SetProperty(ref searchBarVisible, value);
        }

        private string opportunitiesQuery;
        public string OpportunitiesQuery
        {
            get => opportunitiesQuery;
            set
            {
                SetProperty(ref opportunitiesQuery, value);
                if(string.IsNullOrWhiteSpace(this.opportunitiesQuery))
                {
                   SearchAsync();
                }
            }
        }

        private decimal totalOfAllOportunities;
        public decimal TotalOfAllOportunities
        {
            get => Opportunities.Sum(x => x.totalPrice);
            set
            {
                SetProperty(ref totalOfAllOportunities, value);
            }
        }

        public MvxObservableCollection<Opportunity> Opportunities { get; set; } = new MvxObservableCollection<Opportunity>();

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public Command LoadMoreOpportunitiesCommand { get; }
        public Command CreateOpportunityCommand { get; }
        public Command EditOpportunityCommand { get; }
        public Command NewOpportunitiesSearchCommand { get; }
        public Command FilterOportunities { get; }
        public Command ActivarSearchCommand { get; }
        public Command SearchOpportunityCommand { get; }
        public Command RefreshCommand { get; }

        // Constants
        private const int PageSize = 10;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        private readonly IToastService toastService;

        public OpportunitiesViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService, IToastService toastService, IOfflineDataService offlineDataService)
        {
            data = new ApplicationData();
            this.offlineDataService = offlineDataService;

            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            this.toastService = toastService;

            LoadMoreOpportunitiesCommand = new Command(async () => await LoadMoreOpportunitiesAsync());
            CreateOpportunityCommand = new Command(async () => await CreateOpportunityAsync());
            EditOpportunityCommand = new Command<Opportunity>(async o => await EditOpportunityAsync(o));
            NewOpportunitiesSearchCommand = new Command(async () => await NewOpportunitiesSearchAsync());

            FilterOportunities = new Command(async () => await OpenFilterAsync());
            ActivarSearchCommand = new Command(async () => await ActivarSearch());
            SearchOpportunityCommand = new Command(async () => await SearchOpportunity());
            RefreshCommand = new Command(async () => await RefreshList());

            Opportunities.CollectionChanged += (sender, arg) =>
            {
                TotalOfAllOportunities = Opportunities.Sum(x => x.totalPrice);
            };

            MessagingCenter.Subscribe<FilterOpportunitiesViewModel, OpportunitiesViewModel>(this, "filtered", (sender, model) =>
            {
                Opportunities = model.Opportunities;
                Application.Current.MainPage.Navigation.PopModalAsync();
            });

            Sincronizar();
        }

        private async void Sincronizar()
        {
            try
            {
                if (offlineDataService.IsWifiConection)
                {
                    await offlineDataService.LoadOpportunities();
                    var opportunities = await offlineDataService.SearchOpportunities();

                    if (opportunities.Count > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Sincronizando", "Se avisara cuando este listo", "Aceptar");

                        foreach (var item in opportunities)
                        {
                            var send = new OpportunityPost
                            {
                                branchOfficeId = item.customer.Id,
                                closedDate = item.createDt,
                                closedReason = "",
                                customerId = item.customer.Id,
                                description = item.description,
                                opportunityProducts = new List<OpportunityPost.ProductSend>(),
                                opportunityStatusId = item.opportunityStatus.Id,
                                totalPrice = Convert.ToDouble(item.totalPrice)
                            };

                            send.opportunityProducts = listaProductos(item.Details);

                            await prometeoApiService.SaveOpportunityCommand(send, data.LoggedUser.Token, item);
                        }

                        await Application.Current.MainPage.DisplayAlert(
                            "Sincronizado", "Sincronizacion terminada", "Aceptar");
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private List<OpportunityPost.ProductSend> listaProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var lista = new List<OpportunityPost.ProductSend>();

            foreach (var item in details)
            {
                double tempTotal = item.Price * item.Quantity;

                if (item.Discount != 0)
                {
                    tempTotal = tempTotal - ((tempTotal * item.Discount) / 100);
                }

                lista.Add(new OpportunityPost.ProductSend
                {
                    discount = item.Discount,
                    productId = item.productId,
                    quantity = item.Quantity,
                    total = tempTotal,
                    price = item.Price
                });
            }

            return lista;
        }
        private async Task RefreshList()
        {
            //SearchBarVisible = false;

            var requestData = new OpportunitiesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                Query = OpportunitiesQuery,
            };

            await GetOpportunitiesAsync(requestData, true);
        }

        private async Task SearchOpportunity()
        {
            SearchBarVisible = false;

            //var requestData = new OpportunitiesPaginatedRequest()
            //{
            //    CurrentPage = 1,
            //    PageSize = PageSize,
            //    Query = OpportunitiesQuery,
            //};

            //await GetOpportunitiesAsync(requestData, true);

            var stringUpper = OpportunitiesQuery.ToUpper();

            var listaSaerch = new MvxObservableCollection<Opportunity>(
            Opportunities.Where(x => x.customer.CompanyName.ToUpper().Contains(stringUpper)));

            Opportunities.Clear();
            Opportunities.AddRange(listaSaerch);

            await Task.FromResult(0);
        }

        private async Task ActivarSearch()
        {
            SearchBarVisible = true;
            await Task.FromResult(0);
        }

        private async Task OpenFilterAsync()
        {
            var filtro = await navigationService.Navigate<FilterOpportunitiesViewModel, FilterOportunityModel>();

            try
            {
                //SelectedCustomer = await prometeoApiService.GetCustomer(customerId);
                var user = data.LoggedUser;

                if (filtro != null)
                {
                    IsLoading = true;

                    var opportunities = await prometeoApiService.GetOppByfilter(filtro,user.Token);

                    Opportunities.Clear();

                    var ordenOportunidad = new MvxObservableCollection<Opportunity>(opportunities.OrderByDescending(x => x.closedDate));
                    Opportunities.AddRange(ordenOportunidad);
                }
            }
            catch (Exception ex)
            {
                toastService.ShowError("Ocurrió un error al cargar el filtro. Compruebe su conexión a internet.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new OpportunitiesPaginatedRequest()
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
            };

            await GetOpportunitiesAsync(requestData);
        }

        private async Task GetOpportunitiesAsync(OpportunitiesPaginatedRequest requestData, bool newSearch = false)
        {
            try
            {
                var user = data.LoggedUser;

                IsLoading = true;
                
                var request = new ProductList();

                var opportunities = await prometeoApiService.GetOp(requestData, "/api/Opportunity/GetListByCustomerIdAsync", user.Token);

                if (newSearch)
                {
                    Opportunities.Clear();
                }

                var ordenOportunidad = new MvxObservableCollection<Opportunity>(opportunities.OrderByDescending(x => x.closedDate));
                Opportunities.AddRange(ordenOportunidad);

                //CurrentPage = opportunities.CurrentPage;
                //TotalPages = opportunities.TotalPages;

                IsLoading = false;

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

        private async Task LoadMoreOpportunitiesAsync()
        {
            var requestData = new OpportunitiesPaginatedRequest()
            {
                CurrentPage = CurrentPage + 1,
                PageSize = PageSize,
                Query = OpportunitiesQuery,
            };

            await GetOpportunitiesAsync(requestData);
        }

        private async Task NewOpportunitiesSearchAsync()
        {
            var requestData = new OpportunitiesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                Query = OpportunitiesQuery,
            };

            await GetOpportunitiesAsync(requestData,true);
        }
        private async Task SearchAsync()
        {
            SearchBarVisible = false;

            var requestData = new OpportunitiesPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                Query = OpportunitiesQuery,
            };

            await GetOpportunitiesAsync(requestData, true);
        }

        private async Task CreateOpportunityAsync()
        {
            var createOpportunityViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOpportunityViewModel>();
            var opportunity = new Opportunity() { opportunityStatus = new OpportunityStatus { Id = 1}, closedDate = DateTime.Now };

            createOpportunityViewModel.NewOpportunityCreated += async (sender, args) => await NewOpportunitiesSearchAsync();
            await navigationService.Navigate(createOpportunityViewModel, opportunity);

            //await Initialize();
        }

        private async Task EditOpportunityAsync(Opportunity opportunity)
        {
            opportunity.Details = new MvxObservableCollection<OpportunityProducts>(opportunity.opportunityProducts);
            var createOpportunityViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOpportunityViewModel>();

            createOpportunityViewModel.NewOpportunityCreated += async (sender, args) => await NewOpportunitiesSearchAsync();
            await navigationService.Navigate(createOpportunityViewModel, opportunity);
        }
    }
}
