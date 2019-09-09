using Conductor.ActionFilters;
using Conductor.Configuration.Settings;
using Conductor.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Configuration
{

    /// <summary>
    /// Swagger API documentation configuration
    /// </summary>
    public class SwaggerConfiguration
    {
        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureService(IServiceCollection services)
        {
            // Get configuration instance
            var authenticationSettings = services.BuildServiceProvider().GetRequiredService<AuthenticationSettings>();

            // Swagger API documentation
            services.AddSwaggerGen(c =>
            {
                // TODO: Need to push hardcoded strings to resource file
                c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Conductor Api",
                    Version = "v1.0",
                    Description = "Dotnet core workflow server",
                    Contact = new OpenApiContact
                    {
                        Name = "Daniel Gerlag",
                        Email = "TODO: Add Contact email",
                        Url = new Uri("https://github.com/danielgerlag/conductor")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")

                    }
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>(authenticationSettings);

                c.SchemaFilter<SwaggerExcludeFilter>();

                var xmlFile = $"{typeof(Startup).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                #region Identity Server Authentication

                var authenticationServerUri = GenericHelper.GetUriFromEnvironmentVariable(ApplicationConstants.AuthenticationAuthority);
                var authorizationUri = GenericHelper.CombineUri(authenticationServerUri, authenticationSettings.AuthorizationUrl);

                c.AddSecurityDefinition(ApplicationConstants.OAuth2, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Request a token using your UserName and Password",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authorizationUri.ToString()),
                            Scopes = new Dictionary<string, string> { { authenticationSettings.Scope, ApplicationConstants.ApiDescription } }
                        }
                    }
                });
                #endregion

            });
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                    c.OAuthClientId(ApplicationConstants.SwaggerClientId);
                c.OAuthAppName("Conductor Api Swagger Ui");
                c.RoutePrefix = "";
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.EnableDeepLinking();
                c.DisplayOperationId();

                });
        }
    }
}
