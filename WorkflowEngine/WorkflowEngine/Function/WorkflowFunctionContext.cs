using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Function
{
    internal class WorkflowFunctionContext
    {
        public readonly IDictionary<string, SomeData> Variables;

        public WorkflowFunctionContext(IDictionary<string, SomeData> variables)
        {
            Variables = variables;
        }
    }
}
