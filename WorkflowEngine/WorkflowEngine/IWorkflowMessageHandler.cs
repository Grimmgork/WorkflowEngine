using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public interface IWorkflowMessageHandler
    {
        public Task PushMessage(WorkflowMessage message);
        public Task<WorkflowMessage> PullMessage();
    }
}
