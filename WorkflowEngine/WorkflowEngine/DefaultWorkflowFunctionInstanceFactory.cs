using Microsoft.Extensions.DependencyInjection;
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
        private Dictionary<string, Func<IWorkflowFunctionInstance>> builders = new Dictionary<string, Func<IWorkflowFunctionInstance>>();

        public DefaultWorkflowFunctionInstanceFactory()
        {
            
        }

        public virtual IWorkflowFunctionInstance GetNewInstance(string name)
        {
            return builders[name].Invoke();
        }

        public void RegisterFunction(string name, Func<IWorkflowFunctionInstance> build)
        {
            builders.Add(name, build);
        }
    }
}
