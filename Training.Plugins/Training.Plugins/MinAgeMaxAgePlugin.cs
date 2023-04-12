using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Training.Plugins
{
    class MinAgeMaxAgePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            DateTime minAge = (DateTime)entity["min_age"];
            DateTime maxAge = (DateTime)entity["max_age"];
            if (minAge > maxAge)
            {
                throw new InvalidPluginExecutionException("Min age can't be larger than max age ");
            }
            
        }
    }
}

