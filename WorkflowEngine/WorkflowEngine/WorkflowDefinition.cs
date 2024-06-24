using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowDefinition
    {
        private List<WorkflowFunctionNode> functionNodes = new List<WorkflowFunctionNode>();
        private List<WorkflowInputNode> inputNodes = new List<WorkflowInputNode>();
        private List<WorkflowOutputNode> outputNodes = new List<WorkflowOutputNode>();
        public int? EntryPointId { get; private set; }
        public string? Name { get; private set; }

        public WorkflowDefinition(string name) 
        {
            Name = name;
        }

        public WorkflowDefinition() { }

        public WorkflowDefinition CreateFunctionNode(int id, string name, bool initial = false)
        {
            WorkflowFunctionNode node = new WorkflowFunctionNode(id, name, new WorkflowValue());
            if (initial)
                EntryPointId = id;
            functionNodes.Add(node);
            return this;
        }

        public WorkflowDefinition CreateFunctionNode(int id, string name, WorkflowValue next, bool initial = false)
        {
            WorkflowFunctionNode node = new WorkflowFunctionNode(id, name, next);
            if (initial)
                EntryPointId = id;
            functionNodes.Add(node);
            return this;
        }

        public WorkflowDefinition CreateInputNode(int id, int functionId, string name, WorkflowValue value)
        {
            inputNodes.Add(new WorkflowInputNode(id, functionId, name, value));
            return this;
        }

        public WorkflowDefinition CreateOutputNode(int id, int functionId, string name)
        {
            outputNodes.Add(new WorkflowOutputNode(id, functionId, name));
            return this;
        }

        public WorkflowInputNode[] GetInputNodesForFunction(int functionId)
        {
            return inputNodes.Where(node => node.FunctionId == functionId).ToArray();
        }

        public WorkflowOutputNode[] GetOutputNodesForFunction(int functionId)
        {
            return outputNodes.Where(node => node.FunctionId == functionId).ToArray();
        }

        public WorkflowInputNode GetInputNode(int id)
        {
            return inputNodes.First(node => node.Id == id);
        }

        public WorkflowInputNode? GetInputNodeForFunction(int functionId, string name)
        {
            return inputNodes.FirstOrDefault(node => node.FunctionId == functionId && node.Name == name);
        }

        public WorkflowOutputNode GetOutputNode(int id)
        {
            return outputNodes.First(node => node.Id == id);
        }

        public WorkflowFunctionNode GetFunctionNode(int id)
        {
            return functionNodes.First(node => node.Id == id);
        }

        public IEnumerable<WorkflowFunctionNode> GetFunctionNodes()
        {
            return functionNodes;
        }
    }
}
