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

        public WorkflowValueObject Run(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token);

        public Task<WorkflowValueObject> RunAsync(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token);
    }
}
