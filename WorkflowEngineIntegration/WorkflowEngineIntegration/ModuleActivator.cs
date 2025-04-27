using Microsoft.Extensions.Hosting;
using Workflows;

namespace WorkflowEngineIntegration
{
    public class ModuleActivator
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IBusinessLogicService, BusinessLogicService>();
        }

        public void RegisterWorkflowFunctions(IWorkflowFunctionInstanceFactory factory, IServiceScopeFactory scopeFactory)
        {
            factory.RegisterFunction("print", () => new WorkflowMethodPrintBusiness(scopeFactory));
        }
    }
}
