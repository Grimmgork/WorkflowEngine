using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowEngine
    {

        public WorkflowEngine()
        {

        }

        public Dictionary<string, WorkflowValue> RunWorkflow(WorkflowDefinition workflow)
        {
            WorkflowFunctionNode? functionNode = workflow.GetFunctionNode(workflow.EntryPointId!.Value);
            while (true)
            {
                Dictionary<string, WorkflowValue> outputs = ExecuteFunction(workflow, functionNode!);
                WorkflowValue next = ResolveOutputNodeToLiteral(workflow, functionNode.Next);
                if (next.Type == WorkflowDataType.FunctionRef)
                {
                    functionNode = workflow.GetFunctionNode(next.AsFunctionRef());
                    continue;
                }

                if (next.Type == WorkflowDataType.Undef)
                {
                    return outputs;
                }

                throw new Exception("next parameter must be of type FunctionRef or Undef");
            }
        }

        private Dictionary<string, WorkflowValue> GetInputsForFunctionNode(WorkflowDefinition workflow, WorkflowFunctionNode functionNode)
        {
            Dictionary<string, WorkflowValue> inputValues = new Dictionary<string, WorkflowValue>();
            WorkflowInputNode[] inputNodes = workflow.GetInputNodesForFunction(functionNode.Id);
            if (inputNodes.Length == 0)
                return inputValues;

            foreach(WorkflowInputNode input in inputNodes)
            {
                inputValues.Add(input.Name, ResolveOutputNodeToLiteral(workflow, input.Source));
            }

            return inputValues;
        }

        private WorkflowValue ResolveOutputNodeToLiteral(WorkflowDefinition workflow, WorkflowValue value)
        {
            if (value.Type != WorkflowDataType.OutputNode)
                return value;

            WorkflowOutputNode outputNode = workflow.GetOutputNode((int)value.Value!);
            WorkflowFunctionNode functionNode = workflow.GetFunctionNode(outputNode.FunctionId);
            Dictionary<string, WorkflowValue> outputValues = ExecuteFunction(workflow, functionNode);
            return outputValues[outputNode.Name];
        }

        private Dictionary<string, WorkflowValue> ExecuteFunction(WorkflowDefinition workflow, WorkflowFunctionNode function)
        {
            Func<Dictionary<string, WorkflowValue>, Dictionary<string, WorkflowValue>> handler = Functions[function.FunctionName];
            return handler.Invoke(GetInputsForFunctionNode(workflow, function));
        }

        public WorkflowEngine RegisterFunction(string name, Func<Dictionary<string, WorkflowValue>, Dictionary<string, WorkflowValue>> func)
        {
            Functions.Add(name, func);
            return this;
        }
    }

  
}
