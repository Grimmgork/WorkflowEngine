using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;
using Workflows.Function;
using Workflows.Method;

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
        public int OnError;
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

        public WorkflowActionNode Input(string name, SomeData source)
        {
            inputs.Add(new WorkflowInputNode(this, name, source));
            return this;
        }

        public WorkflowActionNode Input(string name, Func<WorkflowFunctionContext, SomeData> func)
        {
            inputs.Add(new WorkflowInputNode(this, name, SomeData.Expression(func)));
            return this;
        }

        public WorkflowActionNode Error(int methodId)
        {
            OnError = methodId;
            return this;
        }

        public WorkflowActionNode Then(int methodId)
        {
            inputs.Add(new WorkflowInputNode(this, "Then", SomeData.Method(methodId)));
            return this;
        }

        public WorkflowActionNode Then(string route, int methodId)
        {
            inputs.Add(new WorkflowInputNode(this, route, SomeData.Method(methodId)));
            return this;
        }
    }
}
