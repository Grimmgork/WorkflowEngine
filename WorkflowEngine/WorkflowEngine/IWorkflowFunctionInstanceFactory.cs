namespace Workflows
{
    public interface IWorkflowFunctionInstanceFactory
    {
        public IWorkflowFunctionInstance GetNewInstance(string name);

        public void RegisterFunction(string name, Func<IWorkflowFunctionInstance> build);
    }
}