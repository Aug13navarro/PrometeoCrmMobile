using Core.Model;
using Core.Model.Common;
using Core.Services.Contracts;
using Core.ViewModels.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class OfflineCharging
    {
        private HttpClient client;

        public OfflineCharging()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://neophos-testing-api.azurewebsites.net/")
            };
        }

        public void ObtenerEmpresas()
        {

        }
    }
}
