using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conductor.Domain.Interfaces;
using Newtonsoft.Json.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class Lambda : StepBodyAsync
    {
        private ILambdaService _service;

        public string Name { get; set; }
        public IDictionary<string, object> Variables { get; set; }

        public Lambda(ILambdaService service)
        {
            _service = service;
        }

        public async override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var vars = new Dictionary<string, object>();

            if (Variables != null)
                vars = new Dictionary<string, object>(Variables);

            _service.ExecuteLambda(Name, vars);

            Variables = new Dictionary<string, object>(vars);

            return ExecutionResult.Next();

        }
    }
    
}
