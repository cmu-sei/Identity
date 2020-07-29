// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.IO;
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
                cfg.SwaggerEndpoint(brandingOptions.PathBase + "/api/v1/openapi.json", $"{brandingOptions.ApplicationName} (v1)");
            });

            return app;
        }
    }
}
