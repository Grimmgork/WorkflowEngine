using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowInstanceContext
    {
        private Func<WorkflowMessage, Task> messageHandler;

        public WorkflowInstanceContext(Func<WorkflowMessage, Task> messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public Task SendMessageAsync(WorkflowMessage message)
        {
            return messageHandler.Invoke(message);
        }

        public void SendMessage(WorkflowMessage message)
        {
            messageHandler.Invoke(message).GetAwaiter().GetResult();
        }

        public void SetVariable()
        {

        }

        public void GetVariable()
        {

        }
    }
}
