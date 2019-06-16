using System;
using System.Threading;
using RestSharp;
using Xunit;

namespace Conductor.IntegrationTests
{
    [Collection("Conductor")]
    public class UnitTest1
    {

        public UnitTest1(Setup setup)
        {

        }

        [Fact]
        public void Test1()
        {
            var client = new RestClient(@"http://localhost:5003");
            var request = new RestRequest(@"/api/values", Method.GET);
            request.AddHeader("Accept", @"application/json");
            var response = client.Execute(request);

            Thread.Sleep(0);
        }
    }
}
