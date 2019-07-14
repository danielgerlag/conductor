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
using Newtonsoft.Json.Linq;

namespace Conductor.Storage.Services
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<StoredResource> _collection => _database.GetCollection<StoredResource>("Resources");

        static ResourceRepository()
        {
        }

        public ResourceRepository(IMongoDatabase database)
        {
            _database = database;
            CreateIndexes(_collection);
        }

        public Resource Find(Bucket bucket, string name)
        {
            var version = GetLatestVersion(bucket, name);
            if (version == null)
                return null;
            return Find(bucket, name, version.Value);
        }

        public Resource Find(Bucket bucket, string name, int version)
        {
            var result = _collection.Find(x => x.Bucket == bucket && x.Name == name && x.Version == version);
            if (!result.Any())
                return null;

            var json = result.First().Resource.ToJson();
            return JsonConvert.DeserializeObject<Resource>(json);
        }

        public int? GetLatestVersion(Bucket bucket, string name)
        {
            var versions = _collection.AsQueryable().Where(x => x.Bucket == bucket && x.Name == name);
            if (!versions.Any())
                return null;

            return versions.Max(x => x.Version);
        }
        
        public void Save(Bucket bucket, Resource resource)
        {
            var json = JsonConvert.SerializeObject(resource);
            var doc = BsonDocument.Parse(json);

            var version = GetLatestVersion(bucket, resource.Name) ?? 1;
            
            _collection.InsertOne(new StoredResource()
            {
                Name = resource.Name,
                Version = version,
                Bucket = bucket,
                Resource = doc
            });
        }

        static bool indexesCreated = false;
        static void CreateIndexes(IMongoCollection<StoredResource> collection)
        {
            if (!indexesCreated)
            {
                collection.Indexes.CreateOne(Builders<StoredResource>.IndexKeys.Ascending(x => x.Bucket).Ascending(x => x.Name).Ascending(x => x.Version), new CreateIndexOptions() { Background = true, Name = "unq_resource_id_version", Unique = true });
                collection.Indexes.CreateOne(Builders<StoredResource>.IndexKeys.Ascending(x => x.Name), new CreateIndexOptions() { Background = true, Name = "idx_resource_id" });
                indexesCreated = true;
            }
        }
    }
}
