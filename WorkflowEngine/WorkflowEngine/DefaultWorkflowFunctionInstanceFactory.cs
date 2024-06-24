using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workflows
{
    public class DefaultWorkflowFunctionInstanceFactory : IWorkflowFunctionInstanceFactory
    {
        public IWorkflowFunctionInstance GetNewInstance(string name)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                WorkflowFunctionNameAttribute? attribute = type.GetCustomAttribute<WorkflowFunctionNameAttribute>();
                if (attribute != null && attribute.Name == name)
                    return (IWorkflowFunctionInstance) Activator.CreateInstance(type)!;
            }

            throw new Exception($"cannot find function with name '{name}'!");
        }
    }
}
