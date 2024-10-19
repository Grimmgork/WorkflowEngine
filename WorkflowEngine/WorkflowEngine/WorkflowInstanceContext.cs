using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowInstanceContext : IWorkflowMessageHandler
    {
        private Func<CancellationToken, Task<WorkflowMessage>> read;
        private Func<WorkflowMessage, Task> write;

        public WorkflowInstanceContext(Func<CancellationToken, Task<WorkflowMessage>> read, Func<WorkflowMessage, Task> write)
        {
            this.read = read;
            this.write = write;
        }

        public Task<WorkflowMessage> ReadMessage(CancellationToken token)
        {
            return read.Invoke(token);
        }

        public Task WriteMessage(WorkflowMessage message)
        {
            return write.Invoke(message);
        }
    }
}
