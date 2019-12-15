using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace Conductor.Storage.Services
{
    public class DataObjectSerializer : SerializerBase<object>
    {
        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None,
        };
        
        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var result = BsonSerializer.Deserialize<string>(context.Reader);

            var obj = JObject.Parse(result);
            //return (obj as IDictionary<string, object>);
            return obj;

            //return JObject.Parse(result).t
            //return JObject.FromObject(result);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var str = JsonConvert.SerializeObject(value, SerializerSettings);
            //var doc = BsonDocument.Parse(str);
            MongoDB.Bson.Serialization.
            
            BsonSerializer.Serialize(context.Writer, str);
        }

    }
}
