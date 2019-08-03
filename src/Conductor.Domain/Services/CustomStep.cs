using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Domain.Services
{
    public class CustomStep : StepBodyAsync
    {
        private ICustomStepService _service;

        public Dictionary<string, object> _variables { get; set; } = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get => _variables[propertyName];
            set => _variables[propertyName] = value;
        }

        public CustomStep(ICustomStepService service)
        {
            _service = service;
        }

        public async override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var resource =_service.GetStepResource(Convert.ToString(_variables["__custom_step__"]));
            
            _service.Execute(resource, _variables);
            return ExecutionResult.Next();
        }
    }
}
