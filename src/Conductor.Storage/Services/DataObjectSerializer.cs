using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Conductor.Storage.Services
{
    public class DataObjectSerializer : SerializerBase<object>
    {
        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Objects,
        };
        
        public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var result = BsonSerializer.Deserialize(context.Reader, typeof(object));
            return JObject.FromObject(result);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var str = JsonConvert.SerializeObject(value, SerializerSettings);
            var doc = BsonDocument.Parse(str);
            
            BsonSerializer.Serialize(context.Writer, doc);
        }

    }
}
