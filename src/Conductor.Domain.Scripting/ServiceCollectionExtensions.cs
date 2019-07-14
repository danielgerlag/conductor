using System;
using Conductor.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Conductor.Domain.Scripting
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureScripting(this IServiceCollection services)
        {
            services.AddSingleton<IScriptEngineFactory, ScriptEngineFactory>();
            services.AddSingleton<IScriptEngineHost, ScriptEngineHost>();
        }
    }
}
