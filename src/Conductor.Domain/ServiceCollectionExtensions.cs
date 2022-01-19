﻿using System;
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
            services.AddSingleton<IFlowService, FlowService>();
            services.AddSingleton<IWorkflowLoader, WorkflowLoader>();
            services.AddSingleton<ICustomStepService, CustomStepService>();
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();
            services.AddTransient<CustomStep>();
        }
    }
}
