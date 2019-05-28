using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class EmitLog : StepBodyAsync
    {
        public string Message { get; set; }

        public override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Console.WriteLine(Message);

            return Task.FromResult(ExecutionResult.Next());
        }
    }
}
