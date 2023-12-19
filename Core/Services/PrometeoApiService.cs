using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Helpers;
using Core.Model;
using Core.Model.Common;
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
        //private readonly IOfflineDataService offlineDataService;

        public PrometeoApiService(HttpClient client)//, IOfflineDataService offlineDataService
        {
            this.client = client;
            //this.offlineDataService = offlineDataService;
        }

        public async Task<LoginData> Login(string userName, string password)
        {
            const string url = "api/User/Login";
            var body = new
            {
                Login = userName,
                Password = password,
            };

            try
            {
                var objeto = JsonConvert.SerializeObject(body);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var respuesta = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await respuesta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginData>(resultado);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserData> GetUserData(int userId)
        {
            string url = $"api/User/{userId}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                UserData result = await client.SendAsyncAs<UserData>(request);

                return result;
            }
        }

        public async Task<List<Opportunity>> GetOpportunietesTest(int userId)
        {
            try
            {
                string url = $"api/Opportunity/{userId}";

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
            try
            {
                const string url = "api/Customer/GetList";
                var body = new
                {
                    requestData.UserId,
                    requestData.CurrentPage,
                    requestData.PageSize,
                    requestData.Query,
                };

                var objeto = JsonConvert.SerializeObject(body);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var respuesta = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await respuesta.Content.ReadAsStringAsync();

                var r = JsonConvert.DeserializeObject<PaginatedList<Customer>>(resultado);

                return r;
            }
            catch(Exception e)
            {
                var m = e.Message;
                return null;
            }
        }

        public async Task<List<Customer>> GetAllCustomer(int userId, bool isParent, int typeCustomer, string token, int companyId)
        {
            try
            {
                var lista = new List<Customer>();

                string url = $"api/Customer?idUser={userId}&companyId={companyId}&isParentCustomer={isParent}&customerTypeId={typeCustomer}";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{url}");
                var resultado = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(resultado))
                {
                    lista = JsonConvert.DeserializeObject<List<Customer>>(resultado);

                }

                return lista;
            }
            catch (Exception e)
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

        public async Task<Customer> CreateCustomer(Customer requestData)
        {
            const string url = "api/Customer";


            var objeto = JsonConvert.SerializeObject(requestData);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var respuesta = await client.PostAsync(string.Format(url), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            if (respuesta.ReasonPhrase == "Bad Request")
            {
                throw new Exception(resultado);
            }

            if (respuesta.ReasonPhrase == "Internal Server Error")
            {
                throw new Exception("Error al Impactar en un Servicio Externo, posiblemente el Cliente no se encuentra registrado");
            }

            return JsonConvert.DeserializeObject<Customer>(resultado);
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
                string url = $"/api/Company/GetCompanyByUserId/{userId}";

                var lista = new List<Company>();

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{url}");

                var resultado = await response.Content.ReadAsStringAsync();

                if (resultado != null)
                {
                    lista = JsonConvert.DeserializeObject<IEnumerable<Company>>(resultado).ToList();
                }

                return lista;
            }
            catch
            {
                throw new Exception("Error en el Servicio Web");
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

        public async Task<PaginatedList<Product>> GetAvailableProducts(ProductList productList, string token)
        {
            try
            {
                string url = $"api/Product/SearchCompanyProductPresentationCanBeSold";

                var content = JsonConvert.SerializeObject(productList);

                HttpContent httpContent = new StringContent(content);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var objeto = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await objeto.Content.ReadAsStringAsync();

                var lista = JsonConvert.DeserializeObject<PaginatedList<Product>>(resultado);

                return lista;
            }
            catch (Exception e)
            {
                var s = e.Message;
                throw;
            }
        }

        public async Task<IEnumerable<Opportunity>> GetOp(OpportunitiesPaginatedRequest requestData, string lang, string token)
        {
            try
            {
                var url = $"/api/Opportunity/GetListByCustomerIdAsync?language={lang}";

                var lista = new List<Opportunity>();

                var content = JsonConvert.SerializeObject(requestData);

                HttpContent httpContent = new StringContent(content);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetStringAsync($"{url}");

                if (response != null)
                {
                    lista = JsonConvert.DeserializeObject<IEnumerable<Opportunity>>(response).ToList();
                }

                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveOpportunityCommand(OpportunityPost opportunityPost, string token, Opportunity opportunity)
        {
            var cadena = "api/Opportunity";


            var objeto = JsonConvert.SerializeObject(opportunityPost);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

            if (respuesta.ReasonPhrase == "Bad Request" || respuesta.ReasonPhrase == "Internal Server Error")
            {
                var result = await respuesta.Content.ReadAsStringAsync();
                return false;
            }

            var resultado = await respuesta.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<IEnumerable<Opportunity>> GetOppByfilter(FilterOportunityModel filtro, string lang, string token)
        {
            try
            {
                var url = $"api/Opportunity/SearchOpportunityAsync?language={lang}";

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

                if (respuesta != null)
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

        public async Task<IEnumerable<PaymentCondition>> GetPaymentConditions(string token, int companyId)
        {
            try
            {
                var lista = new List<PaymentCondition>();

                var url = $"/api/PaymentCondition/GetPaymentTermsByCompanyIdAsync?companyId={companyId}";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.GetAsync(url);

                var resultado = await respuesta.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(resultado))
                {
                    lista = JsonConvert.DeserializeObject<List<PaymentCondition>>(resultado);

                }

                return lista;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderNote> CreateOrderNote(OrderNote nuevaOrder)
        {
            try
            {
                var cadena = "/api/OpportunityOrderNote/CreateOpportunityOrderNoteAsync";

                var objeto = JsonConvert.SerializeObject(nuevaOrder);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<OrderNote>(resultado);
                }
                else
                {
                    throw new Exception("Tenemos problemas para procesar su solicitud. Contacte al administrador del sistema");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Tenemos problemas para procesar su solicitud - {e.Message}");
            }
        }

        public async Task<OrderNote> UpdateOrderNote(OrderNote order, string token)
        {
            try
            {
                var url = $"/api/OpportunityOrderNote/UpdateOpportunityOrderNoteAsync?id={order.id}";

                var objeto = JsonConvert.SerializeObject(order);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PutAsync(string.Format(url), httpContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<OrderNote>(resultado);
                }
                else
                {
                    throw new Exception("Tenemos problemas para procesar su solicitud. Contacte al administrador del sistema");
                }

            }
            catch (Exception e)
            {
                throw new Exception($"Tenemos problemas para procesar su solicitud - {e.Message}");
            }

        }

        public async Task<PaginatedList<OrderNote>> GetOrderNote(OrdersNotesPaginatedRequest requestData, string token)
        {
            var cadena = "/api/OpportunityOrderNote/SearchOpportunityOrderNoteAsync";

            var objeto = JsonConvert.SerializeObject(requestData);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            await Task.FromResult(0);

            return JsonConvert.DeserializeObject<PaginatedList<OrderNote>>(resultado);
        }


        public async Task<IEnumerable<OrderNote>> GetOrdersByfilter(FilterOrderModel filtro, string token)
        {
            try
            {
                var url = $"/api/OpportunityOrderNote/SearchOpportunityOrderNoteByFiltersAsync";

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
                var url = $"/api/OpportunityOrderNote/GetOpportunityOrderNoteByIdAsync?id={id}";

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

        public async Task<IEnumerable<OpportunityStatus>> GetOpportunityStatus(string lang, string token)
        {
            try
            {
                var url = $"/api/OpportunityStatus?language={lang}";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.GetAsync($"{url}");

                var resultado = await respuesta.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<OpportunityStatus>>(resultado);
            }
            catch
            {
                throw new Exception("Error");
            }
        }

        public async Task<string> RecoverPassword(string mail)
        {
            string url = $"/api/User/RecoverPassword?email={mail}";

            var response = await client.GetAsync($"{url}");

            var respuesta = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<string>(respuesta);

        }

        public async Task<IEnumerable<User>> GetUsersByRol(int companyId, string rol)
        {
            try
            {
                var url = $"/api/User/GetUserByRolName?name={rol}&companyId={companyId}";

                var respuesta = await client.GetAsync($"{url}");

                var resultado = await respuesta.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<User>>(resultado);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRolUserVending(string token, string rol)
        {
            try
            {
                var url = $"/api/User/GetUsersByVendingType?alias={rol}";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var respuesta = await client.GetAsync($"{url}");

                var resultado = await respuesta.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<User>>(resultado);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethod(int companyId, string language, string token)
        {
            var url = $"/api/PaymentMethod?companyId={companyId}&language={language}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<PaymentMethod>>(resultado);
        }

        public async Task<IEnumerable<Incoterm>> GetIncoterms(string token)
        {
            var url = $"/api/Incoterm";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<Incoterm>>(resultado);
        }

        public async Task<IEnumerable<FreightInCharge>> GetFreight(string language, string token)
        {
            var url = $"/api/Freight?language={language}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<FreightInCharge>>(resultado);
        }

        public async Task<IEnumerable<TransportCompany>> GetTransport(string language, int companyId, string token)
        {
            var url = $"/api/Transport?language={language}&companyId={companyId}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<TransportCompany>>(resultado);
        }
        public async Task<IEnumerable<Provider>> GetProvidersByType(int providerTypeId, string token)
        {
            var url = $"/api/ExpenseProvider/GetListByProviderTypeId?id={providerTypeId}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<Provider>>(resultado);
        }

        public async Task<User> SetCompany(int companyId, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("user-device", "mobile");

                var getData = await client.GetStringAsync($"/api/User/SetCurrentCompany?companyId={companyId}");

                if (getData != null)
                {
                    return JsonConvert.DeserializeObject<User>(getData);
                }

                return null;

            }
            catch (Exception e)
            {
                var m = e.Message;
                throw;
            }
        }

        public async Task<DataMobileModel> GetAllDataMobile(string language, string token)
        {
            var url = $"/api/DataMobile/GetAllDataToMobile?language={language}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DataMobileModel>(resultado);
        }

        public async Task<IEnumerable<StatusOrderNote>> GetStatusOrderNote(string token)
        {
            var url = $"/api/OpportunityOrderNote/GetStatusOrderNote";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<StatusOrderNote>>(resultado);
        }

        public async Task<OrderNote> UpdateStatusOrderNote(int orderNoteId, int statusId, string token)
        {
            var url = $"/api/OpportunityOrderNote/ChangeStatusOfOpportunityOrderNote?orderNoteId={orderNoteId}&statusId={statusId}";

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"{url}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OrderNote>(resultado);
        }

        public async Task<List<User>> GetUserListByCompanyId(string cadena, string token)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"/api/{cadena}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<User>>(resultado);
        }

        public async Task<List<Deposit>> GetDeposits(string cadena, string token)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.GetAsync($"/api/{cadena}");

            var resultado = await respuesta.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Deposit>>(resultado);
        }
    }
}