using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
namespace TrainingFirst.Plugins
{
    public class MinMaxAgeValidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity entity = (Entity)context.InputParameters["Target"];
            Entity preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;

            if(entity.Contains("new_minage") || entity.Contains("new_maxage"))
            {
                int minAge = entity.Contains("new_minage") ? (int)entity["new_minage"] : (int)preImage["new_minage"];
                int maxAge = entity.Contains("new_maxage") ? (int)entity["new_maxage"] : (int)preImage["new_maxage"];
               
                //In update only
                if (preImage != null)
                {
                    int oldMinAge = (int)preImage["new_minage"];
                    int oldMaxAge = (int)preImage["new_maxage"];
                    if(minAge < oldMinAge)
                    {
                        throw new InvalidPluginExecutionException("Min age can't be less than the previous min age:" + oldMinAge as string);
                    }
                    if(maxAge > oldMaxAge)
                    {
                        throw new InvalidPluginExecutionException("Max age can't be more than the previous max age:" + oldMaxAge as string);
                    }
                }

                if (minAge > maxAge)
                {
                    throw new InvalidPluginExecutionException("Min age can't be more than the max age");
                }
            }

        }
    }
}

