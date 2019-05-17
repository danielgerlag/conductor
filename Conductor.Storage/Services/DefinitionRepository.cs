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

        protected DefinitionRepository(IMongoDatabase database)
        {
            _database = database;
            CreateIndexes(_collection);
        }

        public Definition Find(string workflowId)
        {
            throw new NotImplementedException();
        }

        public Definition Find(string workflowId, int version)
        {
            throw new NotImplementedException();
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
