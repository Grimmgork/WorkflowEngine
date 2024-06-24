using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowInputNode
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int FunctionId { get; set; }
        public WorkflowValue Source { get; set; }

        public WorkflowInputNode(int id, int functionId, string name, WorkflowValue? source = null)
        {
            this.Id = id;
            this.Name = name;
            this.FunctionId = functionId;
            this.Source = source == null ? new WorkflowValue() : source.Value;
        }
    }
}
