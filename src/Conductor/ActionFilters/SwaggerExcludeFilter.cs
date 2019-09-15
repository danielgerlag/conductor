using Conductor.Helpers;
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
    /// Ignores all the properties for the request
    /// </summary>
    public class SwaggerExcludeFilter : ISchemaFilter
    {
        /// <inherit/>
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            var excludeProperties = context.Type?.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SwaggerExcludeAttribute)));
            if (excludeProperties != null)
            {
                foreach (var property in excludeProperties)
                {
                    // Because swagger uses camel casing
                    var propertyName = $"{char.ToLower(property.Name[0])}{property.Name.Substring(1)}";
                    if (model.Properties.ContainsKey(propertyName))
                    {
                        model.Properties.Remove(propertyName);
                    }
                }
            }
        }
    }
}
