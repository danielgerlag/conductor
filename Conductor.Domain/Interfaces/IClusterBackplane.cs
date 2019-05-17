using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Domain.Interfaces
{
    public interface IClusterBackplane
    {
        void LoadNewDefinition(string id, int version);
    }
}
