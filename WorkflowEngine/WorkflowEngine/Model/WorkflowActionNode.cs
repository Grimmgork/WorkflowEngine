using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Model
{
    public enum WorkflowActionNodeType
    {
        Method,
        Function
    }

    public class WorkflowActionNode
    {
        public WorkflowActionNodeType Type;
        public int Id;
        public string Name = "";
        public SomeData Next;
        public SomeData Error;
        public int XPos;
        public int YPos;

        public WorkflowDefinition Workflow;

        public WorkflowActionNode(WorkflowDefinition workflow, WorkflowActionNodeType type)
        {
            this.Type = type;
            this.Workflow = workflow;
        }

        private ICollection<WorkflowInputNode> inputs = new List<WorkflowInputNode>();

        public IEnumerable<WorkflowInputNode> Inputs => inputs;

        public WorkflowActionNode Input(string name, SomeData source = default)
        {
            inputs.Add(new WorkflowInputNode(this, name, source));
            return this;
        }

        public WorkflowActionNode OnError(SomeData method)
        {
            Error = method;
            return this;
        }

        public WorkflowActionNode OnNext(SomeData method)
        {
            Next = method;
            return this;
        }
    }
}
