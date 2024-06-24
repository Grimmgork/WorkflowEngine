using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowOutputNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FunctionId { get; set; }

        public WorkflowOutputNode(int id, int functionId, string name)
        {
            this.Id = id;
            this.Name = name;
            this.FunctionId = functionId;
        }
    }
}
