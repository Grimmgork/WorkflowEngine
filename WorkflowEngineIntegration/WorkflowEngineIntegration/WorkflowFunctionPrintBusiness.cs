using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngineIntegration;

namespace Workflows
{
    public class WorkflowFunctionPrintBusiness : WorkflowFunctionInstanceBase
    {
        private readonly IServiceScopeFactory scopeFactory;

        public WorkflowFunctionPrintBusiness(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public override async Task<WorkflowValueObject> RunAsync(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            WorkflowMessage message = await context.ReadMessage(token);
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IBusinessLogicService>().Hello();
            }
            return new WorkflowValueObject();
        }
    }
}
