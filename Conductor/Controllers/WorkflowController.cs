using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conductor.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public void Post(string id, [FromBody] string data)
        {
            _workflowController.StartWorkflow(id);
            Response.StatusCode = 204;
        }

     
    }
}