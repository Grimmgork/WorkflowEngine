using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows.Data;
using Workflows.Model;

namespace Workflows.Integration
{
    public class WorkflowEngineHost : IHostedService, IWorkflowEngineHost
    {
        public WorkflowEngineHost(IWorkflowDefinitionRepository definitionRepository)
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CompileWorkflowDefinition(WorkflowDefinition definition)
        {
            return Task.CompletedTask;
        }

        public Task<WorkflowInstance> StartWorkflowAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<SomeDataStruct> ExecuteWorkflowAsync(int id, SomeDataStruct input, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
