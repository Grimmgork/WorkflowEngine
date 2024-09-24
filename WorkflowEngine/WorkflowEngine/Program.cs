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
WorkflowFunctionInstanceFactory factory = new WorkflowFunctionInstanceFactory();
WorkflowInstance instance = new WorkflowInstance(definition, factory, messageHandler);

while (true)
{
    if (instance.State == WorkflowInstanceState.Running || instance.State == WorkflowInstanceState.Initial)
    {
        await instance.Run();
    }

    if (instance.State == WorkflowInstanceState.Waiting)
    {
        await instance.SendMessage(new CustomWorkflowMessage(Console.ReadLine() ?? ""));
    }

    if (instance.State == WorkflowInstanceState.Done)
        break;
}

Console.WriteLine("Done!");