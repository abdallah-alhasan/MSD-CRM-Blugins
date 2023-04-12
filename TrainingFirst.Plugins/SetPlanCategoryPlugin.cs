using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrainingFirst.Plugins
{
    public class SetPlanCategoryPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region 
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            Entity preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;
            #endregion

            if (entity.Contains("new_minage"))
            {
               
            }
        }
    }
}
