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
        private readonly List<ProductExtern> presentationsSearchCache = new List<ProductExtern>();
        private readonly List<OpportunityExtern> opportunitiesSearchCache = new List<OpportunityExtern>();


        private const int MaxCustomerToSave = 5000;
        private const int MaxCompanyToSave = 15;
        private const int MaxPaymentConditions = 50;
        private const int MaxPresentations = 500;
        private const int MaxOportunities = 50;

        private const string CustomerSearchCacheFilename = "customersearchcache";
        private const string CompanySearchCacheFileNAme = "companysearchcache";
        private const string PaymentConditionsSearchCacheFilename = "paymentconditionssearchcache";
        private const string PresentationsSearchCacheFilename = "presentationssearchcache";
        private const string OpportunitiesSearchCacheFilename = "opportunitiessearchcache";

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

                itemsInCache.Clear();
            });
        }

        public void UnloadAllData(string tipo)
        {
            switch (tipo)
            {
                case "Customer":
                    customerSearchCache.Clear();
                    break;
                case "Company":
                    companySearchCache.Clear();
                    break;
                case "Payment":
                    paymentConditionsSearchCache.Clear();
                    break;
                case "Presentation":
                    presentationsSearchCache.Clear();
                    break;
                case "Opportunity":
                    opportunitiesSearchCache.Clear();
                    break;
            }


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

        public void SavePresentations(List<Product> products)
        {
            try
            {
                var lista = new List<ProductExtern>();

                foreach (var item in products)
                {
                    lista.Add(new ProductExtern
                    {
                        Discount = item.Discount,
                        Id = item.Id,
                        name = item.name,
                        price = item.price,
                        quantity = item.quantity,
                        stock = item.stock,

                    });
                }

                presentationsSearchCache.AddRange(lista);
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        public void SaveOpportunity(Opportunity opportunity)
        {
            try
            {
                var opportunityEx = new OpportunityExtern
                {
                    customer = opportunity.customer,
                    Details = opportunity.Details,
                    Id = opportunity.Id,
                    opportunityStatus = opportunity.opportunityStatus,
                    //ProductsDescription  = opportunity.ProductsDescription,
                    closedDate = opportunity.closedDate,
                    createDt = opportunity.createDt,
                    description = opportunity.description,
                    opportunityProducts = opportunity.Details,
                    totalPrice = opportunity.totalPrice,
                };

                var lista = new List<OpportunityExtern>();
                lista.Add(opportunityEx);

                opportunitiesSearchCache.AddRange(lista);
            }
            catch(Exception e)
            {
                var s = e.Message;
            }
        }


        public async Task SynchronizeToDisk()
        {
            try
            {
                await SynchronizeItemsToDisk(customerSearchCache, MaxCustomerToSave, CustomerSearchCacheFilename);
                await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);
                await SynchronizeItemsToDisk(paymentConditionsSearchCache, MaxPaymentConditions, PaymentConditionsSearchCacheFilename);
                await SynchronizeItemsToDisk(presentationsSearchCache, MaxPresentations, PresentationsSearchCacheFilename);
                await SynchronizeItemsToDisk(opportunitiesSearchCache, MaxOportunities, OpportunitiesSearchCacheFilename);
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
                    File.Delete(Path.Combine(FileSystem.CacheDirectory, PresentationsSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.CacheDirectory, OpportunitiesSearchCacheFilename));
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        #region SEARCH
        public Task<List<Customer>> SearchCustomers()
        {
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

        public Task<List<Product>> SearchPresentations()
        {
            try
            {
                var lista = new List<Product>();

                var fromCache = presentationsSearchCache;

                foreach (var item in fromCache)
                {
                    lista.Add(new Product
                    {
                        Discount = item.Discount,
                        Id = item.Id,
                        name = item.name,
                        price = item.price,
                        quantity = item.quantity,
                        stock = item.stock,
                    });
                }

                return Task.FromResult(lista);
            }
            catch(Exception e)
            {
                var s = e.Message;
                throw;
            }
        }

        public Task<List<Opportunity>> SearchOpportunities()
        {
            try
            {
                var lista = new List<Opportunity>();

                var fromCache = opportunitiesSearchCache;

                foreach (var opportunity in fromCache)
                {
                    lista.Add(new Opportunity
                    {
                        customer = opportunity.customer,
                        Details = opportunity.Details,
                        Id = opportunity.Id,
                        opportunityStatus = opportunity.opportunityStatus,
                        //ProductsDescription  = opportunity.ProductsDescription,
                        closedDate = opportunity.closedDate,
                        createDt = opportunity.createDt,
                        description = opportunity.description,
                        opportunityProducts = opportunity.opportunityProducts,
                        totalPrice = opportunity.totalPrice,
                    });
                }

                return Task.FromResult(lista);
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
        }
        #endregion

        #region LOAD

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

            IsDataLoaded = true;
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

            IsDataLoaded = true;
        }

        public async Task LoadPresentation()
        {
            try
            {
                await SynchronizeItemsToDisk(presentationsSearchCache, MaxPresentations, PresentationsSearchCacheFilename);

                await Task.Run(() =>
                {
                    string filename = Path.Combine(FileSystem.CacheDirectory, PresentationsSearchCacheFilename);
                    using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        var bf = new BinaryFormatter();

                        file.Position = 0;
                        var data = (List<ProductExtern>)bf.Deserialize(file);
                        presentationsSearchCache.Clear();
                        presentationsSearchCache.AddRange(data);
                    }
                });

                IsDataLoaded = true;
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        public async Task LoadOpportunities()
        {
            try
            {
                await SynchronizeItemsToDisk(opportunitiesSearchCache, MaxOportunities, OpportunitiesSearchCacheFilename);

                await Task.Run(() =>
                {
                    string filename = Path.Combine(FileSystem.CacheDirectory, OpportunitiesSearchCacheFilename);
                    using (Stream file = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read))
                    {
                        var bf = new BinaryFormatter();

                        file.Position = 0;
                        var data = (List<OpportunityExtern>)bf.Deserialize(file);
                        opportunitiesSearchCache.Clear();
                        opportunitiesSearchCache.AddRange(data);
                    }
                });


                IsDataLoaded = true;
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        #endregion
    }
}
