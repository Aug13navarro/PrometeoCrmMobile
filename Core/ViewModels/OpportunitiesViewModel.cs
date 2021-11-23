using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
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
                ConvertirTotalStr(this.totalOfAllOportunities);
            }
        }

        private string totalOfAllOportunitiesStr;
        public string TotalOfAllOportunitiesStr
        {
            get => totalOfAllOportunitiesStr;
            set
            {
                SetProperty(ref totalOfAllOportunitiesStr, value);
            }
        }

        private void ConvertirTotalStr(decimal totalOfAllOportunities)
        {
            if(data.LoggedUser.Language.ToLower() == "es" || data.LoggedUser.Language.Contains("spanish"))
            {
                TotalOfAllOportunitiesStr = TotalOfAllOportunities.ToString("N2", new CultureInfo("es-ES"));
            }
            else
            {
                TotalOfAllOportunitiesStr = TotalOfAllOportunities.ToString("N2", new CultureInfo("en-US"));
            }

            var s = TotalOfAllOportunitiesStr;
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

        public OpportunitiesViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService, IOfflineDataService offlineDataService)
        {
            data = new ApplicationData();
            this.offlineDataService = offlineDataService;

            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;

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
                var red = await Connection.SeeConnection();

                if (red)
                {
                    if (!offlineDataService.IsDataLoadedOpportunities)
                    {
                        await offlineDataService.LoadOpportunities();
                    }

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

                            //await prometeoApiService.SaveOpportunityCommand(send, data.LoggedUser.Token, item);
                        }

                        await offlineDataService.DeleteOpportunities();
                        offlineDataService.UnloadAllData();

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
                var user = data.LoggedUser;

                if (filtro != null)
                {
                    var red = await Connection.SeeConnection();

                    if (red)
                    {
                        IsLoading = true;

                        var opportunities = await prometeoApiService.GetOppByfilter(filtro, user.Language.ToLower(), user.Token);

                        Opportunities.Clear();

                        var ordenOportunidad = new MvxObservableCollection<Opportunity>(opportunities.OrderByDescending(x => x.closedDate));
                        Opportunities.AddRange(ordenOportunidad);
                    }
                    else
                    {
                        #region MODO OFFLINE

                        if (user.Language.ToLower() == "es" || user.Language.Contains("spanish"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Atención", "Revise su conexión a internet.", "Aceptar");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Attention", "Check your internet connection.", "Acept");
                            return;
                        }
                        //else
                        //{
                        //    IsLoading = true;

                        //    if(!offlineDataService.IsDataLoadedOpportunities)
                        //    {
                        //        await offlineDataService.LoadOpportunities();
                        //    }

                        //    var opsCache = await offlineDataService.SearchOpportunities();

                        //    var Opfiltro = new MvxObservableCollection<Opportunity>();

                        //    if (filtro.companies.Count > 0 && filtro.status.Count == 0 && filtro.priceFrom == null && filtro.priceTo == null) //Company
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.Company.Id == filtro.companies.FirstOrDefault().id 
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count > 0 && filtro.priceFrom == null && filtro.priceTo == null) //Status
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.opportunityStatus.Id == filtro.status.FirstOrDefault().id
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count == 0 && filtro.priceFrom != null && filtro.priceTo == null) //Price From
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.totalPrice >= filtro.priceFrom
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count == 0 && filtro.priceFrom == null && filtro.priceTo != null) //Price TO
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.totalPrice >= filtro.priceTo
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count > 0 && filtro.status.Count >= 0 && filtro.priceFrom == null && filtro.priceTo == null) //Company and Status
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.Company.Id == filtro.companies.FirstOrDefault().id && x.opportunityStatus.Id == filtro.status.FirstOrDefault().id
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count > 0 && filtro.status.Count == 0 && filtro.priceFrom != null && filtro.priceTo == null) //Company and Price From
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.Company.Id == filtro.companies.FirstOrDefault().id && x.totalPrice >= filtro.priceFrom
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count > 0 && filtro.status.Count == 0 && filtro.priceFrom == null && filtro.priceTo != null) //Company and Price To
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.Company.Id == filtro.companies.FirstOrDefault().id && x.totalPrice <= filtro.priceTo
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count > 0 && filtro.priceFrom != null && filtro.priceTo == null) //Status and Price From
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.opportunityStatus.Id == filtro.status.FirstOrDefault().id && x.totalPrice <= filtro.priceFrom
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count > 0 && filtro.priceFrom == null && filtro.priceTo != null) //Status and Price To
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.opportunityStatus.Id == filtro.status.FirstOrDefault().id && x.totalPrice <= filtro.priceTo
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }

                        //    if (filtro.companies.Count == 0 && filtro.status.Count == 0 && filtro.priceFrom != null && filtro.priceTo != null) //Price From and Price To
                        //    {
                        //        Opfiltro.AddRange(opsCache.Where(x => x.opportunityStatus.Id == filtro.status.FirstOrDefault().id && x.totalPrice <= filtro.priceTo
                        //                                        && x.closedDate >= filtro.dateFrom && x.closedDate <= filtro.dateTo));
                        //    }


                        //    Opportunities.Clear();

                        //    Opportunities.AddRange(Opfiltro);
                        //}

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Información", ex.Message, "Aceptar"); return;
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

                IEnumerable<Opportunity> opportunities;

                var red = await Connection.SeeConnection();

                if (red)
                {
                    opportunities = await prometeoApiService.GetOp(requestData, user.Language.ToLower(), user.Token);
                }
                else
                {
                    if (!offlineDataService.IsDataLoadedOpportunities)
                    {
                        await offlineDataService.LoadOpportunities();
                    }
                    opportunities = await offlineDataService.SearchOpportunities();

                    foreach (var item in opportunities) 
                    {
                        if(item.opportunityStatus.name == "" || item.opportunityStatus.name == null)
                        {
                            if(user.Language.ToLower() == "es" || user.Language.Contains("spanish"))
                            {
                                item.opportunityStatus.name = item.opportunityStatus.nameCacheEsp;
                            }
                            else
                            {
                                item.opportunityStatus.name = item.opportunityStatus.nameCacheEn;
                            }
                        }
                    }
                }


                if (newSearch)
                {
                    Opportunities.Clear();
                }

                var ordenOportunidad = new MvxObservableCollection<Opportunity>(opportunities.OrderByDescending(x => x.closedDate));
                Opportunities.AddRange(ordenOportunidad);


                IsLoading = false;

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("", ex.Message, "Aceptar");
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
            var opportunity = new Opportunity() { opportunityStatus = new OpportunityStatus { Id = 1 }, closedDate = DateTime.Now };

            createOpportunityViewModel.NewOpportunityCreated += async (sender, args) => await NewOpportunitiesSearchAsync();
            await navigationService.Navigate(createOpportunityViewModel, opportunity);
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
