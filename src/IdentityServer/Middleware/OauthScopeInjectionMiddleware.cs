// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Middleware
{
    public class OauthScopeInjectionMiddleware
    {
        public OauthScopeInjectionMiddleware(
            RequestDelegate next
        )
        {
            _next = next;
            _first = new PathString("/oauth");
        }
        private readonly RequestDelegate _next;
        private readonly PathString _first;

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_first))
            {
                context.Request.Path = new PathString(context.Request.Path.Value.Replace("/oauth/", "/connect/"));
                context.Request.QueryString = new QueryString(context.Request.QueryString.Value.Replace("read_user", "openid"));
            }

            await _next(context);
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    public static class OuathStartUpExtensions
    {
        public static IApplicationBuilder UseOauthScopeInjection (
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<IdentityServer.Middleware.OauthScopeInjectionMiddleware>();
        }
    }
}
