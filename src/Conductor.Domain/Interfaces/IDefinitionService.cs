using Conductor.Domain.Models;
using System.Collections.Generic;

namespace Conductor.Domain.Interfaces
{
    public interface IDefinitionService
    {
        void LoadDefinitionsFromStorage();
        void RegisterNewDefinition(Definition definition);
        void ReplaceVersion(Definition definition);
        Definition GetDefinition(string id);
        IAsyncEnumerable<Definition> GetDefinitions(int pageNumber, int pageSize);
    }
}
