using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class Connection
    {
        public static async Task<bool> SeeConnection()
        {
            if(!CrossConnectivity.Current.IsConnected)
            {
                return false;
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");

            if(!isReachable)
            {
                return false;
            }

            return true;
        }
    } 
}
