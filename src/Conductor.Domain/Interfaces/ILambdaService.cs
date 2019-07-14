using System.Collections.Generic;
using Conductor.Domain.Models;

namespace Conductor.Domain.Interfaces
{
    public interface ILambdaService
    {
        void SaveLambdaResource(Resource resource);
        Resource GetLambdaResource(string name);
        void ExecuteLambda(string name, IDictionary<string, object> scope);
    }
}