using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows.Data
{
    public enum WorkflowDataType
    {
        Undefined,
        Integer,
        Float,
        Decimal,
        DateTime,
        Boolean,
        String,
        Error,
        Variable,
        Output,
        Method,
        Expression,
        Name,
        Array,
        Struct,
    }
}
