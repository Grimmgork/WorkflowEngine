using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public enum WorkflowDataType
    {
        Error,
        Undef,
        Null,
        Int,
        Bool,
        String,
        Array,
        Object,
        OutputRef,
        FunctionRef,
        EventRef
    }
}
