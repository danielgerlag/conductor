using System;
using System.Collections.Generic;
using System.Text;
using Conductor.Domain.Interfaces;

namespace Conductor.Domain.Services
{
    public class LocalBackplane : IClusterBackplane
    {
        public void LoadNewDefinition(string id, int version)
        {
        }
    }
}
