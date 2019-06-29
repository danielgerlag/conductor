using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conductor.Domain.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Conductor.Formatters
{
    public class YamlRequestBodyOutputFormatter : OutputFormatter
    {

        public YamlRequestBodyOutputFormatter()
        {
            SupportedMediaTypes.Add("application/x-yaml");
            SupportedMediaTypes.Add("application/yaml");
        }
        
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            var sets = new SerializerSettings();
            var serializer = new Serializer(sets);
            sets.EmitTags = false;
            var body = Encoding.UTF8.GetBytes(serializer.Serialize(context.Object));
            await response.Body.WriteAsync(body, 0, body.Length);
        }
    }
}
