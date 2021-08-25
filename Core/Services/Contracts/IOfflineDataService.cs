using Core.Model;
using Core.Model.Common;
using Core.Model.Extern;
using System;
using System.Collections.Generic;
using System.Text;
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
        void UnloadAllData();
        Task SynchronizeToDisk();
        Task DeleteAllData();

        void SaveCustomerSearch(IList<Customer> policies);
        void SaveCompanySearch(List<Company> companies);
        void SavePaymentConditions(List<PaymentCondition> paymentConditions);

        Task<List<Customer>> SearchCustomers();
        Task<List<Company>> SearchCompanies();
        Task<List<PaymentCondition>> SearchPaymentConditions();
    }
}
