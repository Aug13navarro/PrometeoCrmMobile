using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Model;
using Core.Model.Common;

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
        Task<List<Customer>> GetCustomersOld(CustomersOldRequest requestData);
        Task CreateCustomer(Customer requestData);
        Task<List<CustomerType>> GetCustomerTypes();
        Task<List<DocumentType>> GetDocumentTypes();
        Task<List<TaxCondition>> GetTaxConditions();
        Task<List<Company>> GetCompaniesByUserId(int userId);
        Task<PaginatedList<CustomerContact>> SearchCustomerContacts(ContactsPaginatedRequest requestData);
        Task<Customer> GetCustomer(int id);
        Task UpdateCustomer(Customer customer);
        Task<ProductList> GetAvailableProducts(ProductList productList);
        Task<PaginatedList<Opportunity>> GetOpportunities(OpportunitiesPaginatedRequest requestData);
        Task SaveOpportunityCommand(Opportunity opportunity);

        Task<List<Opportunity>> GetOpportunietesTest(int userId);
    }
}
