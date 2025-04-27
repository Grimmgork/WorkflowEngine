using Workflows.Data;
using Workflows.Function;
using Workflows.Message;
using Workflows.Method;
using Workflows.Model;

namespace Workflows
{
    public enum WorkflowInstanceState
    {
        Initial,
        Running,
        Suspended,
        Done
    }

    public class WorkflowInstanceData
    {
        public int Id;
        public int WorkflowDefintitionId;
        public WorkflowInstanceState State;
        public int CurrentMethodId;
        public IDictionary<string, SomeData> Input = new Dictionary<string, SomeData>();
        public IDictionary<string, SomeData> Variables = new Dictionary<string, SomeData>();
        public IDictionary<int, SomeData> MethodData = new Dictionary<int, SomeData>();
        public IDictionary<int, SomeData> MethodOutput = new Dictionary<int, SomeData>();
    }

    public class WorkflowInstance
    {
        public readonly int Id;
        public readonly int WorkflowDefintitionId;
        public WorkflowInstanceState State => state;
        public int CurrentMethodId => currentMethodId;

        private int currentMethodId;

        private WorkflowInstanceState state = WorkflowInstanceState.Initial;
        private IDictionary<string, SomeData> workflowInput;
        private IDictionary<string, SomeData> variables;
        private IDictionary<int, SomeData> methodData;
        private IDictionary<int, SomeData> methodOutput;
        private WorkflowDefinition definition;

        private IWorkflowSignalHandler signalHandler;
        private IWorkflowMethodInstance currentMethodInstance;
        private IWorkflowActionInstanceFactory instanceFactory;

        private Func<WorkflowSignal, Task> sendSignal;

        public WorkflowInstance(int id, WorkflowDefinition definition, IWorkflowActionInstanceFactory instanceFactory, Func<WorkflowSignal, Task> sendSignal, IDictionary<string, SomeData>? input = null) 
        {
            this.Id = id;
            this.WorkflowDefintitionId = definition.Id;
            this.definition = definition;
            this.instanceFactory = instanceFactory;
            this.variables = new Dictionary<string, SomeData>();
            this.methodData = new Dictionary<int, SomeData>();
            this.methodOutput = new Dictionary<int, SomeData>();
            this.currentMethodId = definition.EntryPointId;
            this.workflowInput = input ?? new Dictionary<string, SomeData>();
            this.sendSignal = sendSignal;
        }

        public WorkflowInstance(WorkflowInstanceData data, WorkflowDefinition definition, IWorkflowActionInstanceFactory instanceFactory, Func<WorkflowSignal, Task> sendSignal)
        {
            this.Id = data.Id;
            this.state = data.State;
            this.currentMethodId = data.CurrentMethodId;
            this.variables = data.Variables;
            this.methodData = data.MethodData;
            this.methodOutput = data.MethodOutput;
            this.WorkflowDefintitionId = data.WorkflowDefintitionId;
            this.definition = definition;
            this.instanceFactory = instanceFactory;
            this.sendSignal = sendSignal;
        }

        public Task StepAsync(CancellationToken token = default)
        {
            return state switch {
                WorkflowInstanceState.Initial => InitializeAsync(token),
                WorkflowInstanceState.Running => RunMethodAsync(token),
                WorkflowInstanceState.Suspended => Task.CompletedTask,
                _ => throw new InvalidOperationException()
            };
        }

        public void SendSignal(WorkflowSignal signal)
        {
            SomeData inputs = GetInputsForMethod(definition.GetAction(currentMethodId));
            WorkflowMethodContext context = new WorkflowMethodContext(variables, inputs, methodData.GetValueOrDefault(currentMethodId), sendSignal);
            WorkflowMethodState methodState;
            try
            {
                methodState = currentMethodInstance.OnSignal(context, signal);
            }
            catch (Exception exception)
            {
                MoveToHandleError(exception);
                return;
            }

            methodData[currentMethodId] = context.Data;

            if (methodState == WorkflowMethodState.Done)
            {
                methodOutput[currentMethodId] = context.Output;
                MoveNextMethodOrDone();
                return;
            }
            else if (methodState == WorkflowMethodState.Running)
            {
                state = WorkflowInstanceState.Running;
                return;
            }
            else if (methodState == WorkflowMethodState.Suspended)
            {
                state = WorkflowInstanceState.Suspended;
                return;
            }
        }

        private Task InitializeAsync(CancellationToken token = default)
        {
            if (currentMethodId == 0)
            {
                state = WorkflowInstanceState.Done;
                return Task.CompletedTask;
            }
            else
            {
                string methodname = definition.GetAction(currentMethodId).Name;
                currentMethodInstance = instanceFactory.GetMethodInstance(methodname);
                state = WorkflowInstanceState.Running;
                return Task.CompletedTask;
            }
        }

