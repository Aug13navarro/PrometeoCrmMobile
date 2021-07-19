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
using Core.Services.Contracts;
using Core.Services.Exceptions;
using Core.Services.Utils;
using Newtonsoft.Json;

namespace Core.Services
{
    public class PrometeoApiService : IPrometeoApiService
    {
        private readonly HttpClient client;

        private static readonly List<Product> Products = new List<Product>()
        {
            new Product() {Id = 1, name = "Producto 1", price = 2500, stock = 100, Discount = 0},
            new Product() {Id = 2, name = "Producto 2", price = 3600, stock = 250, Discount = 0},
            new Product() {Id = 3, name = "Producto 3", price = 5000, stock = 1000, Discount = 0},
            new Product() {Id = 4, name = "Producto 4", price = 150, stock = 25, Discount = 0},
        };

        private static readonly List<Opportunity> Opportunities = new List<Opportunity>()
        {
            //new Opportunity()
            //{
            //    Id = 1,
            //    Customer = new Customer() {Id = 2773, CompanyName = "COFCO BALCARCE"},
            //    Date = DateTime.Now.AddDays(-5),
            //    Description = "Oportunidad de ejemplo 1",
            //    Status = OpportunityStatus.Analysis,
            //    Details =
            //    {
            //        new OpportunityDetail()
            //        {
            //            Id = 1, Description = Products[0].name, Price = Products[0].price,
            //            Discount = 0, Quantity = 1, ProductId = Products[0].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 2, Description = Products[1].name, Price = Products[1].price,
            //            Discount = 10, Quantity = 5, ProductId = Products[1].Id,
            //        },
            //    }
            //},
            //new Opportunity()
            //{
            //    Id = 2,
            //    Customer = new Customer() {Id = 2773, CompanyName = "COFCO BALCARCE"},
            //    Date = DateTime.Now.AddDays(-1),
            //    Description = "Oportunidad de ejemplo 2",
            //    Status = OpportunityStatus.Negotiation,
            //    Details =
            //    {
            //        new OpportunityDetail()
            //        {
            //            Id = 1, Description = Products[0].name, Price = Products[0].price,
            //            Discount = 0, Quantity = 1, ProductId = Products[0].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 2, Description = Products[1].name, Price = Products[1].price,
            //            Discount = 10, Quantity = 5, ProductId = Products[1].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 3, Description = Products[2].name, Price = Products[2].price,
            //            Discount = 25, Quantity = 10, ProductId = Products[2].Id,
            //        },
            //    }
            //},
            //new Opportunity()
            //{
            //    Id = 3,
            //    Customer = new Customer() {Id = 2773, CompanyName = "COFCO BALCARCE"},
            //    Date = DateTime.Now.AddDays(-10),
            //    Description = "Oportunidad de ejemplo 3",
            //    Status = OpportunityStatus.Proposition,
            //    Details =
            //    {
            //        new OpportunityDetail()
            //        {
            //            Id = 1, Description = Products[0].name, Price = Products[0].price,
            //            Discount = 0, Quantity = 8, ProductId = Products[0].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 2, Description = Products[1].name, Price = Products[1].price,
            //            Discount = 10, Quantity = 3, ProductId = Products[1].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 3, Description = Products[2].name, Price = Products[2].price,
            //            Discount = 40, Quantity = 2, ProductId = Products[2].Id,
            //        },
            //    }
            //},
            //new Opportunity()
            //{
            //    Id = 4,
            //    Customer = new Customer() {Id = 2773, CompanyName = "COFCO BALCARCE"},
            //    Date = DateTime.Now.AddDays(-10),
            //    Description = "Oportunidad de ejemplo 4",
            //    Status = OpportunityStatus.Proposition,
            //    Details =
            //    {
            //        new OpportunityDetail()
            //        {
            //            Id = 1, Description = Products[0].name, Price = Products[0].price,
            //            Discount = 0, Quantity = 8, ProductId = Products[0].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 2, Description = Products[1].name, Price = Products[1].price,
            //            Discount = 10, Quantity = 3, ProductId = Products[1].Id,
            //        },
            //        new OpportunityDetail()
            //        {
            //            Id = 3, Description = Products[2].name, Price = Products[2].price,
            //            Discount = 40, Quantity = 2, ProductId = Products[2].Id,
            //        },
            //    }
            //}
        };

