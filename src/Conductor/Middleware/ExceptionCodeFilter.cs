using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkflowCore.Exceptions;

namespace Conductor.Middleware
{
    public class ExceptionCodeFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception == null)
                return;

            switch (context.Exception)
            {
                case NotFoundException _:
                    context.HttpContext.Response.StatusCode = 404;
                    context.ExceptionHandled = true;
                    break;
                case ArgumentException ae:
                    context.HttpContext.Response.StatusCode = 400;
                    context.Result = new BadRequestObjectResult(ae.Message);
                    context.ExceptionHandled = true;
                    break;
                case SharpYaml.YamlException ye:
                    context.HttpContext.Response.StatusCode = 400;
                    context.Result = new BadRequestObjectResult(ye.Message);
                    context.ExceptionHandled = true;
                    break;
            }            
        }
    }
}
