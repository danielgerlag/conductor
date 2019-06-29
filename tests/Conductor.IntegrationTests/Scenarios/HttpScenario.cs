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
    public class HttpScenario : Scenario
    {

        public HttpScenario(Setup setup) : base(setup)
        {
        }

        [Fact]
        public async void should_get()
        {
            dynamic inputs = new ExpandoObject();
            inputs.BaseUrl = @"""http://demo7149346.mockable.io/""";
            inputs.Resource = @"""ping""";

            var definition = new Definition()
            {
                Id = Guid.NewGuid().ToString(),
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Id = "step1",
                        StepType = "HttpRequest",
                        Inputs = inputs,
                        Outputs = new Dictionary<string, string>()
                        {
                            ["ResponseCode"] = "step.ResponseCode",
                            ["ResponseBody"] = "step.ResponseBody"
                        }
                    }
                }
            };
            
            var registerRequest = new RestRequest(@"/definition", Method.POST);
            registerRequest.AddJsonBody(definition);
            var registerResponse = _client.Execute(registerRequest);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Thread.Sleep(1000);

            var startRequest = new RestRequest($"/workflow/{definition.Id}", Method.POST);
            startRequest.AddJsonBody(new { });
            var startResponse = _client.Execute<WorkflowInstance>(startRequest);
            startResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var instance = await WaitForComplete(startResponse.Data.WorkflowId);
            instance.Status.Should().Be("Complete");
            var data = JObject.FromObject(instance.Data);
            data["ResponseCode"].Value<int>().Should().Be(200);
        }
        
    }
}
