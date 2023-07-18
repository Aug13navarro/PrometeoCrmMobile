using Core.Model;
using Core.Services.Contracts;
using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ChangeCompanyViewModel : MvxViewModel
    {
        private MvxObservableCollection<Company> companies;
        public MvxObservableCollection<Company> ListCompanies
        {
            get => companies;
            set => SetProperty(ref companies, value);
        }

        private readonly ApplicationData data;
        private readonly IPrometeoApiService _prometeoApiService;

        public ChangeCompanyViewModel(IPrometeoApiService prometeoApiService)
        {
            data = new ApplicationData();
            _prometeoApiService = prometeoApiService;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await GetCompanies(data.LoggedUser.Id);
        }

        public async Task GetCompanies(int userId)
        {
            var companies = await _prometeoApiService.GetCompaniesByUserId(userId, data.LoggedUser.Token);
            ListCompanies = new MvxObservableCollection<Company>(companies);
        }
    }
}
