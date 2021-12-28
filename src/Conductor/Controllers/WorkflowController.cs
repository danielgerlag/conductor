using AutoMapper;
using Conductor.Auth;
using Conductor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowController _workflowController;
        private readonly IPersistenceProvider _persistenceProvider;
        private readonly IMapper _mapper;

        public WorkflowController(IWorkflowController workflowController, IPersistenceProvider persistenceProvider, IMapper mapper)
        {
            _workflowController = workflowController;
            _persistenceProvider = persistenceProvider;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Viewer)]
        public async Task<ActionResult<WorkflowInstance>> Get(string id)
        {
            var result = await _persistenceProvider.GetWorkflowInstance(id);
            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<WorkflowInstance>(result));
        }

        [HttpPost("{id}")]
        [Authorize(Policy = Policies.Controller)]
        public async Task<ActionResult<WorkflowInstance>> Post(string id, [FromBody] ExpandoObject data)
        {
            var instanceId = await _workflowController.StartWorkflow(id, data);
            var result = await _persistenceProvider.GetWorkflowInstance(instanceId);

            return Created(instanceId, _mapper.Map<WorkflowInstance>(result));
        }

        [HttpPut("{id}/suspend")]
        [Authorize(Policy = Policies.Controller)]
        public async Task Suspend(string id)
        {
            var result = await _workflowController.SuspendWorkflow(id);
            if (result)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 400;
        }

        [HttpPut("{id}/resume")]
        [Authorize(Policy = Policies.Controller)]
        public async Task Resume(string id)
        {
            var result = await _workflowController.ResumeWorkflow(id);
            if (result)
                Response.StatusCode = 200;
            else
                Response.StatusCode = 400;
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Controller)]
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