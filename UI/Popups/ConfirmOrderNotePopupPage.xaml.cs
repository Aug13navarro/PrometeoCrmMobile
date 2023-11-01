using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Core.Data;
using Core.Data.Tables;
using Core.Helpers;
using Core.Model;
using Core.Services;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmOrderNotePopupPage : PopupPage
    {
        private readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri(EndpointURL.PrometeoApiEndPoint.ToString())
        };

        public delegate void CreateHandler(int? order);

        public event CreateHandler ItCreated;

        public OrderNote OrderNote { get; set; }
        public string Token { get; set; }
        public bool Red { get; set; }

        private IMapper mapper;

        public ConfirmOrderNotePopupPage(OrderNote nuevaOrderNote, string token, bool red)
        {
            InitializeComponent();

            var mapperConfig = new MapperConfiguration(m => { m.AddProfile(new MappingProfile()); });

            mapper = mapperConfig.CreateMapper();

            OrderNote = nuevaOrderNote;
            Token = token;
            Red = red;

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;

            if (OrderNote.id == 0 && OrderNote.idOffline == 0)
            {
                if (Red)
                {
                    var respuesta = await CreateOrderNote(OrderNote);

                    if (respuesta != null)
                    {
                        if (respuesta.opportunityId > 0)
                        {
                            var send = new OpportunityPost
                            {
                                branchOfficeId = respuesta.customerId,
                                closedDate = DateTime.Now,
                                closedReason = "",
                                customerId = respuesta.customerId,
                                description = respuesta.oppDescription,
                                opportunityProducts = new List<OpportunityPost.ProductSend>(),
                                opportunityStatusId = 4,
                                totalPrice = Convert.ToDouble(respuesta.total)
                            };

                            send.opportunityProducts = ListaProductos(respuesta.Details);

                            var opp = new Opportunity();

                            await SaveOpportunityEdit(send, respuesta.id, Token, opp);

                            ItCreated(respuesta.opportunityId.Value);
                        }
                        else
                        {
                            ItCreated(null);
                        }
                    }
                }
                else
                {
                    var saved = await OfflineDatabase.SaveOrderNote(mapper.Map<OrderNoteTable>(OrderNote));

                    ItCreated(null);
                }
            }
            else
            {
                if (Red)
                {
                    await UpdateOrderNote(OrderNote, Token);
                    ItCreated(null);
                }
                else
                {
                    var ok = await OfflineDatabase.UpdateOrderNote(mapper.Map<OrderNoteTable>(OrderNote));
                    ItCreated(null);
                }
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
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

                var respuesta = await client.PostAsync(string.Format(cadena), httpContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<OrderNote>(resultado);
                }
                else
                {
                    throw new Exception(
                        "Tenemos problemas para procesar su solicitud. Contacte al administrador del sistema");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Tenemos problemas para procesar su solicitud - {e.Message}");
            }
        }

        public List<OpportunityPost.ProductSend> ListaProductos(MvxObservableCollection<OpportunityProducts> details)
        {
            var lista = new List<OpportunityPost.ProductSend>();

            foreach (var item in details)
            {
                double tempTotal = item.Price * item.Quantity;

                if (item.Discount != 0)
                {
                    tempTotal = tempTotal - ((tempTotal * item.Discount) / 100);
                }

                lista.Add(new OpportunityPost.ProductSend
                {
                    discount = item.Discount,
                    productId = item.productId,
                    quantity = item.Quantity,
                    total = tempTotal,
                    price = item.Price
                });
            }

            return lista;
        }

        public async Task SaveOpportunityEdit(OpportunityPost send, int id, string token, Opportunity opportunity)
        {
            var cadena = $"api/Opportunity?id={id}";

            var objeto = JsonConvert.SerializeObject(send);

            HttpContent httpContent = new StringContent(objeto);

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var respuesta = await client.PutAsync(string.Format(cadena), httpContent);

            var resultado = await respuesta.Content.ReadAsStringAsync();

            await Task.FromResult(0);
        }

        public async Task<OrderNote> UpdateOrderNote(OrderNote order, string token)
        {
            try
            {
                var url = $"/api/OpportunityOrderNote/UpdateOpportunityOrderNoteAsync?id={order.id}";

                var objeto = JsonConvert.SerializeObject(order);

                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PutAsync(string.Format(url), httpContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<OrderNote>(resultado);
                }
                else
                {
                    throw new Exception(
                        "Tenemos problemas para procesar su solicitud. Contacte al administrador del sistema");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Tenemos problemas para procesar su solicitud - {e.Message}");
            }
        }

        private async void Button_OnClicked_Cancel(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}