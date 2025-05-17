// See https://aka.ms/new-console-template for more information
using Workflows;
using Workflows.Data;
using Workflows.Function;
using Workflows.Method;
using Workflows.Model;
using Workflows.Signal;

WorkflowDefinition definition = new WorkflowDefinition("workflow", 1);
definition.Method(1, "Print")
    .Input("Message", SomeData.String("Enter a message ..."))
    .Then(2);

definition.Method(2, "WriteVariable")
    .Input("Name", SomeData.String("MyVariable"))
    .Input("Value", (context) => SomeData.Integer(1 + 2))
    .Then(3);

definition.Method(3, "WaitForInput")
    .Then(4)
    .Error(5);

definition.Method(4, "Print")
    .Input("Message", (context) => context.GetMethodOutput(3, "Result"));

definition.Method(5, "Print")
    .Input("Message", SomeData.String("An error occured!"));

DefaultWorkflowFunctionInstanceFactory instanceFactory = new DefaultWorkflowFunctionInstanceFactory();
instanceFactory.Register("WriteVariable", () => new WriteVariableMethod());
instanceFactory.Register("Print", () => new PrintMethod());
instanceFactory.Register("WaitForInput", () => new WaitForInputMethod());
instanceFactory.Register("If", () => new IfMethod());
instanceFactory.Register("If", () => new WorkflowFunction((inputs) => inputs["Condition"].ToBoolean() ? inputs["Then"] : inputs["Else"]));
instanceFactory.Register("+", () => new WorkflowFunction((inputs) => inputs["A"].ToDynamic() + inputs["B"].ToDynamic()));

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

    await instance.RunAsync();
}
while (instance.State != WorkflowInstanceState.Done);
Console.WriteLine("Done!");

WorkflowInstanceData data = instance.Export();
Console.WriteLine(data.Variables["MyVariable"].ToInt32());
