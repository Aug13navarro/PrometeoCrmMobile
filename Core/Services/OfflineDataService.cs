using Core.Model;
using Core.Model.Extern;
using Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Core.Model.Extern.OrderNoteExtern;
using static Core.Model.OrderNote;

namespace Core.Services
{
    public class OfflineDataService : IOfflineDataService
    {
        public bool IsDataLoaded { get; private set; }
        public bool IsDataLoadedCustomer { get; private set; }
        public bool IsDataLoadedCompanies { get; private set; }
        public bool IsDataLoadedPaymentConditions { get; private set; }
        public bool IsDataLoadedPresentations { get; private set; }
        public bool IsDataLoadedOpportunities { get; private set; }
        public bool IsDataLoadedOrderNote { get; private set; }
        public bool IsDataLoadedOpportunityStatus { get; private set; }
        public bool IsDataLoadedAssistant { get; private set; }
        public bool IsDataLoadedPaymentMethod { get; private set; }
        public bool IsDataLoadedIncoterms { get; private set; }
        public bool IsDataLoadedFreigths{ get; private set; }
        public bool IsDataLoadedTransports { get; private set; }

        private readonly List<CustomerExtern> customerSearchCache = new List<CustomerExtern>();
        private readonly List<CompanyExtern> companySearchCache = new List<CompanyExtern>();
        private readonly List<PaymentConditionsExtern> paymentConditionsSearchCache = new List<PaymentConditionsExtern>();
        private readonly List<ProductExtern> presentationsSearchCache = new List<ProductExtern>();
        private readonly List<OpportunityExtern> opportunitiesSearchCache = new List<OpportunityExtern>();
        private readonly List<OrderNoteExtern> orderNotesSearchCache = new List<OrderNoteExtern>();
        private readonly List<OpportunityStatusExtern> opportunityStatusSearchCache = new List<OpportunityStatusExtern>();
        private readonly List<UserExtern> assistantSearchCache = new List<UserExtern>();
        private readonly List<PaymentMethodExtern> paymentMethodSearchCache = new List<PaymentMethodExtern>();
        private readonly List<IncotermExtern> incotermsSearchCache = new List<IncotermExtern>();
        private readonly List<FreightInChargeExtern> freightsSearchCache = new List<FreightInChargeExtern>();
        private readonly List<TransportExtern> transportsSearchCache = new List<TransportExtern>();


        private const int MaxCustomerToSave = 5000;
        private const int MaxCompanyToSave = 15;
        private const int MaxPaymentConditions = 600;
        private const int MaxPresentations = 500;
        private const int MaxOportunities = 50;
        private const int MaxOrderNotes= 50;
        private const int MaxOppStatus = 10;
        private const int MaxAssistant = 10;
        private const int MaxPaymentMethod = 100;
        private const int MaxIncoterms= 20;
        private const int MaxFreights= 10;
        private const int MaxTransports= 5;


        private const string CustomerSearchCacheFilename = "customersearchcache";
        private const string CompanySearchCacheFileNAme = "companysearchcache";
        private const string PaymentConditionsSearchCacheFilename = "paymentconditionssearchcache";
        private const string PresentationsSearchCacheFilename = "presentationssearchcache";
        private const string OpportunitiesSearchCacheFilename = "opportunitiessearchcache";
        private const string OrderNotesSearchCacheFilename = "ordernotessearchcache";
        private const string OpportunityStatusSearchCacheFilename = "opportunitystatussearchcache";
        private const string AssistantSearchCacheFilename = "assistantsearchcache";
        private const string PaymentMethodSearchCacheFilename = "paymentMethodsearchcache";
        private const string IncotermsSearchCacheFilename = "incotermssearchcache";
        private const string FreightsSearchCacheFilename = "freightssearchcache";
        private const string TransportsSearchCacheFilename = "transportssearchcache";

