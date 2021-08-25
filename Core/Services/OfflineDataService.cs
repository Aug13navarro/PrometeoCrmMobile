using Core.Model;
using Core.Model.Common;
using Core.Model.Extern;
using Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Core.Services
{
    public class OfflineDataService : IOfflineDataService
    {
        public bool IsWifiConection => Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
        public bool IsMobileConection => Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular);
        public bool IsDataLoaded { get; private set; }

        private readonly List<CustomerExtern> customerSearchCache = new List<CustomerExtern>();
        private readonly List<CompanyExtern> companySearchCache = new List<CompanyExtern>();
        private readonly List<PaymentConditionsExtern> paymentConditionsSearchCache = new List<PaymentConditionsExtern>();

        private const int MaxCustomerToSave = 5000;
        private const int MaxCompanyToSave = 15;
        private const int MaxPaymentConditions = 50;

        private const string CustomerSearchCacheFilename = "customersearchcache";
        private const string CompanySearchCacheFileNAme = "companysearchcache";
        private const string PaymentConditionsSearchCacheFilename = "paymentconditionssearchcache";

        public async Task LoadAllData()
        {
            try
            {
                await SynchronizeItemsToDisk(customerSearchCache, MaxCustomerToSave, CustomerSearchCacheFilename);
                await LoadData(customerSearchCache, CustomerSearchCacheFilename);

                IsDataLoaded = true;
            }
            catch (Exception e)
            {
                var str = e.Message;
                throw;
            }
        }

        private async Task LoadData<T>(List<T> itemsInCache, string cacheFilename)
        {
            await Task.Run(() =>
            {
                string filename = Path.Combine(FileSystem.CacheDirectory, cacheFilename);
                using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    var bf = new BinaryFormatter();

                    file.Position = 0;
                    var data = (List<T>)bf.Deserialize(file);
                    itemsInCache.AddRange(data);
                }
            });
        }

        private async Task SynchronizeItemsToDisk<T>(List<T> itemsInCache, int maxItemInCache, string cacheFilename)
        {
            if(itemsInCache.Count == 0)
            {
                return;
            }

            //string filename = Path.Combine(FileSystem.CacheDirectory, cacheFilename);
            //using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    var bf = new BinaryFormatter();
            //    List<T> cacheData = new List<T>();

            //    try
            //    {
            //        cacheData = (List<T>)bf.Deserialize(file);
            //    }
            //    catch (Exception)
            //    {
            //        cacheData = new List<T>();
            //    }

            //    List<T> itemsNotInCache = itemsInCache.Where(x => !cacheData.Contains(x)).ToList();

            //    if (itemsNotInCache.Count > 0)
            //    {
            //        if (cacheData.Count + itemsNotInCache.Count <= maxItemInCache)
            //        {
            //            cacheData.AddRange(itemsNotInCache);
            //        }
            //        else
            //        {
            //            cacheData.AddRange(itemsNotInCache);
            //            cacheData = cacheData.Skip(cacheData.Count - maxItemInCache).ToList();
            //        }

            //        file.Position = 0;
            //        bf.Serialize(file, cacheData);
            //    }
            //}

            //itemsInCache.Clear();

            await Task.Run(() =>
            {
                string filename = Path.Combine(FileSystem.CacheDirectory, cacheFilename);
                using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var bf = new BinaryFormatter();
                    List<T> cacheData;

                    try
                    {
                        file.Position = 0;
                        cacheData = (List<T>)bf.Deserialize(file);
                    }
                    catch (Exception e)
                    {
                        var str = e.Message;

                        cacheData = new List<T>();
                    }

                    List<T> itemsNotInCache = itemsInCache.Where(x => !cacheData.Contains(x)).ToList();

                    if (itemsNotInCache.Count > 0)
                    {
                        if (cacheData.Count + itemsNotInCache.Count <= maxItemInCache)
                        {
                            cacheData.AddRange(itemsNotInCache);
                        }
                        else
                        {
                            cacheData.AddRange(itemsNotInCache);
                            cacheData = cacheData.Skip(cacheData.Count - maxItemInCache).ToList();
                        }

                        file.Position = 0;
                        bf.Serialize(file, cacheData);
                    }
                }

                //itemsInCache.Clear();
            });
        }

        public void UnloadAllData()
        {
            customerSearchCache.Clear();
            companySearchCache.Clear();
            paymentConditionsSearchCache.Clear();

            IsDataLoaded = false;
        }

        public void SaveCustomerSearch(IList<Customer> customers)
        {
            var lista = new List<CustomerExtern>();

            foreach (var item in customers)
            {
                lista.Add(new CustomerExtern
                {
                    Abbreviature = item.Abbreviature,
                    AccountOwnerId = item.AccountOwnerId,
                    AccountOwnerName = item.AccountOwnerName,
                    BusinessName = item.BusinessName,
                    CompanyName = item.CompanyName,
                    Descriptions = item.Descriptions,
                    DollarBalance = item.DollarBalance,
                    ExternalId = item.ExternalId,
                    Id = item.Id,
                    IdParentCustomer = item.IdParentCustomer,
                    TaxCondition = item.TaxCondition,
                    IdNumber = item.IdNumber,
                    PesosBalance = item.PesosBalance,
                    TypeId = item.TypeId,
                    UnitBalance = item.UnitBalance,
                });
            }

            customerSearchCache.AddRange(lista);
        }

        public void SaveCompanySearch(List<Company> companies)
        {
            var lista = new List<CompanyExtern>();

            foreach (var item in companies)
            {
                lista.Add(new CompanyExtern
                {
                    BusinessName = item.BusinessName,
                    externalId = item.externalId,
                    Id = item.Id,
                });
            }

            companySearchCache.AddRange(lista);
        }

        public void SavePaymentConditions(List<PaymentCondition> paymentConditions)
        {
            var lista = new List<PaymentConditionsExtern>();

            foreach (var item in paymentConditions)
            {
                lista.Add(new PaymentConditionsExtern
                {
                    abbreviature = item.abbreviature,
                    active = item.active,
                    baseDate = item.baseDate,
                    code = item.code,
                    company = item.company,
                    companyId = item.companyId,
                    description = item.description,
                    id = item.id,
                    surcharge = item.surcharge,
                });
            }

            paymentConditionsSearchCache.AddRange(lista);
        }

        public async Task SynchronizeToDisk()
        {
            try
            {
                await SynchronizeItemsToDisk(customerSearchCache, MaxCustomerToSave, CustomerSearchCacheFilename);
                await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);
                await SynchronizeItemsToDisk(paymentConditionsSearchCache, MaxPaymentConditions, PaymentConditionsSearchCacheFilename);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAllData()
        {
            await Task.Run(() =>
            {
                try
                {
                    File.Delete(Path.Combine(FileSystem.CacheDirectory, CustomerSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.CacheDirectory, CompanySearchCacheFileNAme));
                    File.Delete(Path.Combine(FileSystem.CacheDirectory, PaymentConditionsSearchCacheFilename));
                }
                catch (Exception)
                {

                }
            });
        }
        public Task<List<Customer>> SearchCustomers()
        {
            //string criterio = string.IsNullOrWhiteSpace(requestData.Query) ? requestData.Query : requestData.Query.Replace("-", "").ToLower();

            //List<CustomerExtern> query = customerSearchCache.Where(x => string.IsNullOrWhiteSpace(criterio) ||
            //(!string.IsNullOrWhiteSpace(x.BusinessName))).ToList();

            //int totalCustomer = query.Count;

            //List<CustomerExtern> customers = query.Skip((requestData.CurrentPage - 1) * requestData.PageSize)
            //    .Take(requestData.PageSize)
            //    .ToList();

            //var result = new PaginatedList<CustomerExtern>(requestData.PageSize, requestData.CurrentPage, customers, totalCustomer)
            //{
            //    PageSize = requestData.PageSize,
            //    CurrentPage = requestData.CurrentPage,
            //    Results = customers,
            //    ResultsCount = (int)Math.Ceiling(totalCustomer / (double)requestData.PageSize),
            //    TotalCount = totalCustomer,
            //    TotalPages = customers.Count
            //};

            var lista = new List<Customer>();

            var fromCache = customerSearchCache;

            foreach (var item in fromCache)
            {
                lista.Add(new Customer
                {
                    Abbreviature = item.Abbreviature,
                    AccountOwnerId = item.AccountOwnerId,
                    AccountOwnerName = item.AccountOwnerName,
                    BusinessName = item.BusinessName,
                    CompanyName = item.CompanyName,
                    Descriptions = item.Descriptions,
                    DollarBalance = item.DollarBalance,
                    ExternalId = item.ExternalId,
                    Id = item.Id,
                    IdParentCustomer = item.IdParentCustomer,
                    TaxCondition = item.TaxCondition,
                    IdNumber = item.IdNumber,
                    PesosBalance = item.PesosBalance,
                    TypeId = item.TypeId,
                    UnitBalance = item.UnitBalance,
                });
            }

            return Task.FromResult(lista);
        }

        public Task<List<Company>> SearchCompanies()
        {
            var lista = new List<Company>();

            var fromCache = companySearchCache;

            foreach (var item in fromCache)
            {
                lista.Add(new Company
                {
                    BusinessName = item.BusinessName,
                    externalId = item.externalId,
                    Id = item.Id,
                });

            }

            return Task.FromResult(lista);
        }

        public Task<List<PaymentCondition>> SearchPaymentConditions()
        {
            var lista = new List<PaymentCondition>();

            var fromCache = paymentConditionsSearchCache;

            foreach (var item in fromCache)
            {
                lista.Add(new PaymentCondition
                {
                    abbreviature = item.abbreviature,
                    active = item.active,
                    baseDate = item.baseDate,
                    code = item.code,
                    company = item.company,
                    companyId = item.companyId,
                    description = item.description,
                    id = item.id,
                    surcharge = item.surcharge,
                });
            }

            return Task.FromResult(lista);
        }

        public async Task LoadDataPayment()
        {
            await SynchronizeItemsToDisk(paymentConditionsSearchCache, MaxPaymentConditions, PaymentConditionsSearchCacheFilename);

            await Task.Run(() =>
            {
                string filename = Path.Combine(FileSystem.CacheDirectory, PaymentConditionsSearchCacheFilename);
                using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    var bf = new BinaryFormatter();

                    file.Position = 0;
                    var data = (List<PaymentConditionsExtern>)bf.Deserialize(file);
                    paymentConditionsSearchCache.Clear();
                    paymentConditionsSearchCache.AddRange(data);
                }
            });
        }

        public async Task LoadCompanies()
        {
            await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);

            await Task.Run(() =>
            {
                string filename = Path.Combine(FileSystem.CacheDirectory, CompanySearchCacheFileNAme);
                using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    var bf = new BinaryFormatter();

                    file.Position = 0;
                    var data = (List<CompanyExtern>)bf.Deserialize(file);
                    companySearchCache.Clear();
                    companySearchCache.AddRange(data);
                }
            });
        }
    }
}
