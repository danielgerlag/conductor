using Conductor.Configuration.Settings;
using Conductor.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.ActionFilters
{
/// <summary>
    /// Returns http response code along with descriptive message to the client when authentication or authorization is failed
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private AuthenticationSettings _authenticationSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationSettings"></param>
        public AuthorizeCheckOperationFilter(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        /// <summary>
        /// Checks for non authorized requests and returns appropriate status codes
        /// </summary>
        /// <param name="operation">Current swagger request</param>
        /// <param name="context">Request context</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true).Union(context.MethodInfo.GetCustomAttributes(true)).OfType<AuthorizeAttribute>();
            var hasAuthorize = authAttributes.Any();

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            }
        }
    }
}

