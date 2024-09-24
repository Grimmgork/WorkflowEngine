using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowValueObject
    {
        Dictionary<string, WorkflowValue> values = new Dictionary<string, WorkflowValue>();

        public WorkflowValue this[string name]
        {
            get
            {
                if (values.ContainsKey(name))
                    return values[name];
                return new WorkflowValue(WorkflowDataType.Undef);
            }
            set
            {
                if (value.IsDefined)
                    values[name] = value;
                else
                    values.Remove(name);
            }
        }

        public IEnumerable<string> Names => values.Keys;

        public void Remove(string name)
        {
            values.Remove(name);
        }

        public static WorkflowValueObject Empty => new WorkflowValueObject();
    }
}
