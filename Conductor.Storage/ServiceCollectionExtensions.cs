using System;
using Conductor.Domain.Interfaces;
using Conductor.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WorkflowCore.Models;

namespace Conductor.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static void UseMongoDB(this IServiceCollection services, string mongoUrl, string databaseName)
        {
            var client = new MongoClient(mongoUrl);
            var db = client.GetDatabase(databaseName);
            services.AddTransient<IDefinitionRepository, DefinitionRepository>(x => new DefinitionRepository(db));
        }

        public static WorkflowOptions UseMongoDB(this WorkflowOptions options, string mongoUrl, string databaseName)
        {
            options.UsePersistence(sp =>
            {
                var client = new MongoClient(mongoUrl);
                var db = client.GetDatabase(databaseName);
                return new WorkflowPersistenceProvider(db);
            });
            return options;
        }
    }
}
