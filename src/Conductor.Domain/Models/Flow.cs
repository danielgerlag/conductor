using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Conductor.Domain.Models
{
    public class Flow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StartupDefinitionId { get; set; }
        public IEnumerable<string> DefinitionIds { get; set; }

        [BsonIgnore]
        public IList<Definition> Definitions { get; set; } = new List<Definition>();
    }
}
