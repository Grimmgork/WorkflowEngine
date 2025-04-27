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
        Int32,
        Int64,
        Single,
        DateTime,
        Boolean,
        String,
        Error,
        Output,
        Method,
        Array,
        Struct,
    }
}
