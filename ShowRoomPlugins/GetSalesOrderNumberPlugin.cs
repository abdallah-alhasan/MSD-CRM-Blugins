using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ShowRoomPlugins
{
    class GetSalesOrderNumberPlugin
    {
        public string GenerateSalesOrderNumber(int vin)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var salesOrderNumberUrl = $"https://testhussien122.azurewebsites.net/api/generatesalesorder?vin{vin}";
                    var salesOrderNumberResponse = client.GetAsync(salesOrderNumberUrl).Result;
                    var salesOrderNumber = salesOrderNumberResponse.Content.ReadAsStringAsync().Result;

                    return salesOrderNumber;
                }
                catch (HttpRequestException ex)
                {

                    throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
