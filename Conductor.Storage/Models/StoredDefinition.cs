using Conductor.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Storage.Models
{
    public class StoredDefinition
    {
        public ObjectId Id { get; set; }

        public string ExternalId { get; set; }

        public int Version { get; set; }

        public BsonDocument Definition { get; set; }
    }
}
