using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Model;
using Core.Model.Common;
using Core.Model.Enums;
using Core.Model.Extern;
using Core.Services.Contracts;
using Core.Services.Exceptions;
using Core.Services.Utils;
using Core.ViewModels.Model;
using Newtonsoft.Json;

namespace Core.Services
{
    public class PrometeoApiService : IPrometeoApiService
    {
        private readonly HttpClient client;
        private readonly IOfflineDataService offlineDataService;

        public PrometeoApiService(HttpClient client, IOfflineDataService offlineDataService)
        {
            this.client = client;
            this.offlineDataService = offlineDataService;
        }

        public async Task<LoginData> Login(string userName, string password)
        {
            //const string url = "api/User/Login";
            //const string url = "http://testing-prometeo.docworld.com.ar:8089/api/User/Login";
            const string url = "https://neophos-testing-api.azurewebsites.net/api/User/Login";
            // const string url = "https://prometeoerp.com/login";
            var body = new
            {
                Login = userName,
                Password = password,
                //Password = password,
            };

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                using (var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"))
                {
                    request.Content = content;
                    LoginData result = await client.SendAsyncAs<LoginData>(request);

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserData> GetUserData(int userId)
        {
            // string url = $"api/User/{userId}";
            string url = $"https://neophos-testing-api.azurewebsites.net/api/User/{userId}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<UserData> result = await client.SendAsyncAs<List<UserData>>(request);

                if (result.Count > 0)
                    return result[0];
                else
                    return null;
            }
        }

        public async Task<List<Opportunity>> GetOpportunietesTest(int userId)
        {
            // string url = $"api/User/{userId}";
            try
            {
                string url = $"https://neophos-testing-api.azurewebsites.net/api/Opportunity/{userId}";

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    List<Opportunity> result = await client.SendAsyncAs<List<Opportunity>>(request);

                    if (result.Count > 0)
                        return result;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Assignment> GetAssignment(int id)
        {
            string url = $"api/Assignment/Get?id={id}&pointApplicationId=0";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                Assignment result = await client.SendAsyncAs<Assignment>(request);
                return result;
            }
        }

        public async Task<PaginatedList<Assignment>> SearchAssignment(SearchAssignmentRequest requestData)
        {
            const string url = "api/Assignment/Search";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                PaginatedList<Assignment> result = await client.SendAsyncAs<PaginatedList<Assignment>>(request);

                return result;
            }
        }

        public async Task<PaginatedList<TreatmentAlert>> GetAllNotifications(NotificationsPaginatedRequest requestData)
        {
            string url = $"api/Middleware/GetAllNotifications?grdId={requestData.GrdId}" +
                         $"&currentPage={requestData.CurrentPage}&pageSize={requestData.PageSize}&viewed={requestData.Viewed}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                PaginatedList<TreatmentAlert> result = await client.SendAsyncAs<PaginatedList<TreatmentAlert>>(request);
                return result;
            }
        }

        public async Task SetViewedNotification(int notificationId)
        {
            string url = $"api/Middleware/SetViewedNotification?historialId={notificationId}&grdId=3";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                await client.SendAsync(request);
            }
        }

        public async Task<PaginatedList<Customer>> GetCustomers(CustomersPaginatedRequest requestData)
        {
            //if (offlineDataService.IsWifiConection)
            //{
            const string url = "api/Customer/GetList";
            var body = new
            {
                requestData.UserId,
                requestData.CurrentPage,
                requestData.PageSize,
                requestData.Query,
            };
            
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"))
            {
                //string req = await content.ReadAsStringAsync();
                request.Content = content;
                PaginatedList<Customer> result = await client.SendAsyncAs<PaginatedList<Customer>>(request);
            
                if(offlineDataService.IsDataLoaded)
                {
                    offlineDataService.UnloadAllData("Customer");
                }
            
                offlineDataService.SaveCustomerSearch(result.Results);
            
                return result;
            }
            //}
            //else
            //{
            //    if(!offlineDataService.IsDataLoaded)
            //    {
            //        await offlineDataService.LoadAllData();
            //    }

            //    //var result = await offlineDataService.SearchPolicies(requestData);
            //    //var customers = ConvertToPaginatedCustomers(result);

            //    return new PaginatedList<Customer>();
            //}
        }

