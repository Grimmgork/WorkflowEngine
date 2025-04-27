using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;
using Workflows.Message;
using Workflows.Signal;

namespace Workflows.Method
{
    public class WaitForInputMethod : WorkflowMethodBase
    {
        public override async Task<WorkflowMethodState> RunAsync(WorkflowMethodContext context, CancellationToken token)
        {
            await context.RaiseSignal(new RequestInputSignal(DateTime.Now));
            return WorkflowMethodState.Suspended;
        }

        public override WorkflowMethodState OnSignal(WorkflowMethodContext context, WorkflowSignal signal, CancellationToken token)
        {
            if (signal is PromptInputSignal inputSignal)
            {
                SomeData output = SomeData.Struct();
                output["Result"] = SomeData.FromString(inputSignal.Input);
                context.Output = output;
                return WorkflowMethodState.Done;
            }
            else
            {
                return WorkflowMethodState.Suspended;
            }
        }
    }
}
