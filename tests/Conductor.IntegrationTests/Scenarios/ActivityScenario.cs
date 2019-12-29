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
    public class ActivityScenario : Scenario
    {

        public ActivityScenario(Setup setup) : base(setup)
        {
        }

        [Fact]
        public async void Scenario()
        {
            dynamic inputs = new ExpandoObject();
            inputs.ActivityName = "'act1'";
            inputs.Parameters = "data";

            var definition = new Definition()
            {
                Id = Guid.NewGuid().ToString(),
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Id = "step1",
                        StepType = "Activity",
                        Inputs = inputs,
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

            var startRequest = new RestRequest($"/workflow/{definition.Id}", Method.POST);
            startRequest.AddJsonBody(new { Value1 = 2, Value2 = 3 });
            var startResponse = _client.Execute<WorkflowInstance>(startRequest);
            startResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var activityRequest = new RestRequest($"/activity/act1?timeout=10", Method.GET);
            var activityResponse = _client.Execute<PendingActivity>(activityRequest);
            activityResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var activityInput = JObject.FromObject(activityResponse.Data.Parameters);
            var actResult = activityInput["Value1"].Value<int>() + activityInput["Value2"].Value<int>();

            var activitySuccessRequest = new RestRequest($"/activity/success/{activityResponse.Data.Token}", Method.POST);
            activitySuccessRequest.AddJsonBody(actResult);
            var activitySuccessResponse = _client.Execute(activitySuccessRequest);
            activitySuccessResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
            
            var instance = await WaitForComplete(startResponse.Data.WorkflowId);
            instance.Status.Should().Be("Complete");
            var data = JObject.FromObject(instance.Data);
            data["Result"].Value<int>().Should().Be(5);
        }
    }
}
