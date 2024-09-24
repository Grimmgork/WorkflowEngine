namespace Workflows
{
    public interface IWorkflowFunctionInstanceFactory
    {
        void RegisterFunction(string name, Func<IWorkflowFunctionInstance> build);

        IWorkflowFunctionInstance GetNewInstance(string name);
    }
}