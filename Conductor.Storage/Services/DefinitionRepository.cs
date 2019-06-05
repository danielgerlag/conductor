using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Storage.Models;
using MongoDB.Driver;

namespace Conductor.Storage.Services
{
    public class DefinitionRepository : IDefinitionRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<StoredDefinition> _collection => _database.GetCollection<StoredDefinition>("Definitions");

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
            var result = _collection.Find(x => x.Definition.Id == workflowId && x.Definition.Version == version);
            if (!result.Any())
                return null;
            return result.First().Definition;
        }

        public int? GetLatestVersion(string workflowId)
        {
            var versions = _collection.AsQueryable().Where(x => x.Definition.Id == workflowId);
            if (!versions.Any())
                return null;

            return versions.Max(x => x.Definition.Version);
        }

        public IEnumerable<Definition> GetAll()
        {
            return _collection.AsQueryable().Select(x => x.Definition).ToList();
        }

        public void Save(Definition definition)
        {
            if (_collection.AsQueryable().Any(x => x.Definition.Id == definition.Id && x.Definition.Version == definition.Version))
            {
                _collection.ReplaceOne(x => x.Definition.Id == definition.Id && x.Definition.Version == definition.Version, new StoredDefinition()
                {
                    Definition = definition
                });
                return;
            }

            _collection.InsertOne(new StoredDefinition()
            {
                Definition = definition
            });
        }

        static bool indexesCreated = false;
        static void CreateIndexes(IMongoCollection<StoredDefinition> collection)
        {
            if (!indexesCreated)
            {
                collection.Indexes.CreateOne(Builders<StoredDefinition>.IndexKeys.Ascending(x => x.Definition.Id).Ascending(x => x.Definition.Version), new CreateIndexOptions() { Background = true, Name = "unq_definition_id_version", Unique = true });
                collection.Indexes.CreateOne(Builders<StoredDefinition>.IndexKeys.Ascending(x => x.Definition.Id), new CreateIndexOptions() { Background = true, Name = "idx_definition_id" });
                indexesCreated = true;
            }
        }
    }
}
