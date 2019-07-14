using System;
using System.Collections.Generic;
using System.Text;
using Conductor.Domain.Models;

namespace Conductor.Domain.Interfaces
{
    public interface IDefinitionService
    {
        void LoadDefinitionsFromStorage();
        void RegisterNewDefinition(Definition definition);
        void ReplaceVersion(Definition definition);
        Definition GetDefinition(string id);
    }
}
