using System;
using System.Collections.Generic;
using System.Text;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;

namespace Conductor.Domain.Services
{
    public class LambdaService : ILambdaService
    {

        private readonly IResourceRepository _resourceRepository;
        private readonly IScriptEngineHost _scriptHost;

        public LambdaService(IResourceRepository resourceRepository, IScriptEngineHost scriptHost)
        {
            _resourceRepository = resourceRepository;
            _scriptHost = scriptHost;
        }

        public void SaveLambdaResource(Resource resource)
        {
            if (resource.ContentType != @"text/x-python")
                throw new ArgumentException();

            _resourceRepository.Save(Bucket.Lambda, resource);
        }

        public Resource GetLambdaResource(string name)
        {
            return _resourceRepository.Find(Bucket.Lambda, name);
        }

        public void ExecuteLambda(string name, IDictionary<string, object> scope)
        {
            var resource = GetLambdaResource(name);
            _scriptHost.Execute(resource, scope);
        }
    }
}
