using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;
using Workflows.Function;
using Workflows.Message;

namespace Workflows.Method
{
    public class WorkflowMethodContext
    {
        public readonly SomeData Input;

        public SomeData Output;

        public SomeData Data;

        public readonly IDictionary<string, SomeData> Variables;

        private Func<WorkflowSignal, Task> sendSignal;

        public Task RaiseSignal(WorkflowSignal message)
        {
            return sendSignal(message);
        }

        public WorkflowMethodContext(IDictionary<string, SomeData> variables, SomeData input, Func<WorkflowSignal, Task> sendSignal)
        {
            Variables = variables;
            Input = input;
            this.sendSignal = sendSignal;
        }

        public WorkflowMethodContext(IDictionary<string, SomeData> variables, SomeData data, SomeData input, Func<WorkflowSignal, Task> sendSignal)
        {
            Variables = variables;
            Input = input;
            Data = data;
            this.sendSignal = sendSignal;
        }
    }
}
