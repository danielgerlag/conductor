using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Newtonsoft.Json.Linq;
using WorkflowCore.Exceptions;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;

namespace Conductor.Domain.Services
{
    
    public class WorkflowLoader : IWorkflowLoader
    {
        private readonly IWorkflowRegistry _registry;

        public WorkflowLoader(IWorkflowRegistry registry)
        {
            _registry = registry;
        }

        public void LoadDefinition(Definition source)
        {
            var def = Convert(source);
            _registry.RegisterWorkflow(def);
        }

        private WorkflowDefinition Convert(Definition source)
        {
            var dataType = typeof(JObject);

            var result = new WorkflowDefinition
            {
                Id = source.Id,
                Version = source.Version,
                Steps = ConvertSteps(source.Steps, dataType),
                DefaultErrorBehavior = source.DefaultErrorBehavior,
                DefaultErrorRetryInterval = source.DefaultErrorRetryInterval,
                Description = source.Description,
                DataType = dataType
            };

            return result;
        }


        private WorkflowStepCollection ConvertSteps(ICollection<Step> source, Type dataType)
        {
            var result = new WorkflowStepCollection();
            int i = 0;
            var stack = new Stack<Step>(source.Reverse());
            var parents = new List<Step>();
            var compensatables = new List<Step>();

            while (stack.Count > 0)
            {
                var nextStep = stack.Pop();

                var stepType = FindType(nextStep.StepType);
                var containerType = typeof(WorkflowStep<>).MakeGenericType(stepType);
                var targetStep = (containerType.GetConstructor(new Type[] { }).Invoke(null) as WorkflowStep);

                if (nextStep.Saga)
                {
                    containerType = typeof(SagaContainer<>).MakeGenericType(stepType);
                    targetStep = (containerType.GetConstructor(new Type[] { }).Invoke(null) as WorkflowStep);
                }

                if (!string.IsNullOrEmpty(nextStep.CancelCondition))
                {
                    var cancelExprType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(dataType, typeof(bool)));
                    var dataParameter = Expression.Parameter(dataType, "data");
                    var cancelExpr = DynamicExpressionParser.ParseLambda(new[] { dataParameter }, typeof(bool), nextStep.CancelCondition);
                    targetStep.CancelCondition = cancelExpr;
                }

                targetStep.Id = i;
                targetStep.Name = nextStep.Name;
                targetStep.ErrorBehavior = nextStep.ErrorBehavior;
                targetStep.RetryInterval = nextStep.RetryInterval;
                targetStep.ExternalId = $"{nextStep.Id}";

                AttachInputs(nextStep, dataType, stepType, targetStep);
                AttachOutputs(nextStep, dataType, stepType, targetStep);

                if (nextStep.Do != null)
                {
                    foreach (var branch in nextStep.Do)
                    {
                        foreach (var child in branch.Reverse<Step>())
                            stack.Push(child);
                    }

                    if (nextStep.Do.Count > 0)
                        parents.Add(nextStep);
                }

                if (nextStep.CompensateWith != null)
                {
                    foreach (var compChild in nextStep.CompensateWith.Reverse<Step>())
                        stack.Push(compChild);

                    if (nextStep.CompensateWith.Count > 0)
                        compensatables.Add(nextStep);
                }

                if (!string.IsNullOrEmpty(nextStep.NextStepId))
                    targetStep.Outcomes.Add(new StepOutcome() { ExternalNextStepId = $"{nextStep.NextStepId}" });

                result.Add(targetStep);

                i++;
            }

            foreach (var step in result)
            {
                if (result.Any(x => x.ExternalId == step.ExternalId && x.Id != step.Id))
                    throw new WorkflowDefinitionLoadException($"Duplicate step Id {step.ExternalId}");

                foreach (var outcome in step.Outcomes)
                {
                    if (result.All(x => x.ExternalId != outcome.ExternalNextStepId))
                        throw new WorkflowDefinitionLoadException($"Cannot find step id {outcome.ExternalNextStepId}");

                    outcome.NextStep = result.Single(x => x.ExternalId == outcome.ExternalNextStepId).Id;
                }
            }

            foreach (var parent in parents)
            {
                var target = result.Single(x => x.ExternalId == parent.Id);
                foreach (var branch in parent.Do)
                {
                    var childTags = branch.Select(x => x.Id).ToList();
                    target.Children.AddRange(result
                        .Where(x => childTags.Contains(x.ExternalId))
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .Take(1)
                        .ToList());
                }
            }

            foreach (var item in compensatables)
            {
                var target = result.Single(x => x.ExternalId == item.Id);
                var tag = item.CompensateWith.Select(x => x.Id).FirstOrDefault();
                if (tag != null)
                {
                    var compStep = result.FirstOrDefault(x => x.ExternalId == tag);
                    if (compStep != null)
                        target.CompensationStepId = compStep.Id;
                }
            }

            return result;
        }

        private void AttachInputs(Step source, Type dataType, Type stepType, WorkflowStep step)
        {
            foreach (var input in source.Inputs)
            {
                var dataParameter = Expression.Parameter(dataType, "data");
                var contextParameter = Expression.Parameter(typeof(IStepExecutionContext), "context");
                var sourceExpr = DynamicExpressionParser.ParseLambda(new[] { dataParameter, contextParameter }, typeof(object), input.Value);

                var stepParameter = Expression.Parameter(stepType, "step");
                var targetProperty = Expression.Property(stepParameter, input.Key);
                var targetExpr = Expression.Lambda(targetProperty, stepParameter);

                step.Inputs.Add(new MemberMapParameter(sourceExpr, targetExpr));
            }
        }

        private void AttachOutputs(Step source, Type dataType, Type stepType, WorkflowStep step)
        {
            foreach (var output in source.Outputs)
            {
                var stepParameter = Expression.Parameter(stepType, "step");
                var sourceExpr = DynamicExpressionParser.ParseLambda(new[] { stepParameter }, typeof(object), output.Value);

                var dataParameter = Expression.Parameter(dataType, "data");
                Expression targetProperty;

                // Check if our datatype has a matching property
                var propertyInfo = dataType.GetProperty(output.Key);
                if (propertyInfo != null)
                {
                    targetProperty = Expression.Property(dataParameter, propertyInfo);
                    var targetExpr = Expression.Lambda(targetProperty, dataParameter);
                    step.Outputs.Add(new MemberMapParameter(sourceExpr, targetExpr));
                }
                else
                {
                    // If we did not find a matching property try to find a Indexer with string parameter
                    propertyInfo = dataType.GetProperty("Item");
                    targetProperty = Expression.Property(dataParameter, propertyInfo, Expression.Constant(output.Key));

                    Action<IStepBody, object> acn = (pStep, pData) =>
                    {
                        object resolvedValue = sourceExpr.Compile().DynamicInvoke(pStep); ;
                        propertyInfo.SetValue(pData, resolvedValue, new object[] { output.Key });
                    };

                    step.Outputs.Add(new ActionParameter<IStepBody, object>(acn));
                }
            }
        }

        private Type FindType(string name)
        {
            name = name.Trim();
            var result = Type.GetType($"WorkflowCore.Primitives.{name} , WorkflowCore", false, true);

            if (result != null)
                return result;

            return Type.GetType($"Conductor.Steps.{name}, Conductor.Steps", true, true);
        }

    }
}
