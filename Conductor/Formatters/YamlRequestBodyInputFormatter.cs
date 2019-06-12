using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using SharpYaml.Serialization;

namespace Conductor.Formatters
{
    public class YamlRequestBodyInputFormatter : InputFormatter
    {
        public YamlRequestBodyInputFormatter()
        {
            SupportedMediaTypes.Add("application/x-yaml");
            SupportedMediaTypes.Add("application/yaml");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var request = context.HttpContext.Request;
            using (var reader = new StreamReader(request.Body))
            {
                try
                {
                    var content = await reader.ReadToEndAsync();
                    
                    var serializer = new Serializer();

                    if (context.ModelType == typeof(Definition))
                    {
                        var definition = serializer.DeserializeInto(content, new Definition());
                        return await InputFormatterResult.SuccessAsync(definition);
                    }
                    else
                        return await InputFormatterResult.FailureAsync();
                }
                catch (Exception ex)
                {
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