        public PrometeoApiService(HttpClient client)
        {
            this.client = client;
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

                return result;
            }
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

        public async Task<List<Company>> GetCompaniesByUserId(int userId)
        {
            string url = $"api/Company/GetCompanyByUserId/{userId}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                List<Company> result = await client.SendAsyncAs<List<Company>>(request);
                return result;
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

        public async Task<ProductList> GetAvailableProducts(ProductList productList)
        {
            try
            {
                string url = $"/api/Product/SearchCompanyProductPresentation";

                var content = JsonConvert.SerializeObject(productList);

                HttpContent httpContent = new StringContent(content, Encoding.UTF8);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var objeto = await client.PostAsync(string.Format(url), httpContent);

                var resultado = await objeto.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ProductList>(resultado);

            }
            catch (Exception)
            {
                throw;
            }



            //var products = new List<Product>()
            //{
            //    new Product() {Id = 1, Description = "Producto 1", Price = 2500, Stock = 25},
            //    new Product() {Id = 2, Description = "Producto 2", Price = 5000, Stock = 10},
            //    new Product() {Id = 3, Description = "Producto 3", Price = 1200, Stock = 5},
            //    new Product() {Id = 4, Description = "Producto 4", Price = 1500, Stock = 3},
            //};

            //return Task.FromResult(products);
        }

        public Task<PaginatedList<Opportunity>> GetOpportunities(OpportunitiesPaginatedRequest requestData)
        {
            try
            {
                List<Opportunity> opportunities = Opportunities
                                                  .Where(o => string.IsNullOrWhiteSpace(requestData.Query) ||
                                                              o.descripction.ToLower().Contains(requestData.Query.ToLower()))
                                                  .Skip(requestData.CurrentPage - 1 * requestData.PageSize).Take(requestData.PageSize).ToList();

                return Task.FromResult(new PaginatedList<Opportunity>(requestData.PageSize, requestData.CurrentPage, opportunities, Opportunities.Count));

                //var lista = new List<Opportunity>();

                //var content = JsonConvert.SerializeObject(requestData);

                //HttpContent httpContent = new StringContent(content);

                //httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                //var response = await client.GetStringAsync($"{cadena}");

                //if (response != null)
                //{
                //    lista = JsonConvert.DeserializeObject<List<Opportunity>>(response);
                //}

                //return lista;

            }
            catch ( Exception e)
            {
                throw;
            }
        }
        public async Task<IEnumerable<Opportunity>> GetOp(OpportunitiesPaginatedRequest requestData,string cadena, string token)
        {
            try
            {
                //List<Opportunity> opportunities = Opportunities
                //                                  .Where(o => string.IsNullOrWhiteSpace(requestData.Query) ||
                //                                              o.Description.ToLower().Contains(requestData.Query.ToLower()))
                //                                  .Skip(requestData.CurrentPage - 1 * requestData.PageSize).Take(requestData.PageSize).ToList();

                //return Task.FromResult(new PaginatedList<Opportunity>(requestData.PageSize, requestData.CurrentPage, opportunities, Opportunities.Count));

                var lista = new List<Opportunity>();

                var content = JsonConvert.SerializeObject(requestData);

                HttpContent httpContent = new StringContent(content);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetStringAsync($"{cadena}");

                if (response != null)
                {
                    lista = JsonConvert.DeserializeObject<IEnumerable<Opportunity>>(response).ToList();
                }

                return lista;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SaveOpportunityCommand(Opportunity opportunity)
        {
            if (opportunity.Id == 0)
            {
                opportunity.Id = Opportunities.Count == 0 ? 1 : Opportunities.Max(o => o.Id) + 1;
            }

            Opportunities.Add(opportunity);
            await Task.FromResult(0);
        }
    }
}
