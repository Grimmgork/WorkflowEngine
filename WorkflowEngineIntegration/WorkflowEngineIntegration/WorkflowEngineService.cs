
using Workflows;

namespace WorkflowEngineIntegration
{
    public interface IWorkflowEngineService
    {
        void StartWorkflow(string name);

        void StopWorkflow(int instanceId);

        WorkflowInstanceState GetInstance(int instanceId);

        IEnumerable<WorkflowInstanceState> GetInstances();
    }

    public class WorkflowInstanceState
    {
        public readonly string Name;

        public readonly int InstanceId;
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
            WorkflowDefinition definition = new WorkflowDefinition();
            WorkflowFunctionInstanceFactory functionFactory = new WorkflowFunctionInstanceFactory();
            WorkflowInstance instance = new WorkflowInstance(definition, functionFactory);

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
