using Conductor.ActionFilters;
using Conductor.Configuration.Settings;
using Conductor.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
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
                new Info
                {
                    Title = "Conductor Api",
                    Version = "v1.0",
                    Description = "Dotnet core workflow server",
                    TermsOfService = "TODO: Add Terms of service",
                    Contact = new Contact
                    {
                        Name = "Daniel Gerlag",
                        Email = "TODO: Add Contact email",
                        Url = "https://github.com/danielgerlag/conductor"
                    },
                    License = new License
                    {
                        Name = "MIT License",
                        Url = "https://opensource.org/licenses/MIT"
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

                c.AddSecurityDefinition(ApplicationConstants.OAuth2, new OAuth2Scheme
                {
                    Flow = IdentityModel.OidcConstants.GrantTypes.Implicit,
                    AuthorizationUrl = authorizationUri.ToString(),
                    Scopes = new Dictionary<string, string> { { authenticationSettings.Scope, ApplicationConstants.ApiDescription } }
                });
                #endregion

            });
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Configure(IApplicationBuilder app)
        {

            // This will redirect default url to Swagger url
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conductor Api v1.0");
                c.OAuthClientId(ApplicationConstants.SwaggerClientId);
                c.OAuthAppName("Conductor Api Swagger Ui");
            });
        }
    }
}
