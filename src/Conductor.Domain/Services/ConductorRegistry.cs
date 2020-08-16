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
        private readonly List<(string WorkflowId, int Version, WorkflowDefinition WorkflowDefinition)> _registry = new List<(string workflowId, int version, WorkflowDefinition workflowDefinition)>();

        public ConductorRegistry()
        {
            
        }

        public void DeregisterWorkflow(string workflowId, int version)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkflowDefinition> GetAllDefinitions()
        {
            throw new NotImplementedException();
        }

        public WorkflowDefinition GetDefinition(string workflowId, int? version = null)
        {
            if (version.HasValue)
            {
                var entry = _registry.FirstOrDefault(x => x.WorkflowId == workflowId && x.Version == version.Value);
                return entry.WorkflowDefinition;
            }
            else
            {
                var entry = _registry.Where(x => x.WorkflowId == workflowId).OrderByDescending(x => x.Version)
                                     .FirstOrDefault();
                return entry.WorkflowDefinition;
            }
        }

        public bool IsRegistered(string workflowId, int version)
        {
            throw new NotImplementedException();
        }

        public void RegisterWorkflow(WorkflowDefinition definition)
        {
            if (_registry.Any(x => x.Item1 == definition.Id && x.Item2 == definition.Version))
            {
                throw new InvalidOperationException($"Workflow {definition.Id} version {definition.Version} is already registered");
            }

            _registry.Add((definition.Id, definition.Version, definition));
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
