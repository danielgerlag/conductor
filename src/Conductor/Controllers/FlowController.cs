using Conductor.Auth;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowController : ControllerBase
    {
        private readonly IFlowService _service;

        public FlowController(IFlowService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = Policies.Author)]
        public ActionResult<IEnumerable<Flow>> Get([FromQuery] PaginationParameter parameter)
        {
            var result = _service.GetFlows(parameter.PageNumber, parameter.PageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Author)]
        public ActionResult<Flow> Get(string id)
        {
            var result = _service.GetFlow(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.Author)]
        public void Post([FromBody] Flow value)
        {
            _service.RegisterNewFlow(value);
            Response.StatusCode = 204;
        }

        //[HttpPut]
        //public void Put([FromBody] string value)
        //{
        //    _service.RegisterNewFlow(value);
        //}

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Author)]
        public void Delete(int id)
        {
        }
    }
}