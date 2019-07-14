using System;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Conductor.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IDefinitionService, DefinitionService>();
            services.AddSingleton<ILambdaService, LambdaService>();
            services.AddSingleton<IWorkflowLoader, WorkflowLoader>();
        }
    }
}
