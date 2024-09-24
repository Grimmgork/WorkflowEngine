using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class WorkflowValueArray : ICollection<WorkflowValue>
    {
        private List<WorkflowValue> values = new List<WorkflowValue>();
        public int Count => values.Count;

        public bool IsReadOnly => false;

        public void Add(WorkflowValue item)
        {
            values.Add(item);
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(WorkflowValue item)
        {
            return values.Contains(item);
        }

        public void CopyTo(WorkflowValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<WorkflowValue> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        public bool Remove(WorkflowValue item)
        {
            return values.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
