using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Contracts
{
    public interface IOfflineDataService
    {
        bool IsWifiConection { get; }
        bool IsMobileConection { get; }

        bool IsDataLoaded { get; }

        Task LoadAllData();
        Task LoadCompanies();
        Task LoadDataPayment();
        Task LoadPresentation();
        Task LoadOpportunities();

        void UnloadAllData(string tipo);
        Task SynchronizeToDisk();
        Task DeleteAllData();

        void SaveCustomerSearch(IList<Customer> policies);
        void SaveCompanySearch(List<Company> companies);
        void SavePaymentConditions(List<PaymentCondition> paymentConditions);
        void SavePresentations(List<Product> products);
        void SaveOpportunity(Opportunity opportunity);

        Task<List<Customer>> SearchCustomers();
        Task<List<Company>> SearchCompanies();
        Task<List<PaymentCondition>> SearchPaymentConditions();
        Task<List<Product>> SearchPresentations();
        Task<List<Opportunity>> SearchOpportunities();
    }
}
