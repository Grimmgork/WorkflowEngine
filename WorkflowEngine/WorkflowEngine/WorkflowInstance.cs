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
        Aborted
    }

    public class WorkflowInstance
    {
        private WorkflowDefinition definition;
        private IWorkflowFunctionInstanceFactory instanceFactory;
        private int currentFunctionId;
        private Queue<WorkflowEventArgs> events = new Queue<WorkflowEventArgs>();
        private Dictionary<string, IWorkflowFunctionInstance> functionInstances = new Dictionary<string, IWorkflowFunctionInstance>();
        private IWorkflowFunctionInstance? currentFunctionInstance;
        public WorkflowInstanceState State { get; private set; }


        public WorkflowInstance(WorkflowDefinition definition, IWorkflowFunctionInstanceFactory instanceFactory) 
        {
            this.definition = definition;
            this.instanceFactory = instanceFactory;
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

            WorkflowFunctionResult result = await currentFunctionInstance.Run();
            foreach (WorkflowEventArgs args in result.Events)
                events.Enqueue(args);

            if (result.IsError)
            {
                events.Enqueue(new WorkflowEventArgs() { Name = "ErrorOccured" });
                State = WorkflowInstanceState.Error;
                return true;
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

        public WorkflowEventArgs? ReadEvent()
        {
            if (events.Count == 0)
                return null;

            return events.Dequeue();
        }

        private Dictionary<string, WorkflowValue> GetInputsForFunctionNode(WorkflowDefinition workflow, WorkflowFunctionNode functionNode)
        {
            Dictionary<string, WorkflowValue> inputValues = new Dictionary<string, WorkflowValue>();
            WorkflowInputNode[] inputNodes = workflow.GetInputNodesForFunction(functionNode.Id);
            if (inputNodes.Length == 0)
                return inputValues;

            foreach (WorkflowInputNode input in inputNodes)
            {
                inputValues.Add(input.Name, ResolveOutputNodeToLiteral(workflow, input.Source));
            }

            return inputValues;
        }

        private WorkflowValue ResolveOutputNodeToLiteral(WorkflowDefinition workflow, WorkflowValue value)
        {
            if (value.Type != WorkflowDataType.OutputNode)
                return value;

            WorkflowOutputNode outputNode = workflow.GetOutputNode((int)value.Value!);
            WorkflowFunctionNode functionNode = workflow.GetFunctionNode(outputNode.FunctionId);
            Dictionary<string, WorkflowValue> outputValues = ExecuteFunction(workflow, functionNode);
            return outputValues[outputNode.Name];
        }

        private Dictionary<string, WorkflowValue> ExecuteFunction(WorkflowDefinition workflow, WorkflowFunctionNode function)
        {
            Func<Dictionary<string, WorkflowValue>, Dictionary<string, WorkflowValue>> handler = Functions[function.FunctionName];
            return handler.Invoke(GetInputsForFunctionNode(workflow, function));
        }
    }
}