        private async Task RunMethodAsync(CancellationToken token)
        {
            SomeData inputs = GetInputsForMethod(definition.GetAction(currentMethodId));
            WorkflowMethodContext context = new WorkflowMethodContext(variables, methodData.GetValueOrDefault(currentMethodId), inputs, sendSignal);
            WorkflowMethodState methodState;
            try
            {
                methodState = await currentMethodInstance.RunAsync(context, token);
            }
            catch (Exception exception)
            {
                MoveToHandleError(exception);
                return;
            }

            methodData[currentMethodId] = context.Data;

            if (methodState == WorkflowMethodState.Done)
            {
                methodOutput[currentMethodId] = context.Output;
                MoveNextMethodOrDone();
                return;
            }
            else if (methodState == WorkflowMethodState.Suspended)
            {
                state = WorkflowInstanceState.Suspended;
                return;
            }
            else if (methodState == WorkflowMethodState.Running)
            {
                state = WorkflowInstanceState.Running;
                return;
            }
        }

        private async Task ProcessSignalAsync(CancellationToken token)
        {
            SomeData inputs = GetInputsForMethod(definition.GetAction(currentMethodId));
            WorkflowMethodContext context = new WorkflowMethodContext(variables, inputs, methodData.GetValueOrDefault(currentMethodId), sendSignal);
            WorkflowMethodState methodState;
            try
            {
                methodState = currentMethodInstance.OnSignal(context, await signalHandler.WaitForSignal(token));
            }
            catch (Exception exception)
            {
                MoveToHandleError(exception);
                return;
            }

            methodData[currentMethodId] = context.Data;

            if (methodState == WorkflowMethodState.Done)
            {
                methodOutput[currentMethodId] = context.Output;
                MoveNextMethodOrDone();
                return;
            }
            else if (methodState == WorkflowMethodState.Running)
            {
                state = WorkflowInstanceState.Running;
                return;
            }
            else if (methodState == WorkflowMethodState.Suspended)
            {
                state = WorkflowInstanceState.Suspended;
                return;
            }
        }

        private void MoveNextMethodOrDone()
        {
            // resolve next
            SomeData nextRef = ResolveValue(definition.GetAction(currentMethodId).Next);
            if (nextRef.IsNull)
            {
                state = WorkflowInstanceState.Done;
                return;
            }
            else
            {
                // move to next
                currentMethodId = nextRef.ToMethodRef().Id;

                string methodname = definition.GetAction(currentMethodId).Name;
                currentMethodInstance = instanceFactory.GetMethodInstance(methodname);

                state = WorkflowInstanceState.Running;
                return;
            }
        }

        private void MoveToHandleError(Exception exception)
        {
            // resolve next
            SomeData nextRef = ResolveValue(definition.GetAction(currentMethodId).Error);
            if (nextRef.IsNull)
            {
                throw exception;
            }
            else
            {
                // move to next
                currentMethodId = nextRef.ToMethodRef().Id;

                string methodname = definition.GetAction(currentMethodId).Name;
                currentMethodInstance = instanceFactory.GetMethodInstance(methodname);

                state = WorkflowInstanceState.Running;
                return;
            }
        }

        private SomeData GetInputsForFunction(WorkflowActionNode function)
        {
            SomeData inputs = SomeData.Struct();
            IEnumerable<WorkflowInputNode> inputNodes = function.Inputs;
            if (!inputNodes.Any())
                return inputs;

            foreach (WorkflowInputNode input in inputNodes)
                inputs[input.Name] = ResolveValue(input.Source);

            return inputs;
        }

        private SomeData GetInputsForMethod(WorkflowActionNode method)
        {
            SomeData inputs = SomeData.Struct();
            IEnumerable<WorkflowInputNode> inputNodes = method.Inputs;
            if (!inputNodes.Any())
                return inputs;

            foreach (WorkflowInputNode input in inputNodes)
                inputs[input.Name] = ResolveValue(input.Source);

            return inputs;
        }

        private SomeData ResolveValue(SomeData value)
        {
            if (value.DataType == WorkflowDataType.Output)
            {
                WorkflowOutputRef outputRef = value.ToOutputRef();
                SomeData outputValues = GetActionOutput(definition.GetAction(outputRef.Id));
                return outputValues[outputRef.Name];
            }
            else if (value.DataType == WorkflowDataType.Variable)
            {
                return variables[value.ToVariableRef().Name];
            }
            else
            {
                return value;
            }
        }

        private SomeData GetActionOutput(WorkflowActionNode node)
        {
            if (node.Type == WorkflowActionNodeType.Method)
            {
                return methodOutput.GetValueOrDefault(node.Id);
            }
            else
            {
                WorkflowActionNode action = definition.GetAction(currentMethodId);
                SomeData inputs = GetInputsForFunction(action);
                IWorkflowFunctionInstance instance = instanceFactory.GetFunctionInstance(action.Name);
                return instance.Run(inputs);
            }
        }

        public WorkflowInstanceData Export()
        {
            return new WorkflowInstanceData()
            {
                Id = this.Id,
                WorkflowDefintitionId = this.WorkflowDefintitionId,
                State = this.State,
                CurrentMethodId = this.CurrentMethodId,
                Input = workflowInput,
                Variables = variables,
                MethodData = methodData,
                MethodOutput = methodOutput
            };
        }
    }
}
