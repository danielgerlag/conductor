using Conductor.Auth;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult<IEnumerable<Definition>> Get([FromQuery] PaginationParameter parameter)
        {
            var result = parameter.IsValid() ?

                _service.GetDefinitions()
                    .OrderByDescending(x => x.Version)
                    .DistinctBy(p => p.Id)
                    .Skip((parameter.PageNumber - 1) * parameter.PageSize)
                    .Take(parameter.PageSize) :

                _service.GetDefinitions()
                    .OrderByDescending(x => x.Version)
                    .DistinctBy(p => p.Id);       
           
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