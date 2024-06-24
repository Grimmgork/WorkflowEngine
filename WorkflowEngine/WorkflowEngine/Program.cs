// See https://aka.ms/new-console-template for more information
using Workflows;

WorkflowEngine engine = new WorkflowEngine();
engine.RegisterFunction("add", (inputs) =>
    new Dictionary<string, WorkflowValue> { { "result", inputs["a"] + inputs["b"] } }
);

engine.RegisterFunction("if", (inputs) => {
    Dictionary<string, WorkflowValue> outputs = new Dictionary<string, WorkflowValue>();
    outputs.Add("result", inputs["condition"].AsBool() ? inputs["then"] : inputs["else"]);
    return outputs;
});

engine.RegisterFunction("print", (inputs) => {
    Console.WriteLine(inputs["message"].ToString());
    return new Dictionary<string, WorkflowValue>();
});

WorkflowDefinition workflow = new WorkflowDefinition("add_numbers");

workflow.CreateFunctionNode(1, "print", WorkflowValue.OutputNode(3), true);
workflow.CreateInputNode(2, 1, "message", WorkflowValue.String("Hello world!"));

workflow.CreateFunctionNode(3, "print");
workflow.CreateInputNode(6, 3, "message", WorkflowValue.String("lel"));

workflow.CreateFunctionNode(4, "print");
workflow.CreateInputNode(7, 4, "message", WorkflowValue.String("kek"));

workflow.CreateFunctionNode(5, "if");
workflow.CreateInputNode(8, 5, "condition", WorkflowValue.Bool(true));
workflow.CreateInputNode(9, 5, "then", WorkflowValue.Function(4));
workflow.CreateInputNode(10, 5, "else", WorkflowValue.Function(3));
workflow.CreateOutputNode(3, 5, "result");

Dictionary<string, WorkflowValue> outputs = engine.RunWorkflow(workflow);
foreach(string name in outputs.Keys)
{
    Console.WriteLine($"{name}: {outputs[name]}");
}

// WorkflowEngine BackgroundWorker
// WorkflowInstance

// int workflowInstanceId = engine.StartWorkflow(CancellationToken, name, arguments)
// engine.GetWorkflowInstance(workflowInstanceId)
// workflowInstance.Abort(workflowInstanceId) // exits before next instruction
// workflowInstance.SupplyValue()
// workflowInstance.SkipError()
// workflowInstance.InjectValue()