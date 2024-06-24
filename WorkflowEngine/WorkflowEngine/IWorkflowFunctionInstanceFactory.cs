namespace Workflows
{
    public interface IWorkflowFunctionInstanceFactory
    {
        IWorkflowFunctionInstance GetNewInstance(string name);
    }
}