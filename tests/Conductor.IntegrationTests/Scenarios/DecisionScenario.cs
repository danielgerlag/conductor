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
    [Collection("Conductor")]
    public class DecisionScenario : Scenario
    {

        public DecisionScenario(Setup setup) : base(setup)
        {
        }

        [Fact]
        public async void Scenario()
        {
            dynamic add1inputs = new ExpandoObject();
            add1inputs.Value1 = "data.Value1";
            add1inputs.Value2 = "data.Value2";

            dynamic add2inputs = new ExpandoObject();
            add2inputs.Value1 = "data.Value1";
            add2inputs.Value2 = "data.Value3";

            var definition = new Definition()
            {
                Id = Guid.NewGuid().ToString(),
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Id = "Decide",
                        StepType = "Decide",
                        SelectNextStep = new Dictionary<string, string>()
                        {
                            ["A"] = "data.Flag == 1",
                            ["B"] = "data.Flag == 0"
                        }
                    },
                    new Step()
                    {
                        Id = "A",
                        StepType = "AddTest",
                        Inputs = add1inputs,
                        Outputs = new Dictionary<string, string>()
                        {
                            ["Result"] = "step.Result"
                        }
                    },
                    new Step()
                    {
                        Id = "B",
                        StepType = "AddTest",
                        Inputs = add2inputs,
                        Outputs = new Dictionary<string, string>()
                        {
                            ["Result"] = "step.Result"
                        }
                    }
                }
            };
            
            var registerRequest = new RestRequest(@"/definition", Method.POST);
            registerRequest.AddJsonBody(definition);
            var registerResponse = _client.Execute(registerRequest);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Thread.Sleep(1000);

            var startRequest1 = new RestRequest($"/workflow/{definition.Id}", Method.POST);
            startRequest1.AddJsonBody(new { Value1 = 2, Value2 = 3, Value3 = 4, Flag = 1 });
            var startResponse1 = _client.Execute<WorkflowInstance>(startRequest1);
            startResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

            var startRequest2 = new RestRequest($"/workflow/{definition.Id}", Method.POST);
            startRequest2.AddJsonBody(new { Value1 = 2, Value2 = 3, Value3 = 4, Flag = 0 });
            var startResponse2 = _client.Execute<WorkflowInstance>(startRequest2);
            startResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

            var instance1 = await WaitForComplete(startResponse1.Data.WorkflowId);
            instance1.Status.Should().Be("Complete");
            var data1 = JObject.FromObject(instance1.Data);
            data1["Result"].Value<int>().Should().Be(5);

            var instance2 = await WaitForComplete(startResponse2.Data.WorkflowId);
            instance2.Status.Should().Be("Complete");
            var data2 = JObject.FromObject(instance2.Data);
            data2["Result"].Value<int>().Should().Be(6);
        }
        
    }
}
