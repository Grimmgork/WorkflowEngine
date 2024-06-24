// See https://aka.ms/new-console-template for more information
using Workflows;

WorkflowDefinition workflow = new WorkflowDefinition("add_numbers");

workflow.CreateFunctionNode(1, "print", WorkflowValue.OutputNode(3), true);
workflow.CreateInputNode(2, 1, "value", WorkflowValue.String("Hello world!"));

workflow.CreateFunctionNode(3, "print");
workflow.CreateInputNode(6, 3, "value", WorkflowValue.String("lel"));

workflow.CreateFunctionNode(4, "print");
workflow.CreateInputNode(7, 4, "value", WorkflowValue.String("kek"));

workflow.CreateFunctionNode(5, "if");
workflow.CreateInputNode(8, 5, "condition", WorkflowValue.Bool(false));
workflow.CreateInputNode(9, 5, "then", WorkflowValue.Function(4));
workflow.CreateInputNode(10, 5, "else", WorkflowValue.Function(3));
workflow.CreateOutputNode(3, 5, "result");

WorkflowInstance instance = new WorkflowInstance(workflow, new DefaultWorkflowFunctionInstanceFactory());
while(await instance.Iterate())
{
    // Console.WriteLine("iterate!");
    while(instance.HasEvent)
    {
        Console.WriteLine(instance.ReadEvent().Name);
    }
}
Console.WriteLine("Done!");
