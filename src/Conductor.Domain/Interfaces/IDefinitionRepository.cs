using Conductor.Domain.Models;
using System.Collections.Generic;

namespace Conductor.Domain.Interfaces
{
    public interface IDefinitionRepository
    {
        IEnumerable<Definition> GetAll();
        IAsyncEnumerable<Definition> Get(int pageNumber, int pageSize);

        Definition Find(string workflowId);
        Definition Find(string workflowId, int version);
        int? GetLatestVersion(string workflowId);

        void Save(Definition definition);
    }
}
