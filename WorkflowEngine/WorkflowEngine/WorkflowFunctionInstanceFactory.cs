using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowFunctionInstanceFactory : IWorkflowFunctionInstanceFactory
    {
        public virtual IWorkflowFunctionInstance GetNewInstance(string name)
        {
            if (name == "print")
                return new WorkflowFunctionInstancePrint();
            if (name == "if")
                return PureWorkflowFunctionInstance.FromLambda((inputs) => inputs["condition"].Bool() ? inputs["then"] : inputs["else"]);
            if (name == "wait_for_confirmation")
                return new WorkflowFunctionInstanceWaitForConfirmation();

            throw new Exception($"function {name} not found");
        }

        public void RegisterFunction(string name, Func<IWorkflowFunctionInstance> build)
        {
            
        }
    }
}
