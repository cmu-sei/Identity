// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Generic;
using System.IO;
using IdentityServer;
using IdentityServer.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class SwaggerStartupExtensions
    {
        public static IServiceCollection AddSwagger(
            this IServiceCollection services,
            AuthorizationOptions authOptions,
            BrandingOptions brandingOptions
        )
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = brandingOptions.ApplicationName,
                    Version = "v1",
                    Description = "API documentation and interaction"
                });

                options.EnableAnnotations();

                if (File.Exists("IdentityServer.xml"))
                {
                    options.IncludeXmlComments("IdentityServer.xml");
                }

                // if (!string.IsNullOrEmpty(authOptions.Authority))
                // {
                //     options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //     {
                //         Type = SecuritySchemeType.OAuth2,
                //         Flows = new OpenApiOAuthFlows
                //         {
                //             Implicit = new OpenApiOAuthFlow
                //             {
                //                 AuthorizationUrl = new Uri(
                //                     authOptions.SwaggerClient?.AuthorizationUrl
                //                     ?? $"{authOptions.Authority}/connect/authorize"
                //                 ),
                //                 Scopes = new Dictionary<string, string>
                //                 {
                //                     { AppConstants.Audience, "User Access" }
                //                 }
                //             }
                //         },
                //     });

                //     options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //     {
                //         {
                //             new OpenApiSecurityScheme
                //             {
                //                 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                //             },
                //             new[] { authOptions.Audience }
                //         }
                //     });
                // }
            });

            return services;
        }

        public static IApplicationBuilder UseConfiguredSwagger(
            this IApplicationBuilder app,
            AuthorizationOptions authOptions,
            BrandingOptions brandingOptions
        )
        {
            app.UseSwagger(cfg =>
            {
                cfg.RouteTemplate = "api/{documentName}/openapi.json";
            });

            app.UseSwaggerUI(cfg =>
            {
                cfg.RoutePrefix = "api";
                cfg.SwaggerEndpoint("/api/v1/openapi.json", $"{brandingOptions.ApplicationName} (v1)");
                // cfg.OAuthClientId(authOptions.SwaggerClient?.ClientId);
                // cfg.OAuthAppName(authOptions.SwaggerClient?.ClientName ?? authOptions.SwaggerClient?.ClientId);
            });

            return app;
        }
    }
}
