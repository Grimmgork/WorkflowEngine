using Workflows;

namespace WorkflowEngineIntegration
{
    public class WorkflowFunctionInstanceFactory : IWorkflowFunctionInstanceFactory
    {
        IServiceProvider services;

        public WorkflowFunctionInstanceFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IWorkflowFunctionInstance GetNewInstance(string name)
        {
            if (name == "if")
            {
                return new WorkflowFunctionInstanceIf();
            }

            if (name == "print_business")
            {
                return new WorkflowFunctionPrintBusiness(services.GetRequiredService<IBusinessLogicService>());
            }

            throw new Exception("unknown name");
        }
    }
}
