using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public interface IWorkflowFunctionInstance : IDisposable
    {
        public int Id { get; set; } // TODO

        public void SetInput(WorkflowValueObject input);

        public WorkflowValueObject GetOutput();

        public WorkflowFunctionState Run(WorkflowInstanceContext context, CancellationToken token);

        public Task<WorkflowFunctionState> RunAsync(WorkflowInstanceContext context, CancellationToken token);

        public WorkflowFunctionState HandleMessage(WorkflowMessage message, CancellationToken token);

        public Task<WorkflowFunctionState> HandleMessageAsync(WorkflowMessage message, CancellationToken token);
    }
}
