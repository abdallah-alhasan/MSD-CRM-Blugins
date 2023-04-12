using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrainingFirst.Plugins
{
    public class UpdateMonthlyInstallments : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            Entity preImage = context.PreEntityImages.Contains("PreImage") ? context.PreEntityImages["PreImage"] : null;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity targetEntity = (Entity)context.InputParameters["Target"];

                if (targetEntity.LogicalName == "new_installment")
                {
                    EntityReference contactRef = (EntityReference)(targetEntity.Contains("new_contactid") ?  targetEntity["new_contactid"] : preImage["new_contactid"]);
                    Entity contactEntity = service.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet("new_amountofmonthlyinstallment"));

                    QueryExpression query = new QueryExpression("new_installment")
                    {
                        ColumnSet = new ColumnSet("new_new_actualamountpaid")
                    };
                    query.Criteria.AddCondition("new_contactid", ConditionOperator.Equal, contactRef.Id);
                    query.Criteria.AddCondition("new_dateofpayment", ConditionOperator.LastXMonths, 1);
                    var installmentEntities = service.RetrieveMultiple(query).Entities;

                    var totalInstallmentAmount = installmentEntities.Sum(installment => (int)installment["new_new_actualamountpaid"]);
                    var monthlyInstallmentAmount = contactEntity.GetAttributeValue<Money>("new_amountofmonthlyinstallment");
                    if (monthlyInstallmentAmount == null)
                    {
                        monthlyInstallmentAmount = new Money(totalInstallmentAmount);
                    }
                    else if (targetEntity.Contains("new_dateofpayment") && (DateTime)targetEntity["new_dateofpayment"] > DateTime.Today.AddMonths(-1))
                    {
                        monthlyInstallmentAmount.Value = totalInstallmentAmount + (targetEntity.Contains("new_new_actualamountpaid") ? (int)targetEntity["new_new_actualamountpaid"] : (int)preImage["new_new_actualamountpaid"]);
                    }
                    else
                    {
                        monthlyInstallmentAmount.Value = totalInstallmentAmount;
                    }

                    contactEntity.Attributes["new_amountofmonthlyinstallment"] = monthlyInstallmentAmount;
                    service.Update(contactEntity);
                    
                }
            }
        }
    }
}
