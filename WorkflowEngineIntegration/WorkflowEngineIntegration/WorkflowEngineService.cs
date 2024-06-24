
using Workflows;

namespace WorkflowEngineIntegration
{
    public interface IWorkflowEngineService
    {

    }

    public class WorkflowEngineService : IWorkflowEngineService, IAsyncDisposable
    {
        IServiceScopeFactory scopeFactory;

        private List<WorkflowInstance> instances = new List<WorkflowInstance>();

        public WorkflowEngineService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void StartWorkflow(string name)
        {
            // deserialze WorkflowDefiniton from database
            IServiceScope scope = scopeFactory.CreateScope();
            WorkflowDefinition definition = new WorkflowDefinition();
            WorkflowInstance instance = new WorkflowInstance(definition, new WorkflowFunctionInstanceFactory(scope.ServiceProvider));

            // while(!instance.CanStep)
            //    while (HasEvent)
            //       instance.HandleEvent(event)
            //    instance.Step()
            //    while (instance.HasEvent)
            //       RaiseEvent(instance.ReadEvent())

            // SupplyValueEvent
            //    Value

            // RequestStepEvent

            // RequestPauseEvent

            // RequestAbortEvent

            // EventArgs

            
            instances.Add(instance);
            instance.Start();
        }

        public async Task SendMessage(int instanceId, )
        {

        }

        public ValueTask DisposeAsync()
        {
            foreach (WorkflowInstance instanc in instances)
            {
                await AbortWorkflow(0);
            }

            // wait for all completionSources
            // done
        }
    }
}
