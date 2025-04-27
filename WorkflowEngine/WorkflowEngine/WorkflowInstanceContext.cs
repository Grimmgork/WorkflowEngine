using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Function;
using Workflows.Message;

namespace Workflows
{
    public class WorkflowInstanceContext
    {
        private Func<WorkflowSignal, Task> raiseSignal;

        public WorkflowInstanceContext(Func<WorkflowSignal, Task> raiseSignal)
        {
            this.raiseSignal = raiseSignal;
        }

        public Task RaiseSignal(WorkflowSignal signal)
        {
            return raiseSignal(signal);
        }
    }
}
