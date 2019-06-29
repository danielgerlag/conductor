using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Conductor.Domain.Interfaces;

namespace Conductor.Domain.Services
{
    public class LocalBackplane : IClusterBackplane
    {
        public Task Start() => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;

        public void LoadNewDefinition(string id, int version)
        {
        }
    }
}
