using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;
using Workflows.Message;
using Workflows.Method;
using Workflows.Signal;

namespace Workflows.Function
{
    public abstract class WorkflowFunctionInstanceBase : IWorkflowFunctionInstance
    {
        public int Id { get; set; }

        public virtual void Dispose()
        {
            // do nothing
        }

        public virtual SomeData Run(IWorkflowSignalHandler context, SomeData input)
        {
            throw new NotImplementedException();
        }

        public SomeData Run(SomeData input)
        {
            throw new NotImplementedException();
        }
    }


}
