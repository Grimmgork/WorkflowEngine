// See https://aka.ms/new-console-template for more information
using Workflows;

WorkflowDefinition definition = new WorkflowDefinition("add_numbers");

definition.CreateFunctionNode(1, "print", WorkflowValue.OutputNode(3), true);
definition.CreateInputNode(2, 1, "value", WorkflowValue.String("Hello world!"));

definition.CreateFunctionNode(3, "wait_for_confirmation");
// workflow.CreateInputNode(6, 3, "value", WorkflowValue.String("lel"));

definition.CreateFunctionNode(4, "print");
definition.CreateInputNode(7, 4, "value", WorkflowValue.String("kek"));

definition.CreateFunctionNode(5, "if");
definition.CreateInputNode(8, 5, "condition", WorkflowValue.Bool(false));
definition.CreateInputNode(9, 5, "then", WorkflowValue.Function(4));
definition.CreateInputNode(10, 5, "else", WorkflowValue.Function(3));
definition.CreateOutputNode(3, 5, "result");

IWorkflowMessageHandler messageHandler = new ConsoleMessageHandler();
IWorkflowFunctionInstanceFactory factory = new DefaultWorkflowFunctionInstanceFactory();
factory.RegisterFunction("print", () => new WorkflowFunctionInstancePrint());
factory.RegisterFunction("wait_for_confirmation", () => new WorkflowFunctionInstanceWaitForConfirmation());
factory.RegisterFunction("if", () => new PureWorkflowFunctionInstance((inputs) => inputs["condition"].Bool() ? inputs["then"] : inputs["else"]));

WorkflowInstance instance = new WorkflowInstance(definition, factory, messageHandler);
do
{
    await instance.Run();
}
while (instance.State == WorkflowInstanceState.Running);
Console.WriteLine("Done!");