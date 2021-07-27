using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Core.Model.Enums;
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
        private ApplicationData data;
        // Properties
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => SetProperty(ref isLoading, value);
        }

        private string opportunitiesQuery;
        public string OpportunitiesQuery
        {
            get => opportunitiesQuery;
            set => SetProperty(ref opportunitiesQuery, value);
        }

        private double totalOfAllOportunities;
        public double TotalOfAllOportunities
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

        // Constants
        private const int PageSize = 10;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        private readonly IToastService toastService;

        public OpportunitiesViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService, IToastService toastService)
        {
            data = new ApplicationData();

            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            this.toastService = toastService;

            LoadMoreOpportunitiesCommand = new Command(async () => await LoadMoreOpportunitiesAsync());
            CreateOpportunityCommand = new Command(async () => await CreateOpportunityAsync());
            EditOpportunityCommand = new Command<Opportunity>(async o => await EditOpportunityAsync(o));
            NewOpportunitiesSearchCommand = new Command(async () => await NewOpportunitiesSearchAsync());

            FilterOportunities = new Command(async () => await OpenFilterAsync());

            Opportunities.CollectionChanged += (sender, arg) =>
            {
                TotalOfAllOportunities = Opportunities.Sum(x => x.totalPrice);
            };

            MessagingCenter.Subscribe<FilterOpportunitiesViewModel, OpportunitiesViewModel>(this, "filtered", (sender, model) =>
            {
                Opportunities = model.Opportunities;
                Application.Current.MainPage.Navigation.PopModalAsync();
            });
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

                    Opportunities.AddRange(opportunities);
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

                //PaginatedList<Opportunity> opportunities = await prometeoApiService.GetOpportunities(requestData);//"https://neophos-testing-api.azurewebsites.net/api/Opportunity/GetListByCustomerIdAsync", ,user.Token
                var opportunities = await prometeoApiService.GetOp(requestData, "https://neophos-testing-api.azurewebsites.net/api/Opportunity/GetListByCustomerIdAsync", user.Token);

                if (newSearch)
                {
                    Opportunities.Clear();
                }

                Opportunities.AddRange(opportunities);

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

        private async Task CreateOpportunityAsync()
        {
            var createOpportunityViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOpportunityViewModel>();
            var opportunity = new Opportunity() { opportunityStatus = new OpportunityStatus { Id = 1}, closedDate = DateTime.Now };

            createOpportunityViewModel.NewOpportunityCreated += async (sender, args) => await NewOpportunitiesSearchAsync();
            await navigationService.Navigate(createOpportunityViewModel, opportunity);
        }

        private async Task EditOpportunityAsync(Opportunity opportunity)
        {
            opportunity.Details = new MvxObservableCollection<OpportunityProducts>(opportunity.opportunityProducts);

            await navigationService.Navigate<CreateOpportunityViewModel, Opportunity>(opportunity);
        }

    }
}
