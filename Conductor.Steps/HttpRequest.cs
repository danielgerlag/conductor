using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Conductor.Steps
{
    public class HttpRequest : StepBodyAsync
    {

        public string BaseUrl { get; set; }
        public string Resource { get; set; }

        public JObject Headers { get; set; }
        public JObject Parameters { get; set; }
        public JObject Body { get; set; }

        public DataFormat Format { get; set; } = DataFormat.Json;
        public Method Method { get; set; } = Method.GET;


        public string ErrorMessage { get; set; }
        public bool IsSuccessful { get; set; }
        public int ResponseCode { get; set; }
        public JObject ResponseBody { get; set; }

        public async override Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Resource, Method, Format);

            if (Headers != null)
            {
                foreach (var header in Headers.Properties())
                    request.AddHeader(header.Name, header.Value.Value<string>());
            }

            if (Parameters != null)
            {
                foreach (var param in Parameters.Properties())
                    request.AddQueryParameter(param.Name, param.Value.Value<string>());
            }

            if (Body != null)
            {
                switch (Format)
                {
                    case DataFormat.Json:
                        request.AddJsonBody(Body);
                        break;
                    case DataFormat.Xml:
                        request.AddXmlBody(Body);
                        break;

                }
            }
            
            var response = await client.ExecuteTaskAsync<JObject>(request);
            IsSuccessful = response.IsSuccessful;

            if (response.IsSuccessful)
            {
                ResponseCode = (int) response.StatusCode;
                ResponseBody = response.Data;
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }

            return ExecutionResult.Next();

        }
    }
}
