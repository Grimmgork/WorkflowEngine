using Workflows.Function;
using Workflows.Method;

namespace Workflows
{
    public interface IWorkflowFunctionInstanceFactory
    {
        public IWorkflowFunctionInstance GetFunctionInstance(string name);

        public IWorkflowMethodInstance GetMethodInstance(string name);
    }
}