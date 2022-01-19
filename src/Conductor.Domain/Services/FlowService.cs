using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Conductor.Domain.Services
{
    public class FlowService : IFlowService
    {
        private readonly IFlowRepository _repository;
        private readonly ILogger _logger;

        public FlowService(IFlowRepository repository, ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public void RegisterNewFlow(Flow flow)
        {
            _repository.Save(flow);
        }

        public void ReplaceVersion(Definition definition)
        {
            throw new NotImplementedException();
        }

        public Flow GetFlow(string id)
        {
            return _repository.Find(id);
        }

        public IEnumerable<Flow> GetFlows(int pageNumber, int pageSize)
        {
            return _repository.Get(pageNumber, pageSize);
        }
    }
}
