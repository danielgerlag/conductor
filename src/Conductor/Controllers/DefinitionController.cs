using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Conductor.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
#if UseAuthentication
    [Authorize]
#endif
    public class DefinitionController : ControllerBase
    {
        private readonly IDefinitionService _service;

        public DefinitionController(IDefinitionService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<Definition> Get(string id)
        {
            var result = _service.GetDefinition(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
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
        public void Delete(int id)
        {
        }
    }
}