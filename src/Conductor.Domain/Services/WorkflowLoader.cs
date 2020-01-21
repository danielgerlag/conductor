using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly IScriptEngineHost _scriptHost;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly ICustomStepService _stepService;

        public WorkflowLoader(IWorkflowRegistry registry, IScriptEngineHost scriptHost, IExpressionEvaluator expressionEvaluator, ICustomStepService stepService)
        {
            _registry = registry;
            _scriptHost = scriptHost;
            _expressionEvaluator = expressionEvaluator;
            _stepService = stepService;
        }

        public void LoadDefinition(Definition source)
        {
            var def = Convert(source);
            _registry.RegisterWorkflow(def);
        }

        private WorkflowDefinition Convert(Definition source)
        {
            var dataType = typeof(ExpandoObject);
            
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
                if (stepType == typeof(CustomStep))
                {
                    targetStep.Inputs.Add(new ActionParameter<CustomStep, object>((pStep, pData) => pStep["__custom_step__"] = nextStep.StepType));
                }

                if (nextStep.Saga)
                {
                    containerType = typeof(SagaContainer<>).MakeGenericType(stepType);
                    targetStep = (containerType.GetConstructor(new Type[] { }).Invoke(null) as WorkflowStep);
                }

                if (!string.IsNullOrEmpty(nextStep.CancelCondition))
                {
                    Func<ExpandoObject, bool> cancelFunc = (data) => _scriptHost.EvaluateExpression<bool>(nextStep.CancelCondition, new Dictionary<string, object>()
                    {
                        ["data"] = data,
                        ["environment"] = Environment.GetEnvironmentVariables()
                    });

                    Expression<Func<ExpandoObject, bool>> cancelExpr = (data) => cancelFunc(data);
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

                AttachOutcomes(nextStep, dataType, targetStep);

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
                var stepProperty = stepType.GetProperty(input.Key);

                if ((input.Value is IDictionary<string, object>) || (input.Value is IDictionary<object, object>))
                {
                    var acn = BuildObjectInputAction(input, stepProperty);
                    step.Inputs.Add(new ActionParameter<IStepBody, object>(acn));
                    continue;
                }
                else
                {
                    var acn = BuildScalarInputAction(input, stepProperty);
                    step.Inputs.Add(new ActionParameter<IStepBody, object>(acn));
                    continue;
                }

                throw new ArgumentException($"Unknown type for input {input.Key} on {source.Id}");
            }
        }
        
        private void AttachOutputs(Step source, Type dataType, Type stepType, WorkflowStep step)
        {
            foreach (var output in source.Outputs)
            {
                Action<IStepBody, object> acn = (pStep, pData) =>
                {
                    object resolvedValue = _scriptHost.EvaluateExpression(output.Value, new Dictionary<string, object>()
                    {
                        ["step"] = pStep,
                        ["data"] = pData
                    });
                    (pData as IDictionary<string, object>)[output.Key] = resolvedValue;
                };

                step.Outputs.Add(new ActionParameter<IStepBody, object>(acn));
            }
        }
        
        private void AttachOutcomes(Step source, Type dataType, WorkflowStep step)
        {            
            if (!string.IsNullOrEmpty(source.NextStepId))
                step.Outcomes.Add(new ValueOutcome() { ExternalNextStepId = $"{source.NextStepId}" });
            
            foreach (var nextStep in source.SelectNextStep)
            {
                Expression<Func<ExpandoObject, object, bool>> sourceExpr = (data, outcome) => _expressionEvaluator.EvaluateOutcomeExpression(nextStep.Value, data, outcome);
                step.Outcomes.Add(new ExpressionOutcome<ExpandoObject>(sourceExpr)
                {
                    ExternalNextStepId = $"{nextStep.Key}"
                });
            }
        }
              
        private Type FindType(string name)
        {
            name = name.Trim();
            var result = Type.GetType($"WorkflowCore.Primitives.{name}, WorkflowCore", false, true);

            if (result != null)
                return result;

            result = Type.GetType($"Conductor.Steps.{name}, Conductor.Steps", false, true);

            if (result != null)
                return result;

            if (_stepService.GetStepResource(name) != null)
                return typeof(CustomStep);

            throw new ArgumentException($"Step type {name} not found");
        }

        private Action<IStepBody, object, IStepExecutionContext> BuildScalarInputAction(KeyValuePair<string, object> input, PropertyInfo stepProperty)
        {
            var sourceExpr = System.Convert.ToString(input.Value);
            
            void acn(IStepBody pStep, object pData, IStepExecutionContext pContext)
            {
                var resolvedValue = _expressionEvaluator.EvaluateExpression(sourceExpr, pData, pContext);

                if (pStep is CustomStep)
                {
                    (pStep as CustomStep)[input.Key] = resolvedValue;
                    return;
                }

                if (stepProperty.PropertyType.IsEnum)
                    stepProperty.SetValue(pStep, Enum.Parse(stepProperty.PropertyType, (string)resolvedValue, true));
                else
                {
                    if (stepProperty.PropertyType == typeof(object))
                    {
                        stepProperty.SetValue(pStep, resolvedValue);
                    }
                    else
                    {
                        if ((resolvedValue != null) && (stepProperty.PropertyType.IsAssignableFrom(resolvedValue.GetType())))
                            stepProperty.SetValue(pStep, resolvedValue);
                        else
                            stepProperty.SetValue(pStep, System.Convert.ChangeType(resolvedValue, stepProperty.PropertyType));
                    }                    
                }
            }
            return acn;
        }

        

        private Action<IStepBody, object, IStepExecutionContext> BuildObjectInputAction(KeyValuePair<string, object> input, PropertyInfo stepProperty)
        {
            void acn(IStepBody pStep, object pData, IStepExecutionContext pContext)
            {
                var stack = new Stack<JObject>();
                var destObj = JObject.FromObject(input.Value);
                stack.Push(destObj);

                while (stack.Count > 0)
                {
                    var subobj = stack.Pop();
                    foreach (var prop in subobj.Properties().ToList())
                    {
                        if (prop.Name.StartsWith("@"))
                        {
                            var sourceExpr = prop.Value.ToString();
                            var resolvedValue = _expressionEvaluator.EvaluateExpression(sourceExpr, pData, pContext);;
                            subobj.Remove(prop.Name);
                            subobj.Add(prop.Name.TrimStart('@'), JToken.FromObject(resolvedValue));
                        }
                    }

                    foreach (var child in subobj.Children<JObject>())
                        stack.Push(child);
                }

                stepProperty.SetValue(pStep, destObj.ToObject<ExpandoObject>());
            }
            return acn;
        }
        
    }
}
