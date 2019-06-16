using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class Add : StepBodyAsync
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Result { get; set; }

        public override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            Result = Value1 + Value2;
            return Task.FromResult(ExecutionResult.Next());
        }
    }
}
