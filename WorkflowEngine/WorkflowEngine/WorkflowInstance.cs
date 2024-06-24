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
        Running,
        Error,
        Paused,
        Aborted,
        Done
    }

    public class WorkflowInstance
    {
        private WorkflowDefinition definition;
        private Queue<WorkflowEventArgs> events = new Queue<WorkflowEventArgs>();
        private List<IWorkflowFunctionInstance> functionInstances = new List<IWorkflowFunctionInstance>();
        private IWorkflowFunctionInstance? currentFunctionInstance;
        public WorkflowInstanceState State { get; private set; }
        public bool HasEvent => events.Count != 0;

        public WorkflowInstance(WorkflowDefinition definition, IWorkflowFunctionInstanceFactory instanceFactory) 
        {
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

        public async Task<bool> Iterate()
        {
            if (currentFunctionInstance == null)
                return false;

            if (State == WorkflowInstanceState.Aborted)
                return false;

            if (State == WorkflowInstanceState.Paused)
                return true;

            if (State == WorkflowInstanceState.Error)
                return true;


            WorkflowFunctionResult result = await RunStatefulFunction(currentFunctionInstance);
            foreach (WorkflowEventArgs args in result.Events)
                events.Enqueue(args);

            if (result.IsError)
            {
                events.Enqueue(new WorkflowEventArgs() { Name = "ErrorOccured" });
                State = WorkflowInstanceState.Error;
                return true;
            }

            if (result.IsDone)
            {
                // resolve next
                WorkflowValue nextRef = await ResolveOutputRefToLiteral(definition.GetFunctionNode(currentFunctionInstance!.Id).Next);
                if (!nextRef.IsDefined)
                {
                    State = WorkflowInstanceState.Done;
                    return false;
                }

                if (nextRef.Type != WorkflowDataType.FunctionRef)
                {
                    State = WorkflowInstanceState.Error;
                    return true;
                }

                // move to next
                currentFunctionInstance = functionInstances.First(i => i.Id == nextRef.AsFunctionRef());
            }

            return true;
        }

        public void SendEvent(WorkflowEventArgs args)
        {
            if (args.Name == "PauseWorkflow")
            {
                State = WorkflowInstanceState.Paused;
            }
            if (args.Name == "ResumeWorkflow")
            {
                State = WorkflowInstanceState.Running;
            }
            if (args.Name == "AbortWorkflow")
            {
                State = WorkflowInstanceState.Aborted;
            }

            if (currentFunctionInstance != null)
                currentFunctionInstance.HandleEvent(args);
        }

        public WorkflowEventArgs ReadEvent()
        {
            return events.Dequeue();
        }

        private async Task<WorkflowValueObject> GetInputsForFunctionNode(int id)
        {
            WorkflowValueObject inputs = new WorkflowValueObject();
            WorkflowInputNode[] inputNodes = definition.GetInputNodesForFunction(id);
            if (inputNodes.Length == 0)
                return inputs;

            foreach (WorkflowInputNode input in inputNodes)
                inputs[input.Name] = await ResolveOutputRefToLiteral(input.Source);

            return inputs;
        }

        private async Task<WorkflowValue> ResolveOutputRefToLiteral(WorkflowValue value)
        {
            if (value.Type != WorkflowDataType.OutputRef)
                return value;

            WorkflowOutputNode outputNode = definition.GetOutputNode(value.AsOutputRef());
            WorkflowValueObject outputValues = await ComputeFunctionOutputs(functionInstances.First(i => i.Id == outputNode.FunctionId));
            return outputValues[outputNode.Name];
        }


        private async Task<WorkflowValueObject> ComputeFunctionOutputs(IWorkflowFunctionInstance instance)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(instance.Id);
            foreach(string name in inputs.Names)
                instance.SetInput(name, inputs[name]);

            await instance.Run();
            return instance.GetOutputs();
        }

        private async Task<WorkflowFunctionResult> RunStatefulFunction(IWorkflowFunctionInstance instance)
        {
            WorkflowValueObject inputs = await GetInputsForFunctionNode(instance.Id);
            foreach (string name in inputs.Names)
                instance.SetInput(name, inputs[name]);

            return await instance.Run();
        }
    }
}
