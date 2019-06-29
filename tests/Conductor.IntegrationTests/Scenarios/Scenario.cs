using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Conductor.Domain.Models;
using Conductor.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xunit;

namespace Conductor.IntegrationTests.Scenarios
{
    public abstract class Scenario
    {
        protected readonly RestClient _client;

        public Scenario(Setup setup)
        {
            _client = new RestClient(setup.Server1);
        }
        
        protected async Task<WorkflowInstance> WaitForComplete(string workflowId)
        {
            var pollRequest = new RestRequest($"/workflow/{workflowId}", Method.GET);
            var pollResponse = _client.Execute<WorkflowInstance>(pollRequest);

            var count = 0;
            while ((pollResponse.Data.Status != "Complete") && (count < 60))
            {
                await Task.Delay(500);
                count++;
                pollResponse = _client.Execute<WorkflowInstance>(pollRequest);
            }

            return pollResponse.Data;
        }
    }
}
