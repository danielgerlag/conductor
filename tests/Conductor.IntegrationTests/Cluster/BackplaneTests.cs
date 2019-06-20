using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using System.Threading;
using Conductor.Domain.Models;
using RestSharp;
using Xunit;

namespace Conductor.IntegrationTests.Cluster
{
    [Collection("Conductor")]
    public class BackplaneTests
    {

        public BackplaneTests(Setup setup)
        {

        }

        [Fact]
        public void should_notify_peers_of_new_definitions()
        {
            var definition = new Definition()
            {
                Id = Guid.NewGuid().ToString(),
                Steps = new List<Step>()
                {
                    new Step()
                    {
                        Id = "step1",
                        StepType = "EmitLog"
                    }
                }
            };
            var client1 = new RestClient(@"http://localhost:5101/api");
            var client2 = new RestClient(@"http://localhost:5102/api");
            var registerRequest = new RestRequest(@"/definition", Method.POST);
            registerRequest.AddJsonBody(definition);
            var registerResponse = client1.Execute(registerRequest);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Thread.Sleep(1000);

            var startRequest = new RestRequest($"/workflow/{definition.Id}", Method.POST);
            startRequest.AddJsonBody(new object());
            var startResponse = client2.Execute(startRequest);
            startResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
