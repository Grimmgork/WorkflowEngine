using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Function
{
    public class WorkflowFunction : WorkflowFunctionInstanceBase
    {
        private Func<SomeData, SomeData> run;

        public WorkflowFunction(Func<SomeData, SomeData> run)
        {
            this.run = run;
        }

        public sealed override SomeData Run(IWorkflowSignalHandler context, SomeData inputs)
        {
            return run.Invoke(inputs);
        }
    }
}
