using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;

namespace Workflows.Model
{
    public class WorkflowDefinition
    {
        public readonly int Id;

        public readonly string Name;

        public readonly int EntryPointId;

        private List<WorkflowActionNode> actionNodes = new List<WorkflowActionNode>();

        public WorkflowDefinition(string name, int entryPointId)
        {
            Name = name;
            EntryPointId = entryPointId;
        }

        public WorkflowActionNode GetAction(int id)
        {
            return actionNodes.First(i => i.Id == id);
        }

        public WorkflowActionNode AddFunction(int id, string name)
        {
            WorkflowActionNode node = new WorkflowActionNode(this, WorkflowActionNodeType.Function)
            {
                Id = id,
                Name = name
            };
            actionNodes.Add(node);
            return node;
        }

        public WorkflowActionNode AddMethod(int id, string name)
        {
            WorkflowActionNode node = new WorkflowActionNode(this, WorkflowActionNodeType.Method)
            {
                Id = id,
                Name = name
            };
            actionNodes.Add(node);
            return node;
        }
    }
}
