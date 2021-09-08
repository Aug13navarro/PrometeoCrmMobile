using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Model;
using Core.Model.Common;
using Core.ViewModels.Model;

namespace Core.Services.Contracts
{
    public interface IPrometeoApiService
    {
        Task<LoginData> Login(string userName, string password);
        Task<UserData> GetUserData(int userId);
        Task<Assignment> GetAssignment(int id);
        Task<PaginatedList<Assignment>> SearchAssignment(SearchAssignmentRequest requestData);
        Task<PaginatedList<TreatmentAlert>> GetAllNotifications(NotificationsPaginatedRequest requestData);
        Task SetViewedNotification(int notificationId);
        Task<PaginatedList<Customer>> GetCustomers(CustomersPaginatedRequest requestData);
        Task<List<Customer>> GetAllCustomer(int userId, bool isParent, int typeCustomer, string token);
        Task<List<Customer>> GetCustomersOld(CustomersOldRequest requestData);
        Task CreateCustomer(Customer requestData);
        Task<List<CustomerType>> GetCustomerTypes();
        Task<List<DocumentType>> GetDocumentTypes();
        Task<List<TaxCondition>> GetTaxConditions();
        Task<List<Company>> GetCompaniesByUserId(int userId, string token);
        Task<PaginatedList<CustomerContact>> SearchCustomerContacts(ContactsPaginatedRequest requestData);
        Task<Customer> GetCustomer(int id);
        Task UpdateCustomer(Customer customer);
        Task<List<Product>> GetAvailableProducts(ProductList productList, string token);
        //Task<PaginatedList<Opportunity>> GetOpportunities(OpportunitiesPaginatedRequest requestData);
        Task<IEnumerable<Opportunity>> GetOp(OpportunitiesPaginatedRequest requestData, string cadena, string token);
        Task SaveOpportunityCommand(OpportunityPost opportunityPost, string token, Opportunity opportunity);

        Task<List<Opportunity>> GetOpportunietesTest(int userId);
        Task<IEnumerable<Opportunity>> GetOppByfilter(FilterOportunityModel filtro, string token);
        Task<Opportunity> GetOppById(int id);
        Task SaveOpportunityEdit(OpportunityPost send, int id, string token, Opportunity opportunity);
        Task<IEnumerable<PaymentCondition>> GetPaymentConditions(string token, int companyId);
        Task<OrderNote> CreateOrderNote(OrderNote nuevaOrder);
        Task<PaginatedList<OrderNote>> GetOrderNote(OrdersNotesPaginatedRequest requestData, string token);
        Task<PaginatedList<Sale>> GetSales(OrdersNotesPaginatedRequest requestData, string token);
        Task<IEnumerable<OrderNote>> GetOrdersByfilter(FilterOrderModel filtro, string token);
        Task<OrderNote> GetOrdersById(int id, string token);
    }
}
