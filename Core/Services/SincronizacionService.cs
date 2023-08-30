using Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SincronizacionService 
    {
        private readonly HttpClient client;
        public SincronizacionService()
        {
            client = new HttpClient()
            {
                BaseAddress = EndpointURL.PrometeoApiEndPoint
            };
        }

        public async Task<bool> SincronizarPedidosOffline(List<OrderNote> ordenes, string token)
        {
            try
            {
                var url = $"/api/OpportunityOrderNote/Synchronize";

                var objeto = JsonConvert.SerializeObject(ordenes);
                HttpContent httpContent = new StringContent(objeto);

                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var respuesta = await client.PostAsync($"{url}", httpContent);

                if (respuesta.IsSuccessStatusCode)
                {
                    var resultado = await respuesta.Content.ReadAsStringAsync();

                    if(resultado.Contains("["))
                    {
                        throw new Exception(resultado);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
