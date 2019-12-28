using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Linq;
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
                var content = await reader.ReadToEndAsync();
                    
                var serializer = new Serializer();

                if (context.ModelType == typeof(Definition))
                {
                    var data = serializer.DeserializeInto(content, new Definition());
                    return await InputFormatterResult.SuccessAsync(data);
                }

                if (context.ModelType == typeof(JObject))
                {
                    var data = serializer.DeserializeInto(content, new JObject());
                    return await InputFormatterResult.SuccessAsync(data);
                }

                if (context.ModelType == typeof(ExpandoObject))
                {
                    var data = serializer.DeserializeInto(content, new ExpandoObject());
                    return await InputFormatterResult.SuccessAsync(data);
                }
                    
                if (context.ModelType == typeof(object))
                {
                    var data = serializer.Deserialize(content);
                    return await InputFormatterResult.SuccessAsync(JObject.FromObject(data));
                }

                return await InputFormatterResult.FailureAsync();                
            }
        }
    }
}
