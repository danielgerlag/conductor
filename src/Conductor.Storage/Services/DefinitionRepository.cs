using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Storage.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Conductor.Storage.Services
{
    public class DefinitionRepository : IDefinitionRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<StoredDefinition> _collection => _database.GetCollection<StoredDefinition>("Definitions");

        static DefinitionRepository()
        {
            //BsonSerializer.RegisterSerializer(typeof(JObject), new DataObjectSerializer());
            //Dictionary<string, object>
        }

        public DefinitionRepository(IMongoDatabase database)
        {
            _database = database;
            CreateIndexes(_collection);
        }

        public Definition Find(string workflowId)
        {
            var version = GetLatestVersion(workflowId);
            if (version == null)
                return null;
            return Find(workflowId, version.Value);
        }

        public Definition Find(string workflowId, int version)
        {
            var result = _collection.Find(x => x.ExternalId == workflowId && x.Version == version);
            if (!result.Any())
                return null;
            
            var json = result.First().Definition.ToJson();
            return JsonConvert.DeserializeObject<Definition>(json);
        }

        public int? GetLatestVersion(string workflowId)
        {
            var versions = _collection.AsQueryable().Where(x => x.ExternalId == workflowId);
            if (!versions.Any())
                return null;

            return versions.Max(x => x.Version);
        }

        public IEnumerable<Definition> GetAll()
        {
            var results = _collection.AsQueryable().Select(x => x.Definition);

            foreach (var item in results)
            {
                var json = item.ToJson();
                yield return JsonConvert.DeserializeObject<Definition>(json);
            }
        }

        public void Save(Definition definition)
        {
            var json = JsonConvert.SerializeObject(definition);
            var doc = BsonDocument.Parse(json);

            if (_collection.AsQueryable().Any(x => x.ExternalId == definition.Id && x.Version == definition.Version))
            {
                _collection.ReplaceOne(x => x.ExternalId == definition.Id && x.Version == definition.Version, new StoredDefinition()
                {
                    ExternalId = definition.Id,
                    Version = definition.Version,
                    Definition = doc
                });
                return;
            }

            _collection.InsertOne(new StoredDefinition()
            {
                ExternalId = definition.Id,
                Version = definition.Version,
                Definition = doc
            });
        }

        static bool indexesCreated = false;
        static void CreateIndexes(IMongoCollection<StoredDefinition> collection)
        {
            if (!indexesCreated)
            {
                collection.Indexes.CreateOne(Builders<StoredDefinition>.IndexKeys.Ascending(x => x.ExternalId).Ascending(x => x.Version), new CreateIndexOptions() { Background = true, Name = "unq_definition_id_version", Unique = true });
                collection.Indexes.CreateOne(Builders<StoredDefinition>.IndexKeys.Ascending(x => x.ExternalId), new CreateIndexOptions() { Background = true, Name = "idx_definition_id" });
                indexesCreated = true;
            }
        }
    }
}
