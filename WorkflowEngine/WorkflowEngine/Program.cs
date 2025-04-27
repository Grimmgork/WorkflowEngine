// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using Workflows;
using Workflows.Data;
using Workflows.Function;
using Workflows.Method;
using Workflows.Model;

WorkflowDefinition definition = new WorkflowDefinition("workflow", 1);
definition.AddMethod(1, "Print")
    .Input("Message", SomeData.FromString("Enter a message ..."))
    .OnNext(SomeData.FromMethodRef(2));

definition.AddMethod(2, "WaitForInput")
    .OnNext(SomeData.FromMethodRef(3));

definition.AddMethod(3, "Print")
    .Input("Message", SomeData.FromOutputRef(2, "Result"));

IWorkflowSignalHandler signalHandler = new ConsoleSignalHandler();
DefaultWorkflowFunctionInstanceFactory instanceFactory = new DefaultWorkflowFunctionInstanceFactory();

instanceFactory.Register("Print", () => new PrintMethod());
instanceFactory.Register("WaitForInput", () => new WaitForInputMethod());
instanceFactory.Register("If", () => new WorkflowFunction((inputs) => inputs["Condition"].ToBoolean() ? inputs["Then"] : inputs["Else"]));

WorkflowInstance instance = new WorkflowInstance(1, definition, instanceFactory, signalHandler);
do
{
    await instance.StepAsync();
}
while (instance.State != WorkflowInstanceState.Done);
Console.WriteLine("Done!");
