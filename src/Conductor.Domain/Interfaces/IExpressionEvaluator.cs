using WorkflowCore.Interface;

namespace Conductor.Domain.Interfaces
{
    public interface IExpressionEvaluator
    {
        object EvaluateExpression(string sourceExpr, object pData, IStepExecutionContext pContext);
    }
}