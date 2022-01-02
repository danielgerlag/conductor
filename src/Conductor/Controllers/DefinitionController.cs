using Conductor.Auth;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefinitionController : ControllerBase
    {
        private readonly IDefinitionService _service;

        public DefinitionController(IDefinitionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Policy = Policies.Author)]
        public ActionResult<IAsyncEnumerable<Definition>> Get([FromQuery] PaginationParameter parameter)
        {
            var result = _service.GetDefinitions(parameter.PageNumber, parameter.PageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.Author)]
        public ActionResult<Definition> Get(string id)
        {
            var result = _service.GetDefinition(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.Author)]
        public void Post([FromBody] Definition value)
        {
            _service.RegisterNewDefinition(value);
            Response.StatusCode = 204;
        }
                
        //[HttpPut]
        //public void Put([FromBody] string value)
        //{
        //    _service.RegisterNewDefinition(value);
        //}

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Author)]
        public void Delete(int id)
        {
        }
    }
}