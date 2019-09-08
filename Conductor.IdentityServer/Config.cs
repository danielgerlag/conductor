// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using Conductor.IdentityServer.Helpers;

namespace Conductor.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var x = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
            return x;
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApplicationConstants.ConductorApi, "Conductor API", new [] { JwtClaimTypes.Name, JwtClaimTypes.WebSite, JwtClaimTypes.Role})
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // OpenID Connect implicit flow Swagger
                new Client
                {
                    ClientId = ApplicationConstants.ConductorSwaggerClient,
                    ClientName = "Conductor Api Swagger Ui",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    // where to redirect to after login
                    RedirectUris = {
                        GenericHelper.GetUriFromEnvironmentAndCombine(ApplicationConstants.SwaggerClient, "/swagger/oauth2-redirect.html").ToString(),
                        GenericHelper.GetUriFromEnvironmentAndCombine(ApplicationConstants.SwaggerClient, "/swagger/o2c.html").ToString(),

                    },

                    AllowedScopes = { ApplicationConstants.ConductorApi},
                    AlwaysSendClientClaims = true,
                    ClientClaimsPrefix = ""
                },

                new Client
                {
                    ClientId = ApplicationConstants.ConductorTestClient,
                    ClientName = "Conductor Unit Test",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { ApplicationConstants.ConductorApi},
                    ClientSecrets  =
                    {
                        new Secret(ApplicationConstants.ConductorTestClientSecret.Sha256())
                    }
                }
            };
        }
    }
}
