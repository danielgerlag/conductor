using MongoDB.Bson;

namespace Conductor.Storage.Models
{
    public class StoredFlow
    {
        public ObjectId Id { get; set; }
        public string ExternalId { get; set; }
        public BsonDocument Flow { get; set; }
    }
}
