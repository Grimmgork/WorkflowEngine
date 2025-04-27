using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Model
{
    public class WorkflowInputNode
    {
        public readonly string Name;
        public readonly SomeData Source;
        public readonly WorkflowActionNode Function;

        public WorkflowInputNode(WorkflowActionNode function, string name, SomeData? source = null)
        {
            Name = name;
            Function = function;
            Source = source == null ? new SomeData() : source.Value;
        }
    }
}
