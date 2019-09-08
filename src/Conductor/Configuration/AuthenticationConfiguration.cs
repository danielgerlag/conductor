using Conductor.Configuration.Settings;
using Conductor.Helpers;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.Configuration
{
    /// <summary>
    /// Oauth2 authentication settings
    /// </summary>
    public static class AuthenticationConfiguration
    {
        /// <summary>
        /// configures oauth2 authentication based on the configuration
        /// </summary>
        /// <param name="services">services</param>
        public static void Configure(IServiceCollection services)
        {
            // Get the configuration Settings
            // TODO: are there any alternative ways exist to get the configuration settings??
            var authenticationSettings = services.BuildServiceProvider().GetRequiredService<AuthenticationSettings>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = GenericHelper.GetUriFromEnvironmentVariable(ApplicationConstants.AuthenticationAuthority).ToString();
                    options.ApiName = authenticationSettings.Scope;
                    options.RequireHttpsMetadata = false;
                });
        }

    }
}
