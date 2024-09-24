using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public enum WorkflowInstanceState
    {
        Initial,
        Running,
        Waiting,
        Done
    }

    public class WorkflowInstance
    {
        public WorkflowInstanceState State { get; private set; }

        private WorkflowDefinition definition;

        private List<IWorkflowFunctionInstance> functionInstances = new List<IWorkflowFunctionInstance>();
        private IWorkflowFunctionInstance? currentFunctionInstance;
        private WorkflowInstanceContext context;
        private IWorkflowMessageHandler messageHandler;

        public WorkflowInstance(WorkflowDefinition definition, IWorkflowFunctionInstanceFactory instanceFactory, IWorkflowMessageHandler messageHandler) 
        {
            this.messageHandler = messageHandler;
            this.context = new WorkflowInstanceContext(messageHandler.PushMessage);
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
                return;

            if (State == WorkflowInstanceState.Waiting)
                return;

            if (currentFunctionInstance == null)
                return;

            if (State == WorkflowInstanceState.Initial)
                State = WorkflowInstanceState.Running;

            WorkflowFunctionState result = await RunStatefulFunction(context, currentFunctionInstance, token);
            if (result == WorkflowFunctionState.Waiting)
            {
                State = WorkflowInstanceState.Waiting;
                return;
            }
            if (result == WorkflowFunctionState.Done)
            {
                // resolve next
                WorkflowValue nextRef = await ResolveOutputRefToLiteral(context, definition.GetFunctionNode(currentFunctionInstance!.Id).Next);
                if (!nextRef.IsDefined)
                {
                    State = WorkflowInstanceState.Done;
                    return;
                }

                // move to next
                currentFunctionInstance = functionInstances.First(i => i.Id == nextRef.FunctionRef());
                State = WorkflowInstanceState.Running;
                return;
            }
        }

        public async Task SendMessage(WorkflowMessage args)
        {
            if (State != WorkflowInstanceState.Waiting)
                return;

            if (currentFunctionInstance != null)
            {
                WorkflowFunctionState result = await currentFunctionInstance.HandleMessageAsync(args, default);
                if (result == WorkflowFunctionState.Running)
                {
                    State = WorkflowInstanceState.Running;
                    return;
                }
                if (result == WorkflowFunctionState.Done)
                {
                    State = WorkflowInstanceState.Done;
                    return;
                }
            }
        }


        private async Task<WorkflowValueObject> GetInputsForFunctionNode(WorkflowInstanceContext handler, int id)
        {
            WorkflowValueObject inputs = new WorkflowValueObject();
            WorkflowInputNode[] inputNodes = definition.GetInputNodesForFunction(id);
            if (inputNodes.Length == 0)
                return inputs;

            foreach (WorkflowInputNode input in inputNodes)
                inputs[input.Name] = await ResolveOutputRefToLiteral(handler, input.Source);

            return inputs;
        }

        private async Task<WorkflowValue> ResolveOutputRefToLiteral(WorkflowInstanceContext handler, WorkflowValue value)
        {
            if (value.Type != WorkflowDataType.OutputRef)
                return value;

            WorkflowOutputNode outputNode = definition.GetOutputNode(value.OutputRef());
            WorkflowValueObject outputValues = await ComputeFunctionOutputs(handler, functionInstances.First(i => i.Id == outputNode.FunctionId));
            return outputValues[outputNode.Name];
        }

        private async Task<WorkflowValueObject> ComputeFunctionOutputs(WorkflowInstanceContext handler, IWorkflowFunctionInstance instance)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(handler, instance.Id);
            instance.SetInput(inputs);
            await instance.RunAsync(handler, default);
            return instance.GetOutput();
        }

        private async Task<WorkflowFunctionState> RunStatefulFunction(WorkflowInstanceContext handler, IWorkflowFunctionInstance instance, CancellationToken token = default)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(handler, instance.Id);
            instance.SetInput(inputs);
            return await instance.RunAsync(handler, token);
        }
    }
}
