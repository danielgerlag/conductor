using System.Collections.Generic;
using Conductor.Domain.Models;

namespace Conductor.Domain.Interfaces
{
    public interface ICustomStepService
    {
        void SaveStepResource(Resource resource);
        Resource GetStepResource(string name);
        //void ExecuteLambda(string name, IDictionary<string, object> scope);
    }
}