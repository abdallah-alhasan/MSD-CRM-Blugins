using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrainingFirst.Plugins
{
    public class UpdateInstallmentCount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity installment = (Entity)context.InputParameters["Target"];

                if (installment.LogicalName == "new_installment")
                {
                    // Get the related contact record
                    EntityReference contactReference = (EntityReference)installment["new_contactid"];
                    Entity contact = service.Retrieve(contactReference.LogicalName, contactReference.Id, new ColumnSet(true));

                    // Increment the count of installments
                    QueryExpression query = new QueryExpression("new_installment")
                    {
                        ColumnSet = new ColumnSet("new_installmentid")
                    };
                    query.Criteria.AddCondition("new_contactid", ConditionOperator.Equal, contactReference.Id);

                    EntityCollection installmentRecords = service.RetrieveMultiple(query);
                    int count = installmentRecords.Entities.Count;
                    contact["new_countofinstallmentsmade"] = count + 1;

                    if (contact.Contains("new_totalamountofpayments"))
                    {
                        contact["new_totalamountofpayments"] = 1000;
                    }

                    // Update the contact record
                    service.Update(contact);
                }
            }
        }
    }
}
