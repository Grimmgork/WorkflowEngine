using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows.Method
{
    public class IfMethod : WorkflowMethodBase
    {
        public override WorkflowMethodState Run(WorkflowMethodContext context, CancellationToken token)
        {
            context.Next = context.Input["If"].ToBoolean() ? context.Input["Then"].ToMethodRef().Id : context.Input["Else"].ToMethodRef().Id;
            return WorkflowMethodState.Done;
        }
    }
}
