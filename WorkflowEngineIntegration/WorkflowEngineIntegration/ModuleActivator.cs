using Microsoft.Extensions.Hosting;

namespace WorkflowEngineIntegration
{
    public class ModuleActivator
    {
        public void Activate(IServiceCollection services)
        {
            services.AddScoped<IBusinessLogicService, BusinessLogicService>();
        }
    }
}
