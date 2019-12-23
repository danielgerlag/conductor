using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Conductor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conductor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]        
        public ActionResult<DiagnosticInfo> Get()
        {
            var process = Process.GetCurrentProcess();
            var entryAsm = Assembly.GetEntryAssembly();
            var version = FileVersionInfo.GetVersionInfo(entryAsm.Location);
            return new DiagnosticInfo()
            {
                MachineName = Environment.MachineName,
                StartTime = process.StartTime,
                WorkingSet = process.WorkingSet64,
                Version = version.ProductVersion,
                OSVersion = Environment.OSVersion.VersionString
            };
        }
    }
}
