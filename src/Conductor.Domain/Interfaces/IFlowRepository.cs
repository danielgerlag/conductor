using Conductor.Domain.Models;
using System.Collections.Generic;

namespace Conductor.Domain.Interfaces
{
    public interface IFlowRepository
    {
        IEnumerable<Flow> GetAll();
        IEnumerable<Flow> Get(int pageNumber, int pageSize);
        Flow Find(string flowId);
        void Save(Flow flow);
    }
}
