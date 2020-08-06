// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityServer.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Middleware
{
    public class JsonExceptionMiddleware
    {
        public JsonExceptionMiddleware(
            RequestDelegate next,
            ILogger<JsonExceptionMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }
        private readonly RequestDelegate _next;
        private ILogger<JsonExceptionMiddleware> _logger;

        public async Task Invoke(HttpContext context)
        {
            try {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    string message = "Error";

                    if (ex is System.InvalidOperationException
                    || ex is System.ArgumentException
                    || ex.GetType().Namespace.StartsWith("Identity.")
                    ) {
                        context.Response.StatusCode = 400;
                        message = ex.GetType().Name
                            .Split('.')
                            .Last()
                            .Replace("Exception", "");
                        // message += $" {ex.Message}";
                    }

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = message }));
                }
            }

        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class JsonExceptionStartupExtensions
    {
        public static IApplicationBuilder UseJsonExceptions (
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<JsonExceptionMiddleware>();
        }
    }
}
