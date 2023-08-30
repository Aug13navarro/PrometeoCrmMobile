using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Data;
using Core.Helpers;
using Core.Model;
using Core.Model.Common;
using Core.Services;
using Core.Services.Contracts;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Core.ViewModels
{
    public class ContactsViewModel : MvxViewModel
    {
        // Properties
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
            set => SetProperty(ref query, value);
        }

        public MvxObservableCollection<CustomerContact> Contacts { get; } = new MvxObservableCollection<CustomerContact>();

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; }

        // Commands
        public Command LoadMoreContactsCommand { get; }
        public Command GoToCreateContactCommand { get; }
        public Command NewContactsSearchCommand { get; }

        // Constants
        private const int PageSize = 10;

        // Services
        private ApplicationData applicationData { get; set; }
        private readonly IPrometeoApiService prometeoApiService;
        private readonly IMvxNavigationService navigationService;
        //private readonly IOfflineDataService offlineDataService;

        IMapper mapper;

        public ContactsViewModel(IPrometeoApiService prometeoApiService, IMvxNavigationService navigationService)//IOfflineDataService offlineDataService
        {
            applicationData = new ApplicationData();
            
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
            });

            mapper = mapperConfig.CreateMapper();

            this.prometeoApiService = prometeoApiService;
            this.navigationService = navigationService;
            //this.offlineDataService = offlineDataService;

            LoadMoreContactsCommand = new Command(async () => await LoadMoreContactsAsync());
            GoToCreateContactCommand = new Command(async () => await GoToCreateContactAsync());
            NewContactsSearchCommand = new Command(async () => await NewContactsSearchAsync());
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var requestData = new ContactsPaginatedRequest()
            {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
            };

            await SearchCustomersAsync(requestData);
        }

        private async Task SearchCustomersAsync(ContactsPaginatedRequest requestData, bool newSearch = false)
        {
            try
            {
                IsSearchInProgress = true;

                var red = await Connection.SeeConnection();

                if (red)
                {
                    Error = false;

                    PaginatedList<CustomerContact> contacts = await prometeoApiService.SearchCustomerContacts(requestData);

                    if (newSearch)
                    {
                        Contacts.Clear();
                    }

                    Contacts.AddRange(contacts.Results);

                    CurrentPage = contacts.CurrentPage;
                    TotalPages = contacts.TotalPages;
                }
                else
                {
                    
                }
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

        private async Task LoadMoreContactsAsync()
        {
            var requestData = new ContactsPaginatedRequest()
            {
                CurrentPage = CurrentPage + 1,
                PageSize = PageSize,
                Name = Query,
            };

            await SearchCustomersAsync(requestData);
        }

        private async Task GoToCreateContactAsync()
        {
            await navigationService.Navigate<ContactEditViewModel, Customer>(new Customer() {Id = 999});
        }

        private async Task NewContactsSearchAsync()
        {
            var requestData = new ContactsPaginatedRequest()
            {
                CurrentPage = 1,
                PageSize = PageSize,
                Name = Query,
            };

            await SearchCustomersAsync(requestData, true);
        }
    }
}
