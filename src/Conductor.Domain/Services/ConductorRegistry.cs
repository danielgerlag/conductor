using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Domain.Services
{
    public class ConductorRegistry : IWorkflowRegistry
    {
        private readonly List<Tuple<string, int, WorkflowDefinition>> _registry = new List<Tuple<string, int, WorkflowDefinition>>();

        public ConductorRegistry()
        {
            
        }

        public WorkflowDefinition GetDefinition(string workflowId, int? version = null)
        {
            if (version.HasValue)
            {
                var entry = _registry.FirstOrDefault(x => x.Item1 == workflowId && x.Item2 == version.Value);
                // TODO: What in the heck does Item3 mean?
                return entry?.Item3;
            }
            else
            {
                var entry = _registry.Where(x => x.Item1 == workflowId).OrderByDescending(x => x.Item2)
                                     .FirstOrDefault();
                return entry?.Item3;
            }
        }

        public void RegisterWorkflow(WorkflowDefinition definition)
        {
            if (_registry.Any(x => x.Item1 == definition.Id && x.Item2 == definition.Version))
            {
                throw new InvalidOperationException($"Workflow {definition.Id} version {definition.Version} is already registered");
            }

            _registry.Add(Tuple.Create(definition.Id, definition.Version, definition));
        }

        public void RegisterWorkflow(IWorkflow workflow)
        {
            throw new NotImplementedException();
        }

        public void RegisterWorkflow<TData>(IWorkflow<TData> workflow)
            where TData : new()
        {
            throw new NotImplementedException();
        }
    }
}