        public async Task<List<Customer>> GetAllCustomer(int userId, bool isParent, int typeCustomer, string token)
        {
            try
            {
                if(offlineDataService.IsWifiConection)
                {
                    var lista = new List<Customer>();

                    string url = $"api/Customer?idUser={userId}&companyId={0}&isParentCustomer=true&customerTypeId={typeCustomer}";
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"{url}");
                    var resultado = await response.Content.ReadAsStringAsync();

                    if(!string.IsNullOrWhiteSpace(resultado))
                    {
                        lista = JsonConvert.DeserializeObject<List<Customer>>(resultado);
                        
                        if (offlineDataService.IsDataLoaded)
                        {
                            offlineDataService.UnloadAllData("Customer");
                        }

                        offlineDataService.SaveCustomerSearch(lista);
                    }

                    return lista;
                }
                else
                {
                    if(!offlineDataService.IsDataLoaded)
                    {
                        await offlineDataService.LoadAllData();
                    }

                    var result = await offlineDataService.SearchCustomers();

                    return result;
                }
            }
            catch (Exception e )
            {
                var s = e.Message;
                throw;
            }
        }

        private PaginatedList<Customer> ConvertToPaginatedCustomers(PaginatedList<CustomerExtern> results)
        {
            var resultBase = results.Results;

            var clientes = ConvertCustomers(results.Results);

            return new PaginatedList<Customer>(results.PageSize, results.CurrentPage, clientes, results.TotalCount)
            {
                CurrentPage = results.CurrentPage,
                Results = clientes,
                PageSize = results.PageSize,
                ResultsCount = results.ResultsCount,
                TotalCount = results.TotalCount,
                TotalPages = results.TotalPages,
            };
        }

        private List<Customer> ConvertCustomers(List<CustomerExtern> results)
        {
            var listaClientes = new List<Customer>();

            foreach (var item in results)
            {
                listaClientes.Add(new Customer
                {
                    Id = item.Id,
                    Abbreviature = item.Abbreviature,
                    AccountOwnerId = item.AccountOwnerId,
                    AccountOwnerName = item.AccountOwnerName,
                    BusinessName = item.BusinessName,
                    CompanyName = item.CompanyName,
                });
            }

            return listaClientes;
        }

