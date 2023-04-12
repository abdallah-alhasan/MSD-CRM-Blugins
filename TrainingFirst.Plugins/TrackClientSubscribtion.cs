using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;



namespace TrainingFirst.Plugins
{
    public class TrackClientSubscribtion : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            Entity contact = (Entity)context.InputParameters["Target"];
            #endregion

            if (context.Depth > 3)
            {
                return;
            }

            if (contact.Contains("new_planid"))
            {
                EntityReference planRef = (EntityReference)contact["new_planid"];
                Entity plan = service.Retrieve(planRef.LogicalName,planRef.Id, new ColumnSet(true));

                var planType = plan["new_plan_category"];
                contact["new_contacttype"] = planType;
                contact["new_subscribtion_date"] = DateTime.Today;
                service.Update(contact);
            }
        }
    }
}
