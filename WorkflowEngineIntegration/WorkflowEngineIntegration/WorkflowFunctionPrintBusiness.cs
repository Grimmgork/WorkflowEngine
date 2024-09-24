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

        public void HandleEvent(EventArgs args)
        {
            
        }

        public Task<WorkflowFunctionResultStatus> Run()
        {
            using(IServiceScope scope = scopeFactory.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<IBusinessLogicService>().Hello();
            }
            return Task.FromResult(WorkflowFunctionResultStatus.Done);
        }
    }
}
