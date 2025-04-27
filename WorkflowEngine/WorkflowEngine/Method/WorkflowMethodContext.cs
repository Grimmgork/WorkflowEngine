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

        public Task RaiseSignal(WorkflowSignal message)
        {
            Console.WriteLine($"SIGNAL: {message.GetType().Name}");
            return Task.CompletedTask;
        }

        public WorkflowMethodContext(IDictionary<string, SomeData> variables, SomeData input)
        {
            Variables = variables;
            Input = input;
        }

        public WorkflowMethodContext(IDictionary<string, SomeData> variables, SomeData data, SomeData input)
        {
            Variables = variables;
            Input = input;
            Data = data;
        }
    }
}
