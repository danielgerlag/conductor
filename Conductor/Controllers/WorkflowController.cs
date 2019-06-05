using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WorkflowCore.Interface;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowController _workflowController;

        public WorkflowController(IWorkflowController workflowController)
        {
            _workflowController = workflowController;
        }

        [HttpPost("{id}")]
        public void Post(string id, [FromBody] JObject data)
        {
            _workflowController.StartWorkflow(id, data);
            Response.StatusCode = 204;
        }

        [HttpPut("{id}/suspend")]
        public async Task Suspend(string id)
        {
            var result = await _workflowController.SuspendWorkflow(id);
            if (result)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 400;
        }

        [HttpPut("{id}/resume")]
        public async Task Resume(string id)
        {
            var result = await _workflowController.ResumeWorkflow(id);
            if (result)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 400;
        }

        [HttpDelete("{id}")]
        public async Task Terminate(string id)
        {
            var result = await _workflowController.TerminateWorkflow(id);
            if (result)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 400;
        }


    }
}