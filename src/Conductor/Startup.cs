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
                options.EnableEndpointRouting = false;
            })
            .AddNewtonsoftJson()
            .SetCompatibilityVersion(CompatibilityVersion.Latest);

            if (Configuration.GetValue<bool>("AuthEnabled"))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        var publicKey = Convert.FromBase64String(Configuration.GetValue<string>("IssuerKey"));
                        var e1 = ECDsa.Create();
                        e1.ImportParameters(new ECParameters()
                        {
                            Curve = ECCurve.NamedCurves.nistP256,
                            Q = new ECPoint()
                            {
                                X = publicKey.Take(32).ToArray(),
                                Y = publicKey.Skip(32).Take(32).ToArray()
                            }
                        });


                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new ECDsaSecurityKey(e1),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                        options.Events = new JwtBearerEvents()
                        {
                            OnChallenge = context =>
                            {

                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                context.Token = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJmaXJzdE5hbWUiOiJ0ZXN0IiwibGFzdE5hbWUiOiJ0ZXN0IiwibmJmIjoxNTc3MDQ4NDk3LCJleHAiOjE2NzE3NDI4OTcsImlhdCI6MTU3NzA0ODQ5N30.S2UtNp4MybQgOKz43_oC5aLeeN6DKL24UIKZ1_UPcHd9DB0j7gP6SEkutpmAXVb6YQvWcVl2LIo6BdkWXD4F_g";
                                return Task.CompletedTask;
                            },
                            OnAuthenticationFailed = context =>
                            {
                                return Task.CompletedTask;
                            },
                            
                        };
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;

                        options.Validate();
                    });
            }

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

            //app.UseHttpsRedirection();
            app.UseMvc();
            app.UseAuthentication();
            app.UseAuthorization();


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