        public async Task<List<Customer>> GetCustomersOld(CustomersOldRequest requestData)
        {
            string url = $"api/Customer?query={requestData.Query}&&idUser={requestData.UserId}&&companyId={requestData.CompanyId}" +
                         $"&&isParentCustomer={requestData.IsParentCustomer}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<Customer> result = await client.SendAsyncAs<List<Customer>>(request);
                return result;
            }
        }

        public async Task CreateCustomer(Customer requestData)
        {
            const string url = "api/Customer";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"))
            {
                //string req = await content.ReadAsStringAsync();
                request.Content = content;
                HttpResponseMessage result = await client.SendAsync(request);

                if (!result.IsSuccessStatusCode)
                {
                    if (result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        using (Stream stream = await result.Content.ReadAsStreamAsync())
                        {
                            var errors = HttpClientExtensionMethods.DeserializeJsonFromStream<Dictionary<string, string[]>>(stream);
                            string firstError = errors.First().Value[0];

                            throw new ServiceException(firstError);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public async Task<List<CustomerType>> GetCustomerTypes()
        {
            const string url = "api/Customer/CustomerType";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<CustomerType> result = await client.SendAsyncAs<List<CustomerType>>(request);
                return result;
            }
        }

        public async Task<List<DocumentType>> GetDocumentTypes()
        {
            const string url = "api/Customer/DocumentType";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<DocumentType> result = await client.SendAsyncAs<List<DocumentType>>(request);
                return result;
            }
        }

        public async Task<List<TaxCondition>> GetTaxConditions()
        {
            const string url = "api/Company/TaxCondition";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<TaxCondition> result = await client.SendAsyncAs<List<TaxCondition>>(request);
                return result;
            }
        }

        public async Task<List<Company>> GetCompaniesByUserId(int userId, string token)
        {
            try
            {
                if (offlineDataService.IsWifiConection)
                {

                    string url = $"/api/Company/GetCompanyByUserId/{userId}";

                    var lista = new List<Company>();

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"{url}");

                    var resultado = await response.Content.ReadAsStringAsync();

                    if (resultado != null)
                    {
                        lista = JsonConvert.DeserializeObject<IEnumerable<Company>>(resultado).ToList();

                        if(offlineDataService.IsDataLoaded)
                        {
                            offlineDataService.UnloadAllData("Company");
                        }

                        offlineDataService.SaveCompanySearch(lista);
                    }

                    return lista;
                }
                else
                {
                    if(!offlineDataService.IsDataLoaded)
                    {
                        await offlineDataService.LoadCompanies();
                    }

                    var result = await offlineDataService.SearchCompanies();

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<PaginatedList<CustomerContact>> SearchCustomerContacts(ContactsPaginatedRequest requestData)
        {
            const string url = "api/Customer/SearchCustomerContacts";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                PaginatedList<CustomerContact> result = await client.SendAsyncAs<PaginatedList<CustomerContact>>(request);

                return result;
            }
        }

        public async Task<Customer> GetCustomer(int id)
        {
            string url = $"api/Customer/{id}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                Customer result = await client.SendAsyncAs<Customer>(request);
                return result;
            }
        }

        public async Task UpdateCustomer(Customer customer)
        {
            string url = $"api/Customer/{customer.Id}";

            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            using (var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json"))
            {
                //string req = await content.ReadAsStringAsync();
                request.Content = content;
                HttpResponseMessage result = await client.SendAsync(request);

                if (!result.IsSuccessStatusCode)
                {
                    if (result.StatusCode == HttpStatusCode.BadRequest)
                    {
                        using (Stream stream = await result.Content.ReadAsStreamAsync())
                        {
                            var errors = HttpClientExtensionMethods.DeserializeJsonFromStream<Dictionary<string, string[]>>(stream);
                            string firstError = errors.First().Value[0];

                            throw new ServiceException(firstError);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public async Task<List<Product>> GetAvailableProducts(ProductList productList, string token)
        {
            try
            {
                if (offlineDataService.IsWifiConection)
                {
                    string url = $"/api/Product/GetCompanyProductPresentationByUserIdAsync";

                    //var content = JsonConvert.SerializeObject(productList);

                    //HttpContent httpContent = new StringContent(content, Encoding.UTF8);

                    //httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var objeto = await client.GetAsync(string.Format(url));

                    var resultado = await objeto.Content.ReadAsStringAsync();

                    var lista = JsonConvert.DeserializeObject<List<Product>>(resultado);

                    if(offlineDataService.IsDataLoaded)
                    {
                        offlineDataService.UnloadAllData("Presentation");
                    }

                    offlineDataService.SavePresentations(lista);

                    return lista;
                }
                else
                {
                    //if(!offlineDataService.IsDataLoaded)
                    //{
                    //    await offlineDataService.LoadPresentation();
                    //}

                    await offlineDataService.LoadPresentation();

                    var result = await offlineDataService.SearchPresentations();

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Opportunity>> GetOp(OpportunitiesPaginatedRequest requestData,string cadena, string token)
        {
            try
            {
                if (offlineDataService.IsWifiConection)
                {
                    var lista = new List<Opportunity>();

                    var content = JsonConvert.SerializeObject(requestData);

                    HttpContent httpContent = new StringContent(content);

                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetStringAsync($"{cadena}");

                    if (response != null)
                    {
                        lista = JsonConvert.DeserializeObject<IEnumerable<Opportunity>>(response).ToList();
                        
                        //if (offlineDataService.IsDataLoaded)
                        //{
                        //    offlineDataService.UnloadAllData("Opportunity");
                        //}
                    }

                    return lista;
                }
                else
                {
                    await offlineDataService.LoadOpportunities();
                    var result = await offlineDataService.SearchOpportunities();

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveOpportunityCommand(OpportunityPost opportunityPost, string token, Opportunity opportunity)
        {
            if (offlineDataService.IsWifiConection)
            {
                var cadena = "https://neophos-testing-api.azurewebsites.net/api/Opportunity";

                //var dto = new
                //{
                //    customerId = opportunity.customer.Id,
                //    branchOfficeId = 1,
                //    opportunityStatusId = opportunity.opportunityStatus.Id,
                //    opportunityProducts = opportunity.Details,
                //    totalPrice = opportunity.totalPrice,
                //    closedDate = opportunity.closedDate,
                //    description = opportunity.description
                //};

                var objeto = JsonConvert.SerializeObject(opportunityPost);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

                var resultado = await respuesta.Content.ReadAsStringAsync();

                //if (opportunity.Id == 0)
                //{
                //    opportunity.Id = Opportunities.Count == 0 ? 1 : Opportunities.Max(o => o.Id) + 1;
                //}

                //Opportunities.Add(JsonConvert.DeserializeObject<Opportunity>(resultado));
                await Task.FromResult(0);
            }
            else
            {
                if (offlineDataService.IsDataLoaded)
                {
                    await offlineDataService.LoadOpportunities();
                }

                opportunity.totalPrice = Convert.ToDecimal(opportunityPost.totalPrice);
                opportunity.opportunityStatus.Id = opportunityPost.opportunityStatusId;
                offlineDataService.SaveOpportunity(opportunity);
            }
        }

        public async Task<IEnumerable<Opportunity>> GetOppByfilter(FilterOportunityModel filtro, string token)
        {
            try
            {
                var url = $"https://neophos-testing-api.azurewebsites.net/api/Opportunity/SearchOpportunityAsync";

                var dto = JsonConvert.SerializeObject(filtro);

                HttpContent httpContent = new StringContent(dto, Encoding.UTF8);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await respuesta.Content.ReadAsStringAsync();

                var lista = JsonConvert.DeserializeObject<List<Opportunity>>(resultado);

                return lista;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Opportunity> GetOppById(int id)
        {
            try
            {
                Opportunity opp = new Opportunity();
                
                var url = $"/api/Opportunity/{id}";

                var respuesta = await client.GetStringAsync(url);

                if(respuesta != null)
                {
                    opp = JsonConvert.DeserializeObject<Opportunity>(respuesta);
                }

                return opp;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SaveOpportunityEdit(OpportunityPost send, int id, string token, Opportunity opportunity)
        {
            var cadena = $"api/Opportunity?id={id}";

            var objeto = JsonConvert.SerializeObject(send);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PutAsync(string.Format(cadena), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            await Task.FromResult(0);
        }

        public async Task<IEnumerable<PaymentCondition>> GetPaymentConditions(string token)
        {
            try
            {
                if (offlineDataService.IsWifiConection)
                {

                    var lista = new List<PaymentCondition>();

                    var url = $"/api/PaymentCondition/GetAllPaymentTermsAsync";

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var respuesta = await client.GetAsync(string.Format(url));

                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(resultado))
                    {
                        lista = JsonConvert.DeserializeObject<List<PaymentCondition>>(resultado);

                        if(offlineDataService.IsDataLoaded)
                        {
                            offlineDataService.UnloadAllData("Payment");
                        }

                        offlineDataService.SavePaymentConditions(lista.ToList());
                    }

                    return lista;
                }
                else
                {
                    if (!offlineDataService.IsDataLoaded)
                    {
                        await offlineDataService.LoadDataPayment();
                    }

                    var result = await offlineDataService.SearchPaymentConditions();

                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderNote> CreateOrderNote(OrderNote nuevaOrder)
        {
            var cadena = "/api/Opportunity/CreateOpportunityOrderNoteAsync";

            var objeto = JsonConvert.SerializeObject(nuevaOrder);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            await Task.FromResult(0);

            return JsonConvert.DeserializeObject<OrderNote>(resultado);
        }

        public async Task<PaginatedList<OrderNote>> GetOrderNote(OrdersNotesPaginatedRequest requestData, string token)
        {
            var cadena = "/api/Opportunity/SearchOpportunityOrderNoteAsync";

            var objeto = JsonConvert.SerializeObject(requestData);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            await Task.FromResult(0);

            return JsonConvert.DeserializeObject<PaginatedList<OrderNote>>(resultado);
        }

        public Task<PaginatedList<Sale>> GetSales(OrdersNotesPaginatedRequest requestData, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderNote>> GetOrdersByfilter(FilterOrderModel filtro, string token)
        {
            try
            {
                var url = $"/api/Opportunity/SearchOpportunityOrderNoteByFiltersAsync";

                var dto = JsonConvert.SerializeObject(filtro);

                HttpContent httpContent = new StringContent(dto, Encoding.UTF8);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await respuesta.Content.ReadAsStringAsync();

                var lista = JsonConvert.DeserializeObject<List<OrderNote>>(resultado);

                return lista;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderNote> GetOrdersById(int id, string token)
        {
            try
            {
                var url = $"/api/Opportunity/GetOpportunityOrderNoteByIdAsync?id={id}";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.GetAsync($"{url}");

                var resultado = await respuesta.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<OrderNote>(resultado);
            }
            catch
            {
                throw;
            }
        }
    }
}
