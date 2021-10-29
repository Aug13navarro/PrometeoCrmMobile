﻿using Core.Model;
using Core.Model.Extern;
using Core.Services.Contracts;
using MvvmCross.ViewModels;
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
        public bool IsWifiConection => Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
        public bool IsMobileConection => Connectivity.ConnectionProfiles.Contains(ConnectionProfile.Cellular);

        public bool IsDataLoadesCustomer { get; private set; }
        public bool IsDataLoadesCompanies { get; private set; }
        public bool IsDataLoadesPaymentConditions { get; private set; }
        public bool IsDataLoadesPresentations { get; private set; }
        public bool IsDataLoadesOpportunities { get; private set; }
        public bool IsDataLoadesOrderNote { get; private set; }

        private readonly List<CustomerExtern> customerSearchCache = new List<CustomerExtern>();
        private readonly List<CompanyExtern> companySearchCache = new List<CompanyExtern>();
        private readonly List<PaymentConditionsExtern> paymentConditionsSearchCache = new List<PaymentConditionsExtern>();
        private readonly List<ProductExtern> presentationsSearchCache = new List<ProductExtern>();
        private readonly List<OpportunityExtern> opportunitiesSearchCache = new List<OpportunityExtern>();
        private readonly List<OrderNoteExtern> orderNotesSearchCache = new List<OrderNoteExtern>();


        private const int MaxCustomerToSave = 5000;
        private const int MaxCompanyToSave = 15;
        private const int MaxPaymentConditions = 600;
        private const int MaxPresentations = 500;
        private const int MaxOportunities = 50;
        private const int MaxOrderNotes= 50;

        private const string CustomerSearchCacheFilename = "customersearchcache";
        private const string CompanySearchCacheFileNAme = "companysearchcache";
        private const string PaymentConditionsSearchCacheFilename = "paymentconditionssearchcache";
        private const string PresentationsSearchCacheFilename = "presentationssearchcache";
        private const string OpportunitiesSearchCacheFilename = "opportunitiessearchcache";
        private const string OrderNotesSearchCacheFilename = "ordernotessearchcache";

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

        public void UnloadAllData(string tipo)
        {
            switch (tipo)
            {
                case "Customer":
                    customerSearchCache.Clear();
                    IsDataLoadesCustomer = false;
                    break;
                case "Company":
                    companySearchCache.Clear();
                    IsDataLoadesCompanies = false;
                    break;
                case "Payment":
                    paymentConditionsSearchCache.Clear();
                    IsDataLoadesPaymentConditions = false;
                    break;
                case "Presentation":
                    presentationsSearchCache.Clear();
                    IsDataLoadesPresentations = false;
                    break;
                case "Opportunity":
                    opportunitiesSearchCache.Clear();
                    IsDataLoadesOpportunities = false;
                    break;
                case "OrderNotes":
                    orderNotesSearchCache.Clear();
                    IsDataLoadesOrderNote = false;
                    break;
            }
        }

        #region SAVE

        public void SaveCustomerSearch(IList<CustomerExtern> customers)
        {
            //var lista = new List<CustomerExtern>();

            //foreach (var item in customers)
            //{
            //    lista.Add(new CustomerExtern
            //    {
            //        Abbreviature = item.Abbreviature,
            //        AccountOwnerId = item.AccountOwnerId,
            //        AccountOwnerName = item.AccountOwnerName,
            //        BusinessName = item.BusinessName,
            //        CompanyName = item.CompanyName,
            //        Descriptions = item.Descriptions,
            //        DollarBalance = item.DollarBalance,
            //        ExternalId = item.ExternalId,
            //        Id = item.Id,
            //        IdParentCustomer = item.IdParentCustomer,
            //        TaxCondition = item.TaxCondition,
            //        IdNumber = item.IdNumber,
            //        PesosBalance = item.PesosBalance,
            //        TypeId = item.TypeId,
            //        UnitBalance = item.UnitBalance,
            //    });
            //}

            customerSearchCache.AddRange(customers);
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


                var customerEx = new CustomerExtern
                {
                    Abbreviature = opportunity.customer.Abbreviature,
                    AccountOwnerId = opportunity.customer.AccountOwnerId,
                    AccountOwnerName = opportunity.customer.AccountOwnerName,
                    BusinessName = opportunity.customer.BusinessName,
                    CompanyName = opportunity.customer.CompanyName,
                    Descriptions = opportunity.customer.Descriptions,
                    DollarBalance = opportunity.customer.DollarBalance,
                    ExternalId = opportunity.customer.ExternalId,
                    Id = opportunity.customer.Id,
                    IdParentCustomer = opportunity.customer.IdParentCustomer,
                    TaxCondition = opportunity.customer.TaxCondition,
                    IdNumber = opportunity.customer.IdNumber,
                    PesosBalance = opportunity.customer.PesosBalance,
                    TypeId = opportunity.customer.TypeId,
                    UnitBalance = opportunity.customer.UnitBalance,
                };

                var statusEx = new OpportunityStatusExtern
                {
                    Id = opportunity.opportunityStatus.Id,
                    name = opportunity.opportunityStatus.name,
                };

                var detail = ConvertirDetailExtern(opportunity.Details.ToList());

                var opportunityEx = new OpportunityExtern
                {
                    customer = customerEx,
                    Details = detail,
                    Id = opportunity.Id,
                    opportunityStatus = statusEx,
                    //ProductsDescription  = opportunity.ProductsDescription,
                    closedDate = opportunity.closedDate,
                    createDt = opportunity.createDt,
                    description = opportunity.description,
                    opportunityProducts = detail,
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
                    AccountOwnerId = orderNote.customer.AccountOwnerId,
                    AccountOwnerName = orderNote.customer.AccountOwnerName,
                    BusinessName = orderNote.customer.BusinessName,
                    CompanyName = orderNote.customer.CompanyName,
                    Descriptions = orderNote.customer.Descriptions,
                    DollarBalance = orderNote.customer.DollarBalance,
                    ExternalId = orderNote.customer.ExternalId,
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
                await SynchronizeItemsToDisk(opportunitiesSearchCache, MaxOportunities, OpportunitiesSearchCacheFilename);
                await SynchronizeItemsToDisk(orderNotesSearchCache, MaxOrderNotes, OrderNotesSearchCacheFilename);
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
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, OpportunitiesSearchCacheFilename));
                    File.Delete(Path.Combine(FileSystem.AppDataDirectory, OrderNotesSearchCacheFilename));
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
                    var customer = new Customer
                    {
                        Abbreviature = opportunity.customer.Abbreviature,
                        AccountOwnerId = opportunity.customer.AccountOwnerId,
                        AccountOwnerName = opportunity.customer.AccountOwnerName,
                        BusinessName = opportunity.customer.BusinessName,
                        CompanyName = opportunity.customer.CompanyName,
                        Descriptions = opportunity.customer.Descriptions,
                        DollarBalance = opportunity.customer.DollarBalance,
                        ExternalId = opportunity.customer.ExternalId,
                        Id = opportunity.customer.Id,
                        IdParentCustomer = opportunity.customer.IdParentCustomer,
                        TaxCondition = opportunity.customer.TaxCondition,
                        IdNumber = opportunity.customer.IdNumber,
                        PesosBalance = opportunity.customer.PesosBalance,
                        TypeId = opportunity.customer.TypeId,
                        UnitBalance = opportunity.customer.UnitBalance,
                    };

                    var status = new OpportunityStatus
                    {
                        Id = opportunity.opportunityStatus.Id,
                        name = opportunity.opportunityStatus.name,
                    };

                    var detail = ConvertirExternDetail(opportunity.Details);

                    lista.Add(new Opportunity
                    {
                        customer = customer,
                        Details = new MvvmCross.ViewModels.MvxObservableCollection<OpportunityProducts>(detail),
                        Id = opportunity.Id,
                        opportunityStatus = status,
                        //ProductsDescription  = opportunity.ProductsDescription,
                        closedDate = opportunity.closedDate,
                        createDt = opportunity.createDt,
                        description = opportunity.description,
                        opportunityProducts = new MvvmCross.ViewModels.MvxObservableCollection<OpportunityProducts>(detail),
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
                        ExternalId = item.customer.ExternalId,
                        Id = item.customer.Id,
                        IdParentCustomer = item.customer.IdParentCustomer,
                        TaxCondition = item.customer.TaxCondition,
                        IdNumber = item.customer.IdNumber,
                        PesosBalance = item.customer.PesosBalance,
                        TypeId = item.customer.TypeId,
                        UnitBalance = item.customer.UnitBalance,
                    };

                    var productsList = ConvertirExternProduct(item.products);

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
                        products = new MvvmCross.ViewModels.MvxObservableCollection<ProductOrder>(productsList)
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

                IsDataLoadesCustomer = true;
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

            IsDataLoadesPaymentConditions = true;
        }

        public async Task LoadCompanies()
        {
            try
            {
                await SynchronizeItemsToDisk(companySearchCache, MaxCompanyToSave, CompanySearchCacheFileNAme);
                await LoadData(companySearchCache, CompanySearchCacheFileNAme);

                IsDataLoadesCompanies = true;
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

                IsDataLoadesPresentations = true;
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
                await LoadData(opportunitiesSearchCache, OpportunitiesSearchCacheFilename);

                IsDataLoadesOpportunities = true;
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

                IsDataLoadesOrderNote = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error en Order Notes Offline");
            }
        }

        #endregion
    }
}
