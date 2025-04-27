// See https://aka.ms/new-console-template for more information
using Workflows;
using Workflows.Data;
using Workflows.Function;
using Workflows.Message;
using Workflows.Method;
using Workflows.Model;
using Workflows.Signal;

WorkflowDefinition definition = new WorkflowDefinition("workflow", 1);
definition.AddMethod(1, "Print")
    .Input("Message", SomeData.FromString("Enter a message ..."))
    .OnNext(SomeData.FromMethod(2));

definition.AddMethod(2, "WriteVariable")
    .Input("Name", SomeData.FromString("MyVariable"))
    .Input("Value", SomeData.FromInt32(42))
    .OnNext(SomeData.FromMethod(3));

definition.AddMethod(3, "WaitForInput")
    .OnNext(SomeData.FromMethod(4));

definition.AddMethod(4, "Print")
    .Input("Message", SomeData.FromVariable("MyVariable"));

IWorkflowSignalHandler signalHandler = new ConsoleSignalHandler();
DefaultWorkflowFunctionInstanceFactory instanceFactory = new DefaultWorkflowFunctionInstanceFactory();

instanceFactory.Register("WriteVariable", () => new WriteVariableMethod());
instanceFactory.Register("Print", () => new PrintMethod());
instanceFactory.Register("WaitForInput", () => new WaitForInputMethod());
instanceFactory.Register("If", () => new WorkflowFunction((inputs) => inputs["Condition"].ToBoolean() ? inputs["Then"] : inputs["Else"]));

WorkflowInstance instance = new WorkflowInstance(1, definition, instanceFactory, (signal) =>
    Task.Run(() => Console.WriteLine($"SIGNAL: {signal.GetType()}"))
);
do
{
    if (instance.State == WorkflowInstanceState.Suspended)
    {
        string message = Console.ReadLine() ?? "";
        instance.SendSignal(new PromptInputSignal(message, DateTime.Now));
    }
    else
    {
        await instance.StepAsync();
    }
}
while (instance.State != WorkflowInstanceState.Done);
Console.WriteLine("Done!");