        private async Task LoadData<T>(List<T> itemsInCache, string cacheFilename)
        {
            await Task.Run(() =>
            {
                string filename = Path.Combine(FileSystem.AppDataDirectory, cacheFilename);
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
                string filename = Path.Combine(FileSystem.AppDataDirectory, cacheFilename);
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

        public void UnloadAllData()
        {
            customerSearchCache.Clear();
            IsDataLoadedCustomer = false;

            companySearchCache.Clear();
            IsDataLoadedCompanies = false;

            paymentConditionsSearchCache.Clear();
            IsDataLoadedPaymentConditions = false;

            presentationsSearchCache.Clear();
            IsDataLoadedPresentations = false;

            opportunitiesSearchCache.Clear();
            IsDataLoadedOpportunities = false;

            orderNotesSearchCache.Clear();
            IsDataLoadedOrderNote = false;
                        
            assistantSearchCache.Clear();
            IsDataLoadedAssistant = false;

            paymentMethodSearchCache.Clear();
            IsDataLoadedPaymentMethod = false;

            incotermsSearchCache.Clear();
            IsDataLoadedIncoterms = false;

            freightsSearchCache.Clear();
            IsDataLoadedFreigths = false;

            transportsSearchCache.Clear();
            IsDataLoadedTransports = false;
        }

        public void UnloadOpportunities()
        {
            opportunityStatusSearchCache.Clear();
            IsDataLoadedOpportunityStatus = false;
        }

        #region SAVE

        public void SaveCustomerSearch(IList<CustomerExtern> customers)
        {
            customerSearchCache.AddRange(customers);
        }

        public void SaveCompanySearch(List<CompanyExtern> companies)
        {
            companySearchCache.AddRange(companies);
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

        public void SaveOpportunity(OpportunityExtern opportunity)
        {
            try
            {
                //var customerEx = new CustomerExtern
                //{
                //    Abbreviature = opportunity.customer.Abbreviature,
                //    AccountOwnerId = opportunity.customer.AccountOwnerId,
                //    AccountOwnerName = opportunity.customer.AccountOwnerName,
                //    BusinessName = opportunity.customer.BusinessName,
                //    CompanyName = opportunity.customer.CompanyName,
                //    Descriptions = opportunity.customer.Descriptions,
                //    DollarBalance = opportunity.customer.DollarBalance,
                //    externalCustomerId = opportunity.customer.externalCustomerId.Value,
                //    Id = opportunity.customer.Id,
                //    IdParentCustomer = opportunity.customer.IdParentCustomer,
                //    TaxCondition = opportunity.customer.TaxCondition,
                //    IdNumber = opportunity.customer.IdNumber,
                //    PesosBalance = opportunity.customer.PesosBalance,
                //    TypeId = opportunity.customer.TypeId,
                //    UnitBalance = opportunity.customer.UnitBalance,
                //};

                //var statusEx = new OpportunityStatusExtern
                //{
                //    Id = opportunity.opportunityStatus.Id,
                //    name = opportunity.opportunityStatus.name,
                //};

                //var detail = ConvertirDetailExtern(opportunity.Details.ToList());

                //var opportunityEx = new OpportunityExtern
                //{
                //    customer = customerEx,
                //    Details = detail,
                //    Id = opportunity.Id,
                //    opportunityStatus = statusEx,
                //    //ProductsDescription  = opportunity.ProductsDescription,
                //    closedDate = opportunity.closedDate,
                //    createDt = opportunity.createDt,
                //    description = opportunity.description,
                //    opportunityProducts = detail,
                //    totalPrice = opportunity.totalPrice,
                //};

                var lista = new List<OpportunityExtern>();
                lista.Add(opportunity);

                opportunitiesSearchCache.AddRange(lista);
            }
            catch(Exception e)
            {
                throw new Exception($"Error al guardar la Oportunidad, {e.Message}");
            }
        }

        public void SaveOrderNotes(OrderNote orderNote)
        {
            try
            {
                var comp = new CompanyExtern
                {
                    BusinessName = orderNote.company.BusinessName,
                    externalId = orderNote.company.externalId,
                    Id = orderNote.company.Id
                }; 
                
                var customerEx = new CustomerExtern
                {
                    Abbreviature = orderNote.customer.Abbreviature,
                    AccountOwnerId = orderNote.customer.AccountOwnerId.Value,
                    AccountOwnerName = orderNote.customer.AccountOwnerName,
                    BusinessName = orderNote.customer.BusinessName,
                    CompanyName = orderNote.customer.CompanyName,
                    Descriptions = orderNote.customer.Descriptions,
                    DollarBalance = orderNote.customer.DollarBalance,
                    externalCustomerId = orderNote.customer.externalCustomerId.Value,
                    Id = orderNote.customer.Id,
                    IdParentCustomer = orderNote.customer.IdParentCustomer,
                    TaxCondition = orderNote.customer.TaxCondition,
                    IdNumber = orderNote.customer.IdNumber,
                    PesosBalance = orderNote.customer.PesosBalance,
                    TypeId = orderNote.customer.TypeId,
                    UnitBalance = orderNote.customer.UnitBalance,
                };

                var prod = ConvertirProductsExtern(orderNote.products.ToList());

                var orderExtern = new OrderNoteExtern
                {
                    company = comp,
                    companyId = comp.Id,
                    cuenta = orderNote.cuenta,
                    currencyId = orderNote.currencyId,
                    customer = customerEx,
                    customerId = customerEx.Id,
                    DeliveryAddress = orderNote.DeliveryAddress,
                    DeliveryDate = orderNote.DeliveryDate,
                    DeliveryResponsible = orderNote.DeliveryResponsible,
                    Description = orderNote.Description,
                    discount = orderNote.discount,
                    divisionCuentaId = orderNote.divisionCuentaId,
                    fecha = orderNote.fecha,
                    numero = orderNote.numero,
                    OCCustomer = orderNote.OCCustomer,
                    orderStatus = orderNote.orderStatus,
                    paymentConditionId = orderNote.paymentConditionId,
                    PlacePayment = orderNote.PlacePayment,
                    RemittanceType = orderNote.RemittanceType,
                    talon = orderNote.talon,
                    tipoComprobante = orderNote.tipoComprobante,
                    tipoCuentaId = orderNote.tipoCuentaId,
                    tipoServicioId = orderNote.tipoServicioId,
                    total = orderNote.total,
                    products = prod,
                };

                var lista = new List<OrderNoteExtern>();

                lista.Add(orderExtern);

                orderNotesSearchCache.AddRange(lista);
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        public void SaveOpportunityStatus(List<OpportunityStatusExtern> opportunityStatuses)
        {
            opportunityStatusSearchCache.AddRange(opportunityStatuses);
        }

        public void SaveAssitant(List<UserExtern> userExterns)
        {
            assistantSearchCache.AddRange(userExterns);
        }

        public void SavePaymentMethod(List<PaymentMethodExtern> paymentMethodExterns)
        {
            paymentMethodSearchCache.AddRange(paymentMethodExterns);
        }

        public void SaveIncoterms(List<IncotermExtern> incotermExterns)
        {
            incotermsSearchCache.AddRange(incotermExterns);
        }

        public void SaveFreights(List<FreightInChargeExtern> freightInChargeExterns)
        {
            freightsSearchCache.AddRange(freightInChargeExterns);
        }

        public void SaveTransports(List<TransportExtern> transportExterns)
        {
            transportsSearchCache.AddRange(transportExterns);
        }


        private List<ProductOrderExtern> ConvertirProductsExtern(List<ProductOrder> products)
        {
            var lista = new List<ProductOrderExtern>();

            foreach (var item in products)
            {
                var prod = new ProductExtern
                {
                    name = item.companyProductPresentation.name
                };

                var product = new ProductOrderExtern
                {
                    arancel = item.arancel,
                    bonificacion = item.bonificacion,
                    companyProductPresentation = prod,
                    companyProductPresentationId = item.companyProductPresentationId,
                    discount = item.discount,
                    price = item.price,
                    quantity = item.quantity,
                    subtotal = item.subtotal,
                };

                lista.Add(product);
            }

            return lista;
        }

        private List<OpportunityProductsExtern> ConvertirDetailExtern(List<OpportunityProducts> opportunityProducts)
        {
            var lista = new List<OpportunityProductsExtern>();

            foreach (var item in opportunityProducts)
            {
                var product = new ProductExtern
                {
                    Discount = item.product.Discount,
                    Id = item.product.Id,
                    name = item.product.name,
                    price = item.product.price,
                    quantity = item.product.quantity,
                    stock = item.product.stock
                };

                lista.Add(new OpportunityProductsExtern
                {
                    CompanyId = item.CompanyId,
                    Discount = item.Discount,
                    Price = item.Price,
                    product = product,
                    Quantity = item.Quantity,
                    productId = item.productId,
                    Total = item.Total
                });
            }

            return lista;
        }

        #endregion

        public async Task SynchronizeToDisk()
        {
            try
            {
                await SynchronizeItemsToDisk(customerSearchCache, MaxCustomerToSave, CustomerSearchCacheFilename);
                await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);
                await SynchronizeItemsToDisk(paymentConditionsSearchCache, MaxPaymentConditions, PaymentConditionsSearchCacheFilename);
                await SynchronizeItemsToDisk(presentationsSearchCache, MaxPresentations, PresentationsSearchCacheFilename);                
                await SynchronizeItemsToDisk(orderNotesSearchCache, MaxOrderNotes, OrderNotesSearchCacheFilename);
                await SynchronizeItemsToDisk(opportunityStatusSearchCache, MaxOppStatus, OpportunitiesSearchCacheFilename);
                await SynchronizeItemsToDisk(assistantSearchCache, MaxAssistant, AssistantSearchCacheFilename);
                await SynchronizeItemsToDisk(paymentMethodSearchCache, MaxPaymentMethod, PaymentMethodSearchCacheFilename);
                await SynchronizeItemsToDisk(incotermsSearchCache, MaxIncoterms, IncotermsSearchCacheFilename);
                await SynchronizeItemsToDisk(freightsSearchCache, MaxFreights, FreightsSearchCacheFilename);
                await SynchronizeItemsToDisk(transportsSearchCache, MaxTransports, TransportsSearchCacheFilename);

                IsDataLoaded = true;
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
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, CustomerSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, CompanySearchCacheFileNAme));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, PaymentConditionsSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, PresentationsSearchCacheFilename));
                    //File.Delete(Path.Combine(FileSystem.AppDataDirectory, OpportunitiesSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, OrderNotesSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, OpportunityStatusSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, AssistantSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, PaymentMethodSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, IncotermsSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, FreightsSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, TransportsSearchCacheFilename));

                    IsDataLoaded = false;
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        public async Task SynchronizeToDiskOpportunity()
        {
            await SynchronizeItemsToDisk(opportunitiesSearchCache, MaxOportunities, OpportunitiesSearchCacheFilename);
            IsDataLoaded = true;
        }

        public async Task DeleteOpportunities()
        {
            await Task.Run(() =>
            {
                try
                {
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, OpportunitiesSearchCacheFilename));
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        #region SEARCH
        public Task<List<CustomerExtern>> SearchCustomers()
        {
            var fromCache = customerSearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<CompanyExtern>> SearchCompanies()
        {
            var fromCache = companySearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<PaymentConditionsExtern>> SearchPaymentConditions()
        {
            var fromCache = paymentConditionsSearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<ProductExtern>> SearchPresentations()
        {
            try
            {
                var fromCache = presentationsSearchCache;

                return Task.FromResult(fromCache);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<List<OpportunityExtern>> SearchOpportunities()
        {
            try
            {
                var fromCache = opportunitiesSearchCache;

                //foreach (var opportunity in fromCache)
                //{
                //    var customer = new Customer
                //    {
                //        Abbreviature = opportunity.customer.Abbreviature,
                //        AccountOwnerId = opportunity.customer.AccountOwnerId,
                //        AccountOwnerName = opportunity.customer.AccountOwnerName,
                //        BusinessName = opportunity.customer.BusinessName,
                //        CompanyName = opportunity.customer.CompanyName,
                //        Descriptions = opportunity.customer.Descriptions,
                //        DollarBalance = opportunity.customer.DollarBalance,
                //        externalCustomerId = opportunity.customer.externalCustomerId,
                //        Id = opportunity.customer.Id,
                //        IdParentCustomer = opportunity.customer.IdParentCustomer,
                //        TaxCondition = opportunity.customer.TaxCondition,
                //        IdNumber = opportunity.customer.IdNumber,
                //        PesosBalance = opportunity.customer.PesosBalance,
                //        TypeId = opportunity.customer.TypeId,
                //        UnitBalance = opportunity.customer.UnitBalance,
                //    };

                //    var status = new OpportunityStatus
                //    {
                //        Id = opportunity.opportunityStatus.Id,
                //        name = opportunity.opportunityStatus.name,
                //    };

                //    var detail = ConvertirExternDetail(opportunity.Details);

                //    lista.Add(new Opportunity
                //    {
                //        customer = customer,
                //        Details = new MvvmCross.ViewModels.MvxObservableCollection<OpportunityProducts>(detail),
                //        Id = opportunity.Id,
                //        opportunityStatus = status,
                //        //ProductsDescription  = opportunity.ProductsDescription,
                //        closedDate = opportunity.closedDate,
                //        createDt = opportunity.createDt,
                //        description = opportunity.description,
                //        opportunityProducts = new MvvmCross.ViewModels.MvxObservableCollection<OpportunityProducts>(detail),
                //        totalPrice = opportunity.totalPrice,
                //    });
                //}

                return Task.FromResult(fromCache);
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
        }

        public Task<List<OrderNote>> SearchOrderNotes()
        {
            try
            {
                var lista = new List<OrderNote>();

                var fromCache = orderNotesSearchCache;

                foreach (var item in fromCache)
                {
                    var comp = new Company
                    {
                        BusinessName = item.company.BusinessName,
                        externalId = item.company.externalId,
                        Id = item.company.Id
                    };

                    var cust = new Customer
                    {
                        Abbreviature = item.customer.Abbreviature,
                        AccountOwnerId = item.customer.AccountOwnerId,
                        AccountOwnerName = item.customer.AccountOwnerName,
                        BusinessName = item.customer.BusinessName,
                        CompanyName = item.customer.CompanyName,
                        Descriptions = item.customer.Descriptions,
                        DollarBalance = item.customer.DollarBalance,
                        externalCustomerId = item.customer.externalCustomerId,
                        Id = item.customer.Id,
                        IdParentCustomer = item.customer.IdParentCustomer,
                        TaxCondition = item.customer.TaxCondition,
                        IdNumber = item.customer.IdNumber,
                        PesosBalance = item.customer.PesosBalance,
                        TypeId = item.customer.TypeId,
                        UnitBalance = item.customer.UnitBalance,
                    };

                    var productsList = ConvertirExternProduct(item.products);

                    var paymentMethod = new PaymentMethod
                    {
                        id = item.paymentMethod.id,
                        name = item.paymentMethod.name,
                    };

                    var freight = new FreightInCharge
                    {
                        id = item.FreightInCharge.id,
                        name = item.FreightInCharge.name,
                    };

                    lista.Add(new OrderNote
                    {
                        company = comp,
                        companyId = comp.Id,
                        cuenta = item.cuenta,
                        currencyId = item.currencyId,
                        customer = cust,
                        customerId = cust.Id,
                        DeliveryAddress = item.DeliveryAddress,
                        DeliveryDate = item.DeliveryDate,
                        DeliveryResponsible = item.DeliveryResponsible,
                        Description = item.Description,
                        discount = item.discount,
                        divisionCuentaId = item.divisionCuentaId,
                        fecha = item.fecha,
                        numero = item.numero,
                        OCCustomer = item.OCCustomer,
                        orderStatus = item.orderStatus,
                        paymentConditionId = item.paymentConditionId,
                        PlacePayment = item.PlacePayment,
                        RemittanceType = item.RemittanceType,
                        talon = item.talon,
                        tipoComprobante = item.tipoComprobante,
                        tipoCuentaId = item.tipoCuentaId,
                        tipoServicioId = item.tipoServicioId,
                        total = item.total,
                        products = new MvvmCross.ViewModels.MvxObservableCollection<ProductOrder>(productsList),
                        paymentMethod = paymentMethod,
                        PaymentMethodId = paymentMethod.id,
                        FreightInCharge = freight,
                        FreightInChargeId = freight.id,
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

        public Task<List<OpportunityStatusExtern>> SearchOpportunityStatuses()
        {
            var fromCache = opportunityStatusSearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<UserExtern>> SearchAssistant()
        {
            var fromCache = assistantSearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<PaymentMethodExtern>> SearchPaymentMethod()
        {
            var fromCache = paymentMethodSearchCache;

            return Task.FromResult(fromCache);
        }
        
        public Task<List<IncotermExtern>> SearchIncoterms()
        {
            var fromCache = incotermsSearchCache;

            return Task.FromResult(fromCache);
        }

        public Task<List<FreightInChargeExtern>> SearchFreights()
        {
            var fromCache = freightsSearchCache;

            return Task.FromResult(fromCache);
        }
        
        public Task<List<TransportExtern>> SearchTransports()
        {
            var fromCache = transportsSearchCache;

            return Task.FromResult(fromCache);
        }


        private List<ProductOrder> ConvertirExternProduct(List<OrderNoteExtern.ProductOrderExtern> products)
        {
            var lista = new List<ProductOrder>();

            foreach (var item in products)
            {
                var prod = new Product
                {
                    name = item.companyProductPresentation.name
                };

                lista.Add(new ProductOrder
                {
                    arancel = item.arancel,
                    bonificacion = item.bonificacion,
                    companyProductPresentation = prod,
                    companyProductPresentationId = item.companyProductPresentationId,
                    discount = item.discount,
                    price = item.price,
                    quantity = item.quantity,
                    subtotal = item.subtotal,
                });
            }

            return lista;
        }

        private List<OpportunityProducts> ConvertirExternDetail(List<OpportunityProductsExtern> details)
        {
            var lista = new List<OpportunityProducts>();

            foreach (var item in details)
            {
                var product = new Product
                {
                    Discount = item.product.Discount,
                    Id = item.product.Id,
                    name = item.product.name,
                    price = item.product.price,
                    quantity = item.product.quantity,
                    stock = item.product.stock
                };

                lista.Add(new OpportunityProducts
                {
                    CompanyId = item.CompanyId,
                    Discount = item.Discount,
                    Price = item.Price,
                    product = product,
                    Quantity = item.Quantity,
                    productId = item.productId,
                    Total = item.Total
                });
            }

            return lista;
        }
        #endregion

        #region LOAD
        public async Task LoadAllData()
        {
            try
            {
                await SynchronizeItemsToDisk(customerSearchCache, MaxCustomerToSave, CustomerSearchCacheFilename);

                    await LoadData(customerSearchCache, CustomerSearchCacheFilename);
                    IsDataLoadedCustomer = true;
            }
            catch (Exception e)
            {
                var str = e.Message;
            }
        }

        public async Task LoadDataPayment()
        {
            await SynchronizeItemsToDisk(paymentConditionsSearchCache, MaxPaymentConditions, PaymentConditionsSearchCacheFilename);

                await LoadData(paymentConditionsSearchCache, PaymentConditionsSearchCacheFilename);

                IsDataLoadedPaymentConditions = true;
        }

        public async Task LoadCompanies()
        {
            try
            {
                await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);

                    await LoadData(companySearchCache, CompanySearchCacheFileNAme);

                    IsDataLoadedCompanies = true;
            }
            catch (Exception e)
            {
                var s = e.Message;
            }
        }

        public async Task LoadPresentation()
        {
            try
            {
                await SynchronizeItemsToDisk(presentationsSearchCache, MaxPresentations, PresentationsSearchCacheFilename);

                    await LoadData(presentationsSearchCache, PresentationsSearchCacheFilename);

                    IsDataLoadedPresentations = true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error en Presentations Offline, {e.Message}");
            }
        }

        public async Task LoadOpportunities()
        {
            try
            {
                await SynchronizeItemsToDisk(opportunitiesSearchCache, MaxOportunities, OpportunitiesSearchCacheFilename);

                    await LoadData(opportunitiesSearchCache, OpportunitiesSearchCacheFilename);

                    IsDataLoadedOpportunities = true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error en Opportunity Offline, {e.Message}");
            }
        }

        public async Task LoadOrderNotes()
        {
            try
            {
                await SynchronizeItemsToDisk(orderNotesSearchCache, MaxOrderNotes, OrderNotesSearchCacheFilename);

                    await LoadData(orderNotesSearchCache, OrderNotesSearchCacheFilename);

                    IsDataLoadedOrderNote = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error en Order Notes Offline");
            }
        }

        public async Task LoadOpportunityStatus()
        {
            try
            {
                await SynchronizeItemsToDisk(opportunityStatusSearchCache, MaxOppStatus, OpportunityStatusSearchCacheFilename);
                await LoadData(opportunityStatusSearchCache, OpportunityStatusSearchCacheFilename);

                IsDataLoadedOpportunityStatus = true;
            }
            catch ( Exception e)
            {
                throw new Exception("Error en Opportunity Status Offline");
            }
        }

        public async Task LoadAssistant()
        {
            try
            {
                await SynchronizeItemsToDisk(assistantSearchCache, MaxAssistant, AssistantSearchCacheFilename);
                await LoadData(assistantSearchCache, AssistantSearchCacheFilename);

                IsDataLoadedAssistant = true;
            }
            catch (Exception e)

            {
                throw new Exception("Error en Assistant Offline");
            }
        }

        public async Task LoadPaymentMethod()
        {
            try
            {
                await SynchronizeItemsToDisk(paymentMethodSearchCache, MaxPaymentMethod, PaymentMethodSearchCacheFilename);
                await LoadData(paymentMethodSearchCache, PaymentMethodSearchCacheFilename);

                IsDataLoadedPaymentMethod = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en Payment Method Offline");
            }
        }
        
        public async Task LoadIncoterms()
        {
            try
            {
                await SynchronizeItemsToDisk(incotermsSearchCache, MaxIncoterms, IncotermsSearchCacheFilename);
                await LoadData(incotermsSearchCache, IncotermsSearchCacheFilename);

                IsDataLoadedIncoterms = true;
            }
            catch 
            {
                throw new Exception("Error en Incoterms Offline");
            }
        }

        public async Task LoadFreights()
        {
            try
            {
                await SynchronizeItemsToDisk(freightsSearchCache, MaxFreights, FreightsSearchCacheFilename);
                await LoadData(freightsSearchCache, FreightsSearchCacheFilename);

                IsDataLoadedFreigths = true;
            }
            catch
            {
                throw new Exception("Error en Freigts Offline");
            }
        }

        public async Task LoadTransports()
        {
            try
            {
                await SynchronizeItemsToDisk(transportsSearchCache, MaxTransports, TransportsSearchCacheFilename);
                await LoadData(transportsSearchCache, TransportsSearchCacheFilename);

                IsDataLoadedTransports = true;
            }
            catch
            {
                throw new Exception("Error en Transports Offline");
            }
        }
        #endregion
    }
}
