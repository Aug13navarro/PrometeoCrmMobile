using Core.Model.Extern;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data.Tables;

namespace Core.Data
{
    public static class OfflineDatabase
    {
        public static List<IncotermExtern> GetNoteAsync(int id)
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<IncotermExtern>().ToList();
            }
        }

        public static void SaveIncotermListAsync(List<IncotermTable> incoterms)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in incoterms)
                {
                    // Save 
                    conn.Insert(item);

                }
            }
        }
        public static List<IncotermTable> GetIncoterms()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<IncotermTable>().ToList();
            }
        }

        public static void SaveProviderListAsync(List<ProviderTable> providers)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in providers)
                {
                    var entidad = conn.Table<ProviderTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<ProviderTable> GetProviders()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<ProviderTable>().ToList();
            }
        }
        public static void SavePaymentMethodListAsync(List<PaymentMethodTable> providers)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in providers)
                {
                    var entidad = conn.Table<PaymentMethodTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<PaymentMethodTable> GetPaymentMethod()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<PaymentMethodTable>().ToList();
            }
        }
        public static void SaveTransportCompaniesListAsync(List<TransportCompanyTable> transportCompanies)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in transportCompanies)
                {
                    var entidad = conn.Table<PaymentMethodTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<TransportCompanyTable> GetTransportCompany()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<TransportCompanyTable>().ToList();
            }
        }
        public static void SaveAssistantComercialsListAsync(List<AssistantComercialTable> assistantComercials)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in assistantComercials)
                {
                    var entidad = conn.Table<AssistantComercialTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<AssistantComercialTable> GetAssistantComercial()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<AssistantComercialTable>().ToList();
            }
        }
        public static void SaveConditionListAsync(List<PaymentConditionTable> paymentConditions)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in paymentConditions)
                {
                    var entidad = conn.Table<PaymentConditionTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<PaymentConditionTable> GetPaymentCondition()
        {
            // Get
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<PaymentConditionTable>().ToList();
            }
        }
        public static void SaveFreightInChargesListAsync(List<FreightInChargeTable> incoterms)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in incoterms)
                {
                    var entidad = conn.Table<FreightInChargeTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }

                }
            }
        }
        public static List<FreightInChargeTable> GetFreightInCharges()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<FreightInChargeTable>().ToList();
            }
        }
        public static void SaveCompaniesListAsync(List<CompanyTable> companies)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in companies)
                {
                    var entidad = conn.Table<CompanyTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<CompanyTable> GetCompanies()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<CompanyTable>().ToList();
            }
        }
        public static void SaveCustomerListAsync(List<CustomerTable> customers)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in customers)
                {
                    var entidad = conn.Table<CustomerTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<CustomerTable> GetCustomers()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<CustomerTable>().ToList();
            }
        }

        public static void SaveProductsListAsync(List<ProductTable> products)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                foreach (var item in products)
                {
                    var entidad = conn.Table<ProductTable>().FirstOrDefault(p => p.Id == item.Id);
                    if (entidad == null)
                    {
                        // Save 
                        conn.Insert(item);
                    }
                }
            }
        }
        public static List<ProductTable> GetProducts()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<ProductTable>().ToList();
            }
        }

        public static async Task<bool> SaveOrderNote(OrderNoteTable orderNoteTable)
        {
            using (var conn = DataBaseHelper.GetConnection())
            {
                // Save 
                conn.Insert(orderNoteTable);
            }
            return await Task.FromResult(true);
        }

        public static List<OrderNoteTable> GetOrderNotes()
        {
            // Get 
            using (var conn = DataBaseHelper.GetConnection())
            {
                return conn.Table<OrderNoteTable>().ToList();
            }
        }
    }
}
