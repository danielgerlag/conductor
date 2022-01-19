using Conductor.Domain.Models;
using System.Collections.Generic;

namespace Conductor.Domain.Interfaces
{
    public interface IFlowService
    {
        void RegisterNewFlow(Flow flow);
        Flow GetFlow(string id);
        IEnumerable<Flow> GetFlows(int pageNumber, int pageSize);
    }
}
