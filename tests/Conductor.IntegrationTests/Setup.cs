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
                //.ForceBuild()
                .WaitForHttp("conductor1", @"http://localhost:5101/api/info")
                .WaitForHttp("conductor2", @"http://localhost:5102/api/info")
                .Build().Start();
        }

        public void Dispose()
        {
            _svc?.Stop();
            _svc?.Dispose();
        }

        public string Server1 => "http://localhost:5101/api";
        public string Server2 => "http://localhost:5102/api";

    }
}
