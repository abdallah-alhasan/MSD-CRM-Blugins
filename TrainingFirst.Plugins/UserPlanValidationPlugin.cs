using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrainingFirst.Plugins
{
    public class UserPlanValidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region 
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            Entity preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;
            #endregion

            

            if (entity.Contains("new_planid") || entity.Contains("birthdate"))
            {
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                DateTime birthday = entity.Contains("birthdate") ? (DateTime)entity["birthdate"] : (DateTime)preImage["birthdate"];

                int GetAge(DateTime dob)
                {
                    int age = DateTime.Now.Year - dob.Year;
                    if (DateTime.Now.Month < dob.Month || (DateTime.Now.Month == dob.Month && DateTime.Now.Day < dob.Day))
                       age--;
                    return age;
                }

                EntityReference plan = entity.Contains("new_planid") ? (EntityReference)entity["new_planid"] : (EntityReference)preImage["new_planid"];

                Entity planEntity = service.Retrieve(plan.LogicalName, plan.Id, new ColumnSet("new_minage", "new_maxage"));
                int userAge = GetAge(birthday);


                int planMinAge = (int)planEntity["new_minage"];
                int planMaxAge = (int)planEntity["new_maxage"];
                if (userAge < planMinAge || userAge > planMaxAge)
                {
                    throw new InvalidPluginExecutionException("user age: " + userAge + " is not applicable to this plan, plan age: " + planMinAge + "-" + planMaxAge);
                }
            }
        }
    }
}
