namespace Workflows
{
    public enum WorkflowInstanceState
    {
        Initial,
        Running,
        Done
    }

    public class WorkflowInstance
    {
        public WorkflowInstanceState State { get; private set; } = WorkflowInstanceState.Initial;

        private WorkflowDefinition definition;

        private List<IWorkflowFunctionInstance> functionInstances = new List<IWorkflowFunctionInstance>();
        private IWorkflowFunctionInstance? currentFunctionInstance;
        private WorkflowInstanceContext context;

        public WorkflowInstance(WorkflowDefinition definition, IWorkflowFunctionInstanceFactory instanceFactory, IWorkflowMessageHandler messageHandler) 
        {
            this.context = new WorkflowInstanceContext(
                messageHandler.ReadMessage, 
                messageHandler.WriteMessage
            );

            this.definition = definition;
            foreach (WorkflowFunctionNode node in definition.GetFunctionNodes())
            {
                IWorkflowFunctionInstance instance = instanceFactory.GetNewInstance(node.FunctionName);
                instance.Id = node.Id; // TODO
                functionInstances.Add(instance);
            }

            if (definition.EntryPointId != null)
                currentFunctionInstance = functionInstances.First(i => i.Id == definition.EntryPointId);
        }

        public async Task Run(CancellationToken token = default)
        {
            if (State == WorkflowInstanceState.Done)
                throw new InvalidOperationException();

            State = WorkflowInstanceState.Running;

            WorkflowValueObject result = await RunStatefulFunction(context, currentFunctionInstance!, token);
                
            // resolve next
            WorkflowValue nextRef = await ResolveOutputRefToLiteral(context, definition.GetFunctionNode(currentFunctionInstance!.Id).Next);
            if (!nextRef.IsDefined)
            {
                State = WorkflowInstanceState.Done;
                return;
            }

            // move to next
            currentFunctionInstance = functionInstances.First(i => i.Id == nextRef.FunctionRef());
        }

        private async Task<WorkflowValueObject> GetInputsForFunctionNode(IWorkflowMessageHandler handler, int id)
        {
            WorkflowValueObject inputs = new WorkflowValueObject();
            WorkflowInputNode[] inputNodes = definition.GetInputNodesForFunction(id);
            if (inputNodes.Length == 0)
                return inputs;

            foreach (WorkflowInputNode input in inputNodes)
                inputs[input.Name] = await ResolveOutputRefToLiteral(handler, input.Source);

            return inputs;
        }

        private async Task<WorkflowValue> ResolveOutputRefToLiteral(IWorkflowMessageHandler handler, WorkflowValue value)
        {
            if (value.Type != WorkflowDataType.OutputRef)
                return value;

            WorkflowOutputNode outputNode = definition.GetOutputNode(value.OutputRef());
            WorkflowValueObject outputValues = await ComputeFunctionOutputs(functionInstances.First(i => i.Id == outputNode.FunctionId));
            return outputValues[outputNode.Name];
        }

        private async Task<WorkflowValueObject> ComputeFunctionOutputs(IWorkflowFunctionInstance instance)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(null, instance.Id);
            return await instance.RunAsync(null, inputs, default);
        }

        private async Task<WorkflowValueObject> RunStatefulFunction(IWorkflowMessageHandler handler, IWorkflowFunctionInstance instance, CancellationToken token = default)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(handler, instance.Id);
            return await instance.RunAsync(handler, inputs, token);
        }
    }
}
