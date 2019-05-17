using System;
using System.Collections.Generic;
using System.Text;
using Conductor.Domain.Models;
using WorkflowCore.Models;

namespace Conductor.Domain.Interfaces
{
    public interface IWorkflowLoader
    {
        void LoadDefinition(Definition source);
    }
}
