using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Conductor.IntegrationTests
{
    public class Setup : IDisposable
    {
        ICompositeService _svc;
        public Setup()
        {
            Environment.CurrentDirectory = @"../../../";
            _svc = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(@"docker-compose.yml")
                .RemoveOrphans()
                .WaitForHttp("conductor", @"http://localhost:5003/api/values")
                .Build().Start();
        }

        public void Dispose()
        {
            _svc.Stop();
            _svc.Dispose();
        }

    }
}
