using System;
using Microsoft.Extensions.DependencyInjection;

namespace Conductor.Steps
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSteps(this IServiceCollection services)
        {
            services.AddTransient<HttpRequest>();
            services.AddTransient<EmitLog>();
        }
    }
}
