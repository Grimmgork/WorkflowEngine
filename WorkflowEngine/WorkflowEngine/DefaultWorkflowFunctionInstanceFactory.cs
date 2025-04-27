using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Workflows.Function;
using Workflows.Method;

namespace Workflows
{
    public class DefaultWorkflowFunctionInstanceFactory : IWorkflowFunctionInstanceFactory
    {
        private Dictionary<string, Func<IWorkflowMethodInstance>> methods = new Dictionary<string, Func<IWorkflowMethodInstance>>();
        private Dictionary<string, Func<IWorkflowFunctionInstance>> functions = new Dictionary<string, Func<IWorkflowFunctionInstance>>();

        public DefaultWorkflowFunctionInstanceFactory()
        {
            
        }

        public IWorkflowMethodInstance GetMethodInstance(string name)
        {
            return methods[name].Invoke();
        }

        public IWorkflowFunctionInstance GetFunctionInstance(string name)
        {
            return functions[name].Invoke();
        }

        public void Register(string name, Func<IWorkflowFunctionInstance> build)
        {
            functions.Add(name, build);
        }

        public void Register(string name, Func<IWorkflowMethodInstance> build)
        {
            methods.Add(name, build);
        }
    }
}
