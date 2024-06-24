using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowFunctionNode
    {
        public int Id;
        public string FunctionName;
        public WorkflowValue Next;
        public int XPos;
        public int YPos;
        
        public WorkflowFunctionNode(int id, string name, WorkflowValue next)
        {
            this.Id = id;
            this.FunctionName = name;
            this.Next = next;
        }
    }
}
