using Core.Model;
using Core.Model.Extern;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Contracts
{
    public interface IOfflineDataService
    {
        bool IsDataLoadedCustomer { get; }
        bool IsDataLoadedCompanies { get; }
        bool IsDataLoadedPaymentConditions { get; }
        bool IsDataLoadedPresentations { get; }
        bool IsDataLoadedOpportunities { get; }
        bool IsDataLoadedOrderNote { get; }
        bool IsDataLoadedOpportunityStatus { get; }
        bool IsDataLoadedAssistant { get;}
        bool IsDataLoadedPaymentMethod { get; }
        bool IsDataLoadedIncoterms { get; }
        bool IsDataLoadedFreigths { get; }
        bool IsDataLoadedTransports { get; }

        Task LoadAllData();
        Task LoadCompanies();
        Task LoadDataPayment();
        Task LoadPresentation();
        Task LoadOpportunities();
        Task LoadOrderNotes();
        Task LoadOpportunityStatus();
        Task LoadAssistant();
        Task LoadPaymentMethod();
        Task LoadIncoterms();
        Task LoadFreights();
        Task LoadTransports();


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
        void SaveAssitant(List<UserExtern> userExterns);
        void SavePaymentMethod(List<PaymentMethodExtern> paymentMethodExterns);
        void SaveIncoterms(List<IncotermExtern> incotermExterns);
        void SaveFreights(List<FreightInChargeExtern> freightInChargeExterns);
        void SaveTransports(List<TransportExtern> transportExterns);

        Task<List<CustomerExtern>> SearchCustomers();
        Task<List<CompanyExtern>> SearchCompanies();
        Task<List<PaymentCondition>> SearchPaymentConditions();
        Task<List<Product>> SearchPresentations();
        Task<List<Opportunity>> SearchOpportunities();
        Task<List<OrderNote>> SearchOrderNotes();
        Task<List<OpportunityStatusExtern>> SearchOpportunityStatuses();
        Task<List<UserExtern>> SearchAssistant();
        Task<List<PaymentMethodExtern>> SearchPaymentMethod();
        Task<List<IncotermExtern>> SearchIncoterms();
        Task<List<FreightInChargeExtern>> SearchFreights();
        Task<List<TransportExtern>> SearchTransports();
    }
}
