using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;

namespace ShowRoomPlugins
{
    public class CheckPastDeals : CodeActivity
    {
        [Input("Check for past deals")]
        [Default("false")]
        public InArgument<bool> CheckForPastDeals { get; set; }

        [Input("Contact")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> Contact { get; set; }

        [Output("Past Deal")]
        [ReferenceTarget("new_deal")]
        public OutArgument<EntityReference> PastDeal { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationService organizationService = context.GetExtension<IOrganizationServiceFactory>().CreateOrganizationService(workflowContext.UserId);

            if (CheckForPastDeals.Get(context))
            {
                QueryExpression query = new QueryExpression("new_deal");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition("new_contactid", ConditionOperator.Equal, Contact.Get(context).Id);

                EntityCollection results = organizationService.RetrieveMultiple(query);
                if (results.Entities.Count > 0)
                {
                    EntityReference pastDealRef = new EntityReference("new_deal", results.Entities[0].Id);
                    PastDeal.Set(context, pastDealRef);
                   
                }
               
            }
        }
    }
}
