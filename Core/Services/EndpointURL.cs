using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services
{
    public static class EndpointURL
    {

        public static Uri PrometeoApiEndPoint { get; } = new Uri("https://prometeo-erp-develop-api.azurewebsites.net/");
        //public static Uri PrometeoApiEndPoint { get; } = new Uri("https://neophos-testing-api.azurewebsites.net/");
        //public static Uri PrometeoApiEndPoint { get; } = new Uri("https://prometeo-produccion-api.azurewebsites.net/");
    }
}
