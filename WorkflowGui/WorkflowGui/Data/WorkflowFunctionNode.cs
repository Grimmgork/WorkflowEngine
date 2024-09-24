namespace WorkflowGui.Data
{
    public class WorkflowFunctionNode
    {
        public int Id { get; init; }

        public int XPos { get; set; }

        public int YPos { get; set; }

        public string FunctionName { get; init; }

        public WorkflowFunctionSignature Signature { get; init; }
    }

    public class WorkflowFunctionSignature
    {
        public string FunctionName { get; init; } = "";

        public IEnumerable<string> Inputs => inputs;

        private List<string> inputs = new List<string>();

        public WorkflowFunctionSignature()
        {
            
        }

        public static WorkflowFunctionSignature Create(string name)
        {
            return new WorkflowFunctionSignature() { FunctionName = name };
        }

        public WorkflowFunctionSignature AddInput(string name)
        {
            inputs.Add(name);
            return this;
        }
    }
}
