using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ShowRoomPlugins
{
    public class GetDownPaymentInvoicePlugin : IPlugin
    {
        public string GeneratePaymentInvoiceNumber(string Model, string VinNumber, string Make, int MakeYear)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Generate Payment Invoice
                    var paymentInvoiceUrl = $"https://testhussien122.azurewebsites.net/api/generatePaymentInvoice";
                    
                    var values = new Dictionary<string, string>
                      {
                        { "Model", Model},
                        { "VinNumber", VinNumber},
                        { "Make", Make},
                        {"MakeYear", MakeYear.ToString()}
                      };
                    var vehicleData = new FormUrlEncodedContent(values);

                    var paymentInvoiceResponse = client.PostAsync(paymentInvoiceUrl, vehicleData).Result;
                    paymentInvoiceResponse.EnsureSuccessStatusCode();
                    string paymentInvoiceNumber = paymentInvoiceResponse.Content.ReadAsStringAsync().Result;

                    return paymentInvoiceNumber;
                }
                catch (HttpRequestException ex)
                {
                    // Handle error
                    throw new InvalidPluginExecutionException($"An error occurred: {ex.Message}");
                }
            }
        }

        
        public void Execute(IServiceProvider serviceProvider)
        {
            #region 
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            Entity deal = (Entity)context.InputParameters["Target"];
            Entity preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;
            #endregion

            if (context.Depth > 3)
                return;


            if (deal.Contains("crd69_downpaymenthasbeenpaid"))
            {
                Boolean downPaymentCheck = deal.GetAttributeValue<Boolean>("crd69_downpaymenthasbeenpaid");
                if (downPaymentCheck)
                {
                    EntityReference vehicleRef = deal.Contains("new_vehicleid") ? (EntityReference)preImage["new_vehicleid"] : (EntityReference)preImage["new_vehicleid"];
                    Entity vehicle = service.Retrieve(vehicleRef.LogicalName, vehicleRef.Id, new ColumnSet(true));

                    string make = (string)vehicle["new_make"];
                    string model = (string)vehicle["new_model"];
                    string vin = (string)vehicle["new_vin"];
                    int makeYear = (int)vehicle["new_modelyear"];

                    string paymentInvoiceNumber = GeneratePaymentInvoiceNumber(model, vin, make, makeYear);
                    deal["crd69_paymentinvoicenumber"] = paymentInvoiceNumber;
                    service.Update(deal);
                }
            }
        }
    }
}
