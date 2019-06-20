using System;
using System.Collections.Generic;
using System.Text;
using Conductor.Domain.Models;

namespace Conductor.Domain.Interfaces
{
    public interface IScriptEngineHost
    {
        void Execute(Resource resource, IDictionary<string, object> inputs);
        dynamic EvaluateExpression(string expression, IDictionary<string, object> inputs);
    }
}
