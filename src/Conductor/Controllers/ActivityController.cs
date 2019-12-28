using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Conductor.Auth;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WorkflowCore.Interface;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityController _activityService;
        private readonly IMapper _mapper;

        public ActivityController(IActivityController activityService, IMapper mapper)
        {
            _activityService = activityService;
            _mapper = mapper;
        }

        [Authorize(Policy = Policies.Worker)]
        [HttpPost("success/{token}")]
        public async Task<IActionResult> Success(string token, [FromBody] object data)
        {
            if (data is JObject)
                data = (data as JObject).ToObject<ExpandoObject>();
            await _activityService.SubmitActivitySuccess(token, data);
            return Accepted();                
        }

        [Authorize(Policy = Policies.Worker)]
        [HttpPost("fail/{token}")]
        public async Task<IActionResult> Fail(string token, [FromBody] object data)
        {
            if (data is JObject)
                data = (data as JObject).ToObject<ExpandoObject>();
            await _activityService.SubmitActivityFailure(token, data);
            return Accepted();
        }

        [HttpGet("{name}")]
        [Authorize(Policy = Policies.Worker)]
        public async Task<IActionResult> Get(string name, string workerId, int timeout)
        {
            var result = await _activityService.GetPendingActivity(name, workerId, TimeSpan.FromSeconds(timeout));

            if (result == null)
                return NotFound();
            
            return Ok(_mapper.Map<Models.PendingActivity>(result));
        }

        [Authorize(Policy = Policies.Worker)]
        [HttpDelete("{token}")]
        public async Task<IActionResult> Delete(string token)
        {
            await _activityService.ReleaseActivityToken(token);
            return Accepted();
        }
    }
}