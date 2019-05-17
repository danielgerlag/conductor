using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class HttpRequest : StepBodyAsync
    {
        public async override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
