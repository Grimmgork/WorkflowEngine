using Workflows.Message;

namespace Workflows.Method
{
    public interface IWorkflowMethodInstance : IDisposable
    {

        public Task<WorkflowMethodState> RunAsync(WorkflowMethodContext context, CancellationToken token);

        public Task<WorkflowMethodState> OnSignalAsync(WorkflowMethodContext context, WorkflowSignal signal, CancellationToken token);
    }
}
