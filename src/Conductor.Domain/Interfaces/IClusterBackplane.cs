using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Domain.Interfaces
{
    public interface IClusterBackplane
    {
        Task Start();
        Task Stop();
        void LoadNewDefinition(string id, int version);
    }
}
