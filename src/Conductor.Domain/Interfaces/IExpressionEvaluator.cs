using WorkflowCore.Interface;

namespace Conductor.Domain.Services
{
    public interface IExpressionEvaluator
    {
        object EvaluateExpression(string sourceExpr, object pData, IStepExecutionContext pContext);
    }
}