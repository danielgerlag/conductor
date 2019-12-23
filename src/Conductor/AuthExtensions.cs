using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Conductor
{
    public static class AuthExtensions
    {
        public static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder builder, IConfiguration config)
        {
            builder.AddJwtBearer(options =>
             {
                 var publicKey = Convert.FromBase64String(config.GetValue<string>("IssuerKey"));
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

                 options.IncludeErrorDetails = true;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new ECDsaSecurityKey(e1),
                     ValidateIssuer = false,
                     ValidateAudience = false
                 };

                 options.RequireHttpsMetadata = false;
                 options.SaveToken = true;

                 options.Validate();
             });

            return builder;
        }

        public static AuthenticationBuilder AddBypassAuth(this AuthenticationBuilder builder)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var privateKey = Convert.FromBase64String("MHcCAQEEIEVB7uPYNa0BSvKQPhXVPf0cVilo88STthQrwzIEHnfSoAoGCCqGSM49AwEHoUQDQgAEGlmSn1KFXFsQW1GjivT1cES9AD/Sl/bqwcYqdsDFRL4b56cYGK413FFPNRQS8TworgBDHIJSi1toDJ19WzhLXw==");

            var e1 = ECDsa.Create();            
            e1.ImportECPrivateKey(privateKey, out int rb1);

            var key = new ECDsaSecurityKey(e1);
            var sc = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim( ClaimTypes.Role, "admin" ),
                    new Claim( ClaimTypes.Role, "user" ),
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
                    IssuerSigningKey = new ECDsaSecurityKey(e1),
                    ValidateIssuer = false,
                    ValidateAudience = false
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
    }
    
}
