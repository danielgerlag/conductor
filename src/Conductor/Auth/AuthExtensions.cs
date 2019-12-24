using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Conductor.Auth
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder builder, IConfiguration config)
        {
            var publicKeyBase64 = Environment.GetEnvironmentVariable("PUBLICKEY");
            if (string.IsNullOrEmpty(publicKeyBase64))
                publicKeyBase64 = config.GetValue<string>("AuthPublicKey");
            var publicKey = Convert.FromBase64String(publicKeyBase64);
            var e1 = ECDsa.Create();
            e1.ImportSubjectPublicKeyInfo(publicKey, out int br);
            var signingKey = new ECDsaSecurityKey(e1);

            builder.AddJwtBearer(options =>
             {
                 options.IncludeErrorDetails = true;
                 options.RequireHttpsMetadata = false;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = signingKey,
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     RequireExpirationTime = false
                 };
                 options.Events = new JwtBearerEvents()
                 {
                     OnTokenValidated = context =>
                     {
                         
                         return Task.CompletedTask;
                     }
                 };

                 options.Validate();
             });

            return builder;
        }

        public static AuthenticationBuilder AddBypassAuth(this AuthenticationBuilder builder)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(new byte[121]);
            var sc = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, Policies.Admin),
                    new Claim(ClaimTypes.Role, Policies.Author),
                    new Claim(ClaimTypes.Role, Policies.Controller),
                    new Claim(ClaimTypes.Role, Policies.Viewer),
                }),
                SigningCredentials = sc,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            builder.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                };
                options.RequireHttpsMetadata = false;
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = tokenString;
                        return Task.CompletedTask;
                    }                    
                };
                options.Validate();
            });

            return builder;
        }

        public static void AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy => policy.RequireAssertion(context => context.User.Claims.Any(x => x.Type == "scope" && x.Value.Split(' ').Contains("admin"))));
                options.AddPolicy(Policies.Author, policy => policy.RequireAssertion(context => context.User.Claims.Any(x => x.Type == "scope" && x.Value.Split(' ').Contains("author"))));
                options.AddPolicy(Policies.Controller, policy => policy.RequireAssertion(context => context.User.Claims.Any(x => x.Type == "scope" && x.Value.Split(' ').Contains("controller"))));
                options.AddPolicy(Policies.Viewer, policy => policy.RequireAssertion(context => context.User.Claims.Any(x => x.Type == "scope" && x.Value.Split(' ').Contains("viewer"))));
            });
        }
    }
    
}
