using Core.Model;
using Core.Model.Extern;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Contracts
{
    public interface IOfflineDataService
    {
        bool IsWifiConection { get; }
        bool IsMobileConection { get; }

        bool IsDataLoadesCustomer { get; }
        bool IsDataLoadesCompanies { get; }
        bool IsDataLoadesPaymentConditions { get; }
        bool IsDataLoadesPresentations { get; }
        bool IsDataLoadesOpportunities { get; }
        bool IsDataLoadesOrderNote { get; }

        Task LoadAllData();
        Task LoadCompanies();
        Task LoadDataPayment();
        Task LoadPresentation();
        Task LoadOpportunities();
        Task LoadOrderNotes();

        void UnloadAllData(string tipo);
        Task SynchronizeToDisk();
        Task DeleteAllData();

        void SaveCustomerSearch(IList<CustomerExtern> policies);
        void SaveCompanySearch(List<Company> companies);
        void SavePaymentConditions(List<PaymentCondition> paymentConditions);
        void SavePresentations(List<Product> products);
        void SaveOpportunity(Opportunity opportunity);
        void SaveOrderNotes(OrderNote orderNote);

        Task<List<CustomerExtern>> SearchCustomers();
        Task<List<Company>> SearchCompanies();
        Task<List<PaymentCondition>> SearchPaymentConditions();
        Task<List<Product>> SearchPresentations();
        Task<List<Opportunity>> SearchOpportunities();
        Task<List<OrderNote>> SearchOrderNotes();
    }
}
