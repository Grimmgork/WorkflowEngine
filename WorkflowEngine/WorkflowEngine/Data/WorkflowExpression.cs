using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Function;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Workflows.Data
{
    public struct WorkflowExpression
    {
        public Func<WorkflowFunctionContext, SomeData> func;

        public WorkflowExpression(Func<WorkflowFunctionContext, SomeData> func)
        {
            this.func = func;
        }


        public SomeData Evaluate(WorkflowFunctionContext context)
        {
            SomeData result = func.Invoke(context);
            if (result.DataType == WorkflowDataType.Expression)
            {
                return result.ToExpression().Evaluate(context);
            }
            else
            {
                return result;
            }
        }
    }
}
