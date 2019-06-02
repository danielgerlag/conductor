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
            services.AddScoped<IDefinitionService, DefinitionService>();
            services.AddScoped<IWorkflowLoader, WorkflowLoader>();
            services.AddScoped<IClusterBackplane, LocalBackplane>();
        }
    }
}
