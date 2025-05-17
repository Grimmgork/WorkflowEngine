using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Function
{
    public class WorkflowFunctionContext
    {
        public readonly IReadOnlyDictionary<string, SomeData> Variables;

        private readonly IDictionary<int, SomeDataStruct> methodOutputs;

        public WorkflowFunctionContext(SomeDataStruct variables, IDictionary<int, SomeDataStruct> methodOutputs)
        {
            Variables = variables;
            this.methodOutputs = methodOutputs;
        }

        public SomeData GetMethodOutput(int id, string name)
        {
            return methodOutputs[id][name];
        }
    }
}
