using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discount
{
    class DiscountAction
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            if (entity.Contains("telephone1"))
            {
                IOrganizationServiceFactory serviceFactory =
               (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                EntityCollection result = null;
                do
                {
                    result = RetrieveContacts(service, entity.Id);

                    foreach (var e in result.Entities)
                    {

                        Entity updatedContact = new Entity(e.LogicalName);
                        updatedContact.Id = e.Id;
                        updatedContact["company"] = entity["telephone1"];
                        service.Update(updatedContact);
                    }

                } while (result.MoreRecords);
            }
        }
    }
}
