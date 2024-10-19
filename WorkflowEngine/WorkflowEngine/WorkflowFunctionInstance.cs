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

    public abstract class WorkflowFunctionInstanceBase : IWorkflowFunctionInstance
    {
        public int Id { get; set; }

        public virtual void Dispose()
        {
            // do nothing
        }

        public virtual WorkflowValueObject Run(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public virtual Task<WorkflowValueObject> RunAsync(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            return Task.FromResult(Run(context, input, token));
        }
    }

    public class PureWorkflowFunctionInstance : WorkflowFunctionInstanceBase
    {
        private Func<WorkflowValueObject, WorkflowValueObject> run;
        public PureWorkflowFunctionInstance(Func<WorkflowValueObject, WorkflowValueObject> run)
        {
            this.run = run;
        }

        public PureWorkflowFunctionInstance(Func<WorkflowValueObject, WorkflowValue> run)
        {
            this.run = (inputs) => {
                WorkflowValueObject outputs = new WorkflowValueObject();
                outputs["result"] = run.Invoke(inputs);
                return outputs;
            };
        }

        public sealed override WorkflowValueObject Run(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            return run.Invoke(input);
        }
    }

    public class WorkflowFunctionInstancePrint : WorkflowFunctionInstanceBase
    {
        public override WorkflowValueObject Run(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            Console.WriteLine(input["value"]);
            return new WorkflowValueObject();
        }
    }

    public class WorkflowFunctionHttpFetch : WorkflowFunctionInstanceBase
    {
        public override async Task<WorkflowValueObject> RunAsync(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            // HttpClient client = new HttpClient();
            // json = await client.GetStringAsync(url, token);

            return new WorkflowValueObject();
        }
    }

    public class WorkflowFunctionInstanceWaitForConfirmation : WorkflowFunctionInstanceBase
    {
        public override async Task<WorkflowValueObject> RunAsync(IWorkflowMessageHandler context, WorkflowValueObject input, CancellationToken token)
        {
            await context.WriteMessage(new CustomWorkflowMessage("asdf"));
            while (true)
            {
                WorkflowMessage message = await context.ReadMessage(token);
                if (message is CustomWorkflowMessage)
                {
                    break;
                }
            }
            return new WorkflowValueObject();
        }
    }
}
