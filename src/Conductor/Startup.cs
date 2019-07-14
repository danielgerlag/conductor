using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Conductor.Domain;
using Conductor.Domain.Interfaces;
using Conductor.Domain.Scripting;
using Conductor.Domain.Services;
using Conductor.Formatters;
using Conductor.Mappings;
using Conductor.Steps;
using Conductor.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;

namespace Conductor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //System.Reflection.Assembly.GetEntryAssembly().
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionStr = Environment.GetEnvironmentVariable("DBHOST");
            if (string.IsNullOrEmpty(dbConnectionStr))
                dbConnectionStr = Configuration.GetValue<string>("DbConnectionString");

            var redisConnectionStr = Environment.GetEnvironmentVariable("REDIS");
            if (string.IsNullOrEmpty(redisConnectionStr))
                redisConnectionStr = Configuration.GetValue<string>("RedisConnectionString");



            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new YamlRequestBodyInputFormatter());
                options.OutputFormatters.Add(new YamlRequestBodyOutputFormatter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddWorkflow(cfg =>
            {
                cfg.UseMongoDB(dbConnectionStr, Configuration.GetValue<string>("DbName"));
                if (!string.IsNullOrEmpty(redisConnectionStr))
                {
                    cfg.UseRedisLocking(redisConnectionStr);
                    cfg.UseRedisQueues(redisConnectionStr, "conductor");
                }
            });
            services.ConfigureDomainServices();
            services.ConfigureScripting();
            services.AddSteps();
            services.UseMongoDB(dbConnectionStr, Configuration.GetValue<string>("DbName"));

            if (string.IsNullOrEmpty(redisConnectionStr))
                services.AddSingleton<IClusterBackplane, LocalBackplane>();
            else
                services.AddSingleton<IClusterBackplane>(sp => new RedisBackplane(redisConnectionStr, "conductor", sp.GetService<IDefinitionRepository>(), sp.GetService<IWorkflowLoader>(), sp.GetService<ILoggerFactory>()));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<APIProfile>();
            });

            services.AddSingleton<IMapper>(x => new Mapper(config));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
            
            var host = app.ApplicationServices.GetService<IWorkflowHost>();
            var defService = app.ApplicationServices.GetService<IDefinitionService>();
            var backplane = app.ApplicationServices.GetService<IClusterBackplane>();
            defService.LoadDefinitionsFromStorage();
            backplane.Start();
            host.Start();
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                host.Stop();
                backplane.Stop();
            });
        }
    }
}
