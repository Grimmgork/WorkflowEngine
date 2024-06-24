using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngineIntegration;

namespace Workflows
{
    public class WorkflowFunctionPrintBusiness : IWorkflowFunctionInstance
    {
        IBusinessLogicService businesLogicService;

        public WorkflowFunctionPrintBusiness(IBusinessLogicService businesLogicService)
        {
            this.businesLogicService = businesLogicService;
        }

        public WorkflowValue GetOutput(string name)
        {
            return new WorkflowValue();
        }

        public void HandleEvent(EventArgs args)
        {
            
        }

        public Task<WorkflowFunctionResultStatus> Run()
        {
            Console.WriteLine(WorkflowValue.String("business!"));
            return Task.FromResult(WorkflowFunctionResultStatus.Done);
        }

        public void SetInput(string name, WorkflowValue value)
        {
            
        }
    }
}
