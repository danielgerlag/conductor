using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class EmitLog : StepBodyAsync
    {
        private readonly ILoggerFactory _loggerFactory;

        public string Message { get; set; }

        public LogLevel Level { get; set; } = LogLevel.Information;

        public EmitLog(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var logger = _loggerFactory.CreateLogger(context.Workflow.WorkflowDefinitionId);
            logger.Log(Level, default(EventId), Message, null, (state, ex) => state);
            return Task.FromResult(ExecutionResult.Next());
        }
    }
}
