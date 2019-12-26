using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conductor.Auth;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StepController : ControllerBase
    {
        private readonly ICustomStepService _service;

        public StepController(ICustomStepService service)
        {
            _service = service;
        }


        [HttpGet("{name}")]
        [Authorize(Policy = Policies.Author)]
        public IActionResult Get(string name)
        {
            var resource = _service.GetStepResource(name);

            if (resource == null)
                return NotFound();


            Response.Headers["Content-Type"] = resource.ContentType;
            return new StepResourceResult(resource);
        }

        [HttpPost("{name}")]
        [Authorize(Policy = Policies.Author)]
        public async void Post(string name)
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var resource = new Resource()
                {
                    Name = name,
                    ContentType = Request.ContentType,
                    Content = await sr.ReadToEndAsync()
                };
                _service.SaveStepResource(resource);
                Response.StatusCode = 200;
            }   
        }

    }

    public class StepResourceResult : IActionResult
    {
        private readonly Resource _resource;

        public StepResourceResult(Resource resource)
        {
            _resource = resource;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers["Content-Type"] = _resource.ContentType;
            context.HttpContext.Response.StatusCode = 200;
            var body = Encoding.UTF8.GetBytes(_resource.Content);
            await context.HttpContext.Response.Body.WriteAsync(body, 0, body.Length);
        }
    }
}