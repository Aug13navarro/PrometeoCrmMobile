using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Core.Model.Enums;
using Core.Services.Contracts;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class OpportunitiesViewModel : MvxViewModel
    {
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

        private decimal totalOfAllOportunities;
        public decimal TotalOfAllOportunities
        {
            get => Opportunities.Sum(x => x.Total);
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

        // Constants
        private const int PageSize = 10;

        // Services
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        private readonly IToastService toastService;

        public OpportunitiesViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService, IToastService toastService)
        {
            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            this.toastService = toastService;

            LoadMoreOpportunitiesCommand = new Command(async () => await LoadMoreOpportunitiesAsync());
            CreateOpportunityCommand = new Command(async () => await CreateOpportunityAsync());
            EditOpportunityCommand = new Command<Opportunity>(async o => await EditOpportunityAsync(o));
            NewOpportunitiesSearchCommand = new Command(async () => await NewOpportunitiesSearchAsync());

            Opportunities.CollectionChanged += (sender, arg) =>
            {
                TotalOfAllOportunities = Opportunities.Sum(x => x.Total);
            };

            MessagingCenter.Subscribe<FilterOpportunitiesViewModel, OpportunitiesViewModel>(this, "filtered", (sender, model) =>
            {
                Opportunities = model.Opportunities;
                Application.Current.MainPage.Navigation.PopModalAsync();
            });
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
                IsLoading = true;

                PaginatedList<Opportunity> opportunities = await prometeoApiService.GetOpportunities(requestData);

                if (newSearch)
                {
                    Opportunities.Clear();
                }

                Opportunities.AddRange(opportunities.Results);

                CurrentPage = opportunities.CurrentPage;
                TotalPages = opportunities.TotalPages;
            }
            catch (Exception ex)
            {
                toastService.ShowError("Error cargando oportunidades. Compruebe su conexión a internet.");
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

            await GetOpportunitiesAsync(requestData, true);
        }

        private async Task CreateOpportunityAsync()
        {
            var createOpportunityViewModel = MvxIoCProvider.Instance.IoCConstruct<CreateOpportunityViewModel>();
            var opportunity = new Opportunity() { Status = OpportunityStatus.Analysis, Date = DateTime.Now };

            createOpportunityViewModel.NewOpportunityCreated += async (sender, args) => await NewOpportunitiesSearchAsync();
            await navigationService.Navigate(createOpportunityViewModel, opportunity);
        }

        private async Task EditOpportunityAsync(Opportunity opportunity)
        {
            await navigationService.Navigate<CreateOpportunityViewModel, Opportunity>(opportunity);
        }

    }
}
