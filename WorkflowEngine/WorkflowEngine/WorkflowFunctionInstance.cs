using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowMessage
    {
        public readonly WorkflowValueObject Data;

        public WorkflowMessage(WorkflowValueObject? data = null)
        {
            this.Data = data ?? new WorkflowValueObject();
        }
    }

    public class CustomWorkflowMessage : WorkflowMessage
    {
        public readonly string Name;

        public CustomWorkflowMessage(string name, WorkflowValueObject? data = null) : base(data)
        {
            Name = name;
        }
    }

    public enum WorkflowFunctionState
    {
        Init,
        Running,
        Waiting,
        Done
    }

    public abstract class WorkflowFunctionInstanceBase : IWorkflowFunctionInstance
    {
        public int Id { get; set; }

        public virtual void SetInput(WorkflowValueObject inputs)
        {
            // do nothing
        }

        public virtual WorkflowValueObject GetOutput()
        {
            return new WorkflowValueObject();
        }

        public virtual WorkflowFunctionState Run(WorkflowInstanceContext context, CancellationToken token)
        {
            return WorkflowFunctionState.Done;
        }

        public virtual Task<WorkflowFunctionState> RunAsync(WorkflowInstanceContext context, CancellationToken token)
        {
            return Task.FromResult(Run(context, token));
        }

        public virtual WorkflowFunctionState HandleMessage(WorkflowMessage message, CancellationToken token)
        {
            return WorkflowFunctionState.Running;
        }

        public virtual Task<WorkflowFunctionState> HandleMessageAsync(WorkflowMessage message, CancellationToken token)
        {
            return Task.FromResult(HandleMessage(message, token));
        }

        public virtual void Dispose()
        {
            // do nothing
        }
    }

    public abstract class PureWorkflowFunctionInstanceBase : WorkflowFunctionInstanceBase
    {
        private Func<WorkflowValueObject, WorkflowValueObject> run;
        private WorkflowValueObject inputs = new WorkflowValueObject();
        private WorkflowValueObject outputs = new WorkflowValueObject();

        public PureWorkflowFunctionInstanceBase(Func<WorkflowValueObject, WorkflowValueObject> run)
        {
            this.run = run;
        }

        public PureWorkflowFunctionInstanceBase(Func<WorkflowValueObject, WorkflowValue> run)
        {
            this.run = (inputs) =>
            {
                WorkflowValueObject outputs = new WorkflowValueObject();
                outputs["result"] = run.Invoke(inputs);
                return outputs;
            };
        }

        public sealed override void SetInput(WorkflowValueObject inputs)
        {
            this.inputs = inputs;
        }

        public sealed override WorkflowValueObject GetOutput()
        {
            return outputs;
        }

        public sealed override WorkflowFunctionState Run(WorkflowInstanceContext context, CancellationToken token)
        {
            outputs = run.Invoke(inputs);
            return WorkflowFunctionState.Done;
        }
    }

    public sealed class PureWorkflowFunctionInstance : PureWorkflowFunctionInstanceBase
    {
        public static PureWorkflowFunctionInstance FromLambda(Func<WorkflowValueObject, WorkflowValueObject> run) => new PureWorkflowFunctionInstance(run);
        public static PureWorkflowFunctionInstance FromLambda(Func<WorkflowValueObject, WorkflowValue> run) => new PureWorkflowFunctionInstance(run);

        private PureWorkflowFunctionInstance(Func<WorkflowValueObject, WorkflowValueObject> run) : base(run) { }

        private PureWorkflowFunctionInstance(Func<WorkflowValueObject, WorkflowValue> run) : base(run) { }
    }

    public class WorkflowFunctionInstancePrint : WorkflowFunctionInstanceBase
    {
        WorkflowValueObject inputs = WorkflowValueObject.Empty;

        public override void SetInput(WorkflowValueObject inputs)
        {
            this.inputs = inputs;
        }

        public override WorkflowFunctionState Run(WorkflowInstanceContext context, CancellationToken token)
        {
            Console.WriteLine(inputs["value"]);
            return WorkflowFunctionState.Done;
        }
    }

    public class WorkflowFunctionLongProcess : WorkflowFunctionInstanceBase
    {
        private string url = "";
        private string json = "";

        public override void SetInput(WorkflowValueObject inputs)
        {
            url = inputs["url"].String();
        }

        public override async Task<WorkflowFunctionState> RunAsync(WorkflowInstanceContext context, CancellationToken token)
        {
            HttpClient client = new HttpClient();
            json = await client.GetStringAsync(url, token);
            return WorkflowFunctionState.Done;
        }

        public override WorkflowValueObject GetOutput()
        {
            WorkflowValueObject result = new WorkflowValueObject();
            result["json"] = WorkflowValue.String(json);
            return result;
        }
    }

    public class WorkflowFunctionInstanceWaitForConfirmation : WorkflowFunctionInstanceBase
    {
        public override WorkflowFunctionState Run(WorkflowInstanceContext context, CancellationToken token)
        {
            context.SendMessage(new CustomWorkflowMessage("asdf"));
            return WorkflowFunctionState.Waiting;
        }

        public override WorkflowFunctionState HandleMessage(WorkflowMessage message, CancellationToken token)
        {
            if (message is CustomWorkflowMessage custom)
            {
                if (custom.Name.Trim() == "Confirm")
                {
                    return WorkflowFunctionState.Done;
                }
            }

            return WorkflowFunctionState.Waiting;
        }
    }
}
