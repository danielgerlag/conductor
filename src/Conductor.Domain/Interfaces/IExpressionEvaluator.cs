using System.Collections.Generic;
using WorkflowCore.Interface;

namespace Conductor.Domain.Interfaces
{
    public interface IExpressionEvaluator
    {
        object EvaluateExpression(string sourceExpr, object pData, IStepExecutionContext pContext);
        object EvaluateExpression(string sourceExpr, IDictionary<string, object> parameteters);
        bool EvaluateOutcomeExpression(string sourceExpr, object data, object outcome);
    }
}