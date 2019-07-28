using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conductor.Domain.Interfaces;
using WorkflowCore.Interface;

namespace Conductor.Domain.Services
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly IScriptEngineHost _scriptHost;

        public ExpressionEvaluator(IScriptEngineHost scriptHost)
        {
            _scriptHost = scriptHost;
        }
        
        public object EvaluateExpression(string sourceExpr, object pData, IStepExecutionContext pContext)
        {
            object resolvedValue = _scriptHost.EvaluateExpression(sourceExpr, new Dictionary<string, object>()
            {
                ["data"] = pData,
                ["context"] = pContext,
                ["environment"] = Environment.GetEnvironmentVariables(),
                ["readFile"] = new Func<string, byte[]>(File.ReadAllBytes),
                ["readText"] = new Func<string, Encoding, string>(File.ReadAllText)
            });
            return resolvedValue;
        }

    }
}