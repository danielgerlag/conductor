using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Domain.Interfaces
{
    public interface IDefinitionService
    {
        void LoadDefinitionsFromStorage();
        void RegisterNewDefinition(string yaml);
    }
}
