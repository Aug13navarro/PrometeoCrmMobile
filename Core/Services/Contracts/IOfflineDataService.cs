using Core.Model;
using Core.Model.Extern;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Contracts
{
    public interface IOfflineDataService
    {
        //bool IsWifiConection { get; }
        //bool IsMobileConection { get; }

        bool IsDataLoadedCustomer { get; }
        bool IsDataLoadedCompanies { get; }
        bool IsDataLoadedPaymentConditions { get; }
        bool IsDataLoadedPresentations { get; }
        bool IsDataLoadedOpportunities { get; }
        bool IsDataLoadedOrderNote { get; }
        bool IsDataLoadedOpportunityStatus { get; }

        Task LoadAllData();
        Task LoadCompanies();
        Task LoadDataPayment();
        Task LoadPresentation();
        Task LoadOpportunities();
        Task LoadOrderNotes();
        Task LoadOpportunityStatus();

        void UnloadAllData();
        Task SynchronizeToDisk();
        Task DeleteAllData();
        Task DeleteOpportunities();

        void SaveCustomerSearch(IList<CustomerExtern> policies);
        void SaveCompanySearch(List<CompanyExtern> companies);
        void SavePaymentConditions(List<PaymentCondition> paymentConditions);
        void SavePresentations(List<Product> products);
        void SaveOpportunity(Opportunity opportunity);
        void SaveOrderNotes(OrderNote orderNote);
        void SaveOpportunityStatus(List<OpportunityStatusExtern> opportunityStatuses);

        Task<List<CustomerExtern>> SearchCustomers();
        Task<List<CompanyExtern>> SearchCompanies();
        Task<List<PaymentCondition>> SearchPaymentConditions();
        Task<List<Product>> SearchPresentations();
        Task<List<Opportunity>> SearchOpportunities();
        Task<List<OrderNote>> SearchOrderNotes();
        Task<List<OpportunityStatusExtern>> SearchOpportunityStatuses();
    }
}
