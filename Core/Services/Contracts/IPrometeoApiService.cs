using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Common;
using Core.ViewModels.Model;

namespace Core.Services.Contracts
{
    public interface IPrometeoApiService
    {
        Task<LoginData> Login(string userName, string password);
        Task<string> RecoverPassword(string mail);
        Task<UserData> GetUserData(int userId);
        Task<Assignment> GetAssignment(int id);
        Task<PaginatedList<Assignment>> SearchAssignment(SearchAssignmentRequest requestData);
        Task<PaginatedList<TreatmentAlert>> GetAllNotifications(NotificationsPaginatedRequest requestData);
        Task SetViewedNotification(int notificationId);
        Task<PaginatedList<Customer>> GetCustomers(CustomersPaginatedRequest requestData);
        Task<List<Customer>> GetAllCustomer(int userId, bool isParent, int typeCustomer, string token, int companyId);
        Task<List<Customer>> GetCustomersOld(CustomersOldRequest requestData);
        Task<Customer> CreateCustomer(Customer requestData);
        Task<List<CustomerType>> GetCustomerTypes();
        Task<List<DocumentType>> GetDocumentTypes();
        Task<List<TaxCondition>> GetTaxConditions();
        Task<List<Company>> GetCompaniesByUserId(int userId, string token);
        Task<PaginatedList<CustomerContact>> SearchCustomerContacts(ContactsPaginatedRequest requestData);
        Task<Customer> GetCustomer(int id);
        Task UpdateCustomer(Customer customer);
        Task<PaginatedList<Product>> GetAvailableProducts(ProductList productList, string token);
        //Task<PaginatedList<Opportunity>> GetOpportunities(OpportunitiesPaginatedRequest requestData);
        Task<IEnumerable<Opportunity>> GetOp(OpportunitiesPaginatedRequest requestData, string lang, string token);
        Task<bool> SaveOpportunityCommand(OpportunityPost opportunityPost, string token, Opportunity opportunity);

        Task<List<Opportunity>> GetOpportunietesTest(int userId);
        Task<IEnumerable<Opportunity>> GetOppByfilter(FilterOportunityModel filtro,string lang, string token);
        Task<Opportunity> GetOppById(int id);
        Task SaveOpportunityEdit(OpportunityPost send, int id, string token, Opportunity opportunity);
        Task<IEnumerable<PaymentCondition>> GetPaymentConditions(string token, int companyId);
        Task<OrderNote> CreateOrderNote(OrderNote nuevaOrder);
        Task<OrderNote> UpdateOrderNote(OrderNote order,string token);
        Task<PaginatedList<OrderNote>> GetOrderNote(OrdersNotesPaginatedRequest requestData, string token);
        Task<IEnumerable<OrderNote>> GetOrdersByfilter(FilterOrderModel filtro, string token);
        Task<IEnumerable<OpportunityStatus>> GetOpportunityStatus(string lang, string token);
        Task<OrderNote> GetOrdersById(int id, string token);
        Task<IEnumerable<User>> GetUsersByRol(int companyId, string rol);
        Task<IEnumerable<PaymentMethod>> GetPaymentMethod(int companyId, string language, string token);
        Task<IEnumerable<Incoterm>> GetIncoterms(string token);
        Task<IEnumerable<FreightInCharge>> GetFreight(string language, string token);
        Task<IEnumerable<Transport>> GetTransport(string language, string token);
    }
}
