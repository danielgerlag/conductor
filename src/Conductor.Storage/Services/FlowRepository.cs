using Conductor.Domain.Interfaces;
using Conductor.Domain.Models;
using Conductor.Storage.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Conductor.Storage.Services
{
    public class FlowRepository : IFlowRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<StoredFlow> _collection => _database.GetCollection<StoredFlow>("Flows");
        private IMongoCollection<StoredDefinition> _definitionsCollection => _database.GetCollection<StoredDefinition>("Definitions");

        static FlowRepository()
        {
        }

        public FlowRepository(IMongoDatabase database)
        {
            _database = database;
            CreateIndexes(_collection);
        }

        public Flow Find(string flowId)
        {
            var result = _collection.Find(x => x.ExternalId == flowId);

            if (!result.Any())
                return null;

            var json = result.First().Flow.ToJson();
            var flow = JsonConvert.DeserializeObject<Flow>(json);
           
            var definitions = _definitionsCollection.AsQueryable()
                .Where(x => flow.DefinitionIds.Contains(x.ExternalId))
                .Select(x => x.Definition);

            foreach (var definitionBson in definitions)
            {
                var definitionJson = definitionBson.ToJson();
                var definition = JsonConvert.DeserializeObject<Definition>(definitionJson);
                flow.Definitions.Add(definition);
            }

            return flow;
        }

        public IEnumerable<Flow> GetAll()
        {
            var results = _collection.AsQueryable().Select(x => x.Flow);

            foreach (var item in results)
            {
                var json = item.ToJson();
                yield return JsonConvert.DeserializeObject<Flow>(json);
            }
        }

        public IEnumerable<Flow> Get(int pageNumber, int pageSize)
        {
            var paginationValid = pageNumber > 0 && pageSize > 0;

            var results = paginationValid ?

                _collection
                    .AsQueryable()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => x.Flow) :

                _collection
                    .AsQueryable()
                    .Select(x => x.Flow);

            foreach (var item in results)
            {
                var json = item.ToJson();
                yield return JsonConvert.DeserializeObject<Flow>(json);
            }
        }

        public void Save(Flow flow)
        {
            var json = JsonConvert.SerializeObject(flow);
            var doc = BsonDocument.Parse(json);

            if (_collection.AsQueryable().Any(x => x.ExternalId == flow.Id))
            {
                _collection.ReplaceOne(x => x.ExternalId == flow.Id, new StoredFlow()
                {
                    ExternalId = flow.Id,
                    Flow = doc
                });
                return;
            }

            _collection.InsertOne(new StoredFlow()
            {
                ExternalId = flow.Id,
                Flow = doc
            });
        }

        static bool indexesCreated = false;
        static void CreateIndexes(IMongoCollection<StoredFlow> collection)
        {
            if (!indexesCreated)
            {
                collection.Indexes.CreateOne(Builders<StoredFlow>.IndexKeys.Ascending(x => x.ExternalId), new CreateIndexOptions() { Background = true, Name = "idx_flow_id" });
                indexesCreated = true;
            }
        }
    }
}
