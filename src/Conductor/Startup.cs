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
//using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Conductor.Auth;
using Conductor.Middleware;

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
            var dbConnectionStr = EnvironmentVariables.DbHost;
            if (string.IsNullOrEmpty(dbConnectionStr))
                dbConnectionStr = Configuration.GetValue<string>("DbConnectionString");

            var redisConnectionStr = EnvironmentVariables.Redis;
            if (string.IsNullOrEmpty(redisConnectionStr))
                redisConnectionStr = Configuration.GetValue<string>("RedisConnectionString");

            var authEnabled = false;
            var authEnabledStr = EnvironmentVariables.Auth;
            if (string.IsNullOrEmpty(authEnabledStr))
                authEnabled = Configuration.GetSection("Auth").GetValue<bool>("Enabled");
            else
                authEnabled = Convert.ToBoolean(authEnabledStr);
            

            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new YamlRequestBodyInputFormatter());                
                options.OutputFormatters.Add(new YamlRequestBodyOutputFormatter());
                options.Filters.Add<RequestObjectFilter>();
                options.Filters.Add<ExceptionCodeFilter>();
                options.EnableEndpointRouting = false;                
            })
            .AddNewtonsoftJson()
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            
            var authConfig = services.AddAuthentication(options =>
            {                
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            if (authEnabled)
                authConfig.AddJwtAuth(Configuration);
            else
                authConfig.AddBypassAuth();

            services.AddPolicies();

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
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
                        
            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc(cfg =>
            {
              //  cfg.
            });
            app.UseRouting();
            //app.UseMvcWithDefaultRoute();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

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
