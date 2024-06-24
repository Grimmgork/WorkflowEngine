using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{

    public interface IWorkflowFunctionInstance
    {
        public int Id { get; set; }
        public Task<WorkflowFunctionResult> Run();

        public void SetInput(string name, WorkflowValue value);

        public WorkflowValue GetOutput(string name);

        public WorkflowValueObject GetOutputs();

        public void HandleEvent(WorkflowEventArgs args);
    }

    public enum WorkflowFunctionResultStatus
    {
        Done,
        Again,
        Error,
    }

    public class WorkflowFunctionResult
    {
        public bool IsError => Status == WorkflowFunctionResultStatus.Error;

        public bool IsDone => Status == WorkflowFunctionResultStatus.Done;

        public WorkflowFunctionResultStatus Status { get; private set; }
        public IEnumerable<WorkflowEventArgs> Events => events;

        private List<WorkflowEventArgs> events = new List<WorkflowEventArgs>();

        public WorkflowFunctionResult(WorkflowFunctionResultStatus status = WorkflowFunctionResultStatus.Done) { Status = status; }

        public static WorkflowFunctionResult Done(params WorkflowEventArgs[] eventArgs)
        {
            WorkflowFunctionResult result = new WorkflowFunctionResult(WorkflowFunctionResultStatus.Done);
            foreach (WorkflowEventArgs args in eventArgs)
                result.AddEvent(args);
            return result;
        }

        public static WorkflowFunctionResult Error(params WorkflowEventArgs[] eventArgs)
        {
            WorkflowFunctionResult result = new WorkflowFunctionResult(WorkflowFunctionResultStatus.Error);
            foreach (WorkflowEventArgs args in eventArgs)
                result.AddEvent(args);
            return result;
        }

        public static WorkflowFunctionResult Again(params WorkflowEventArgs[] eventArgs)
        {
            WorkflowFunctionResult result = new WorkflowFunctionResult(WorkflowFunctionResultStatus.Again);
            foreach (WorkflowEventArgs args in eventArgs)
                result.AddEvent(args);
            return result;
        }

        public void AddEvent(WorkflowEventArgs args)
        {
            events.Add(args);
        }

        public void Done()
        {
            Status = WorkflowFunctionResultStatus.Done;
        }

        public void Error()
        {
            Status = WorkflowFunctionResultStatus.Error;
        }

        public void Again()
        {
            Status = WorkflowFunctionResultStatus.Again;
        }
    }

    public class WorkflowEventArgs
    {
        public string Name;
        public WorkflowValueObject Data;
    }

    public abstract class WorkflowFunctionInstanceBase : IWorkflowFunctionInstance
    {
        public int Id { get; set; }

        protected WorkflowValueObject inputs = new WorkflowValueObject();
        protected WorkflowValueObject outputs = new WorkflowValueObject();

        public WorkflowValue GetOutput(string name)
        {
            return inputs[name];
        }

        public WorkflowValueObject GetOutputs()
        {
            return outputs;
        }

        public void SetInput(string name, WorkflowValue value)
        {
            inputs[name] = value;
        }

        public virtual Task<WorkflowFunctionResult> Run()
        {
            return Task.FromResult(WorkflowFunctionResult.Done());
        }

        public virtual void HandleEvent(WorkflowEventArgs args)
        {
            // do nothing
        }
    }

    public class WorkflowFunctionNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public WorkflowFunctionNameAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class WorkflowFunctionIsStatefulAttribute : Attribute
    {

    }


    [WorkflowFunctionName("print")]
    [WorkflowFunctionIsStateful]
    public class WorkflowFunctionInstancePrint : WorkflowFunctionInstanceBase
    {
        public override Task<WorkflowFunctionResult> Run()
        {
            Console.WriteLine(inputs["value"]);
            return Task.FromResult(WorkflowFunctionResult.Done());
        }
    }

    [WorkflowFunctionName("if")]
    public class WorkflowFunctionInstanceIf : WorkflowFunctionInstanceBase
    {
        public override Task<WorkflowFunctionResult> Run()
        {
            if (!inputs["condition"].IsBool)
                return Task.FromResult(WorkflowFunctionResult.Error());

            outputs["result"] = inputs["condition"].AsBool() ? inputs["then"] : inputs["else"];
            return Task.FromResult(WorkflowFunctionResult.Done());
        }
    }
}
