using System;
using System.Collections.Generic;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.Scripting;

namespace Conductor.Domain.Scripting
{
    public class ScriptEngineHost : IScriptEngineHost
    {
        private readonly IScriptEngineFactory _engineFactory;

        public ScriptEngineHost(IScriptEngineFactory engineFactory)
        {
            _engineFactory = engineFactory;
        }

        public void Execute(Resource resource, IDictionary<string, object> inputs)
        {
            var engine = _engineFactory.GetEngine(resource.ContentType);

            var source = engine.CreateScriptSourceFromString(resource.Content, SourceCodeKind.Statements);
            var scope = engine.CreateScope(inputs);
            source.Execute(scope);
            SanitizeScope(inputs);
        }

        public dynamic EvaluateExpression(string expression, IDictionary<string, object> inputs)
        {
            var engine = _engineFactory.GetExpressionEngine();
            var source = engine.CreateScriptSourceFromString(expression, SourceCodeKind.Expression);
            var scope = engine.CreateScope(inputs);
            return source.Execute(scope);
        }

        public T EvaluateExpression<T>(string expression, IDictionary<string, object> inputs)
        {
            return EvaluateExpression(expression, inputs);
        }

        private void SanitizeScope(IDictionary<string, object> scope)
        {
            scope.Remove("__builtins__");
        }

    }
}
