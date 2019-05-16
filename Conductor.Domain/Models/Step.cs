using System;
using System.Collections.Generic;
using System.Text;
using WorkflowCore.Models;

namespace Conductor.Domain.Models
{
    public class Step
    {
        public string StepType { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string CancelCondition { get; set; }

        public WorkflowErrorHandling? ErrorBehavior { get; set; }

        public TimeSpan? RetryInterval { get; set; }

        public List<List<Step>> Do { get; set; } = new List<List<Step>>();

        public List<Step> CompensateWith { get; set; } = new List<Step>();

        public bool Saga { get; set; } = false;

        public string NextStepId { get; set; }

        public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Outputs { get; set; } = new Dictionary<string, string>();


    }
}
