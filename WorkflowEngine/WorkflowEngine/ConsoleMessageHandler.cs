using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    internal class ConsoleMessageHandler : IWorkflowMessageHandler
    {
        public Task<WorkflowMessage> PullMessage()
        {
            return Task.FromResult<WorkflowMessage>(new CustomWorkflowMessage(Console.ReadLine() ?? ""));
        }

        public Task PushMessage(WorkflowMessage message)
        {
            return Task.Run(() => Console.WriteLine($"{message}"));
        }
    }
}
