using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class ConsoleMessageHandler : IWorkflowMessageHandler
    {
        public Task<WorkflowMessage> ReadMessage(CancellationToken token = default)
        {
            return Task.FromResult<WorkflowMessage>(new CustomWorkflowMessage(Console.ReadLine() ?? ""));
        }

        public Task WriteMessage(WorkflowMessage message)
        {
            return Task.Run(() => Console.WriteLine($"{message}"));
        }
    }
}
