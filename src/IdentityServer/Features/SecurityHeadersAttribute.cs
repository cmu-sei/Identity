// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using IdentityServer.Extensions;
using IdentityServer.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Features
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {

            var result = context.Result;
            if (result is ViewResult)
            {
                SecurityHeaderOptions options = context.HttpContext.RequestServices.GetService(typeof(SecurityHeaderOptions)) as SecurityHeaderOptions;

                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", options.XContentType);
                }
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", options.XFrame);
                }

                //var csp = "default-src 'self' 'unsafe-inline'";
                // an example if you need client images to be displayed from twitter
                //var csp = "default-src 'self'; img-src 'self' https://pbs.twimg.com";

                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", options.ContentSecurity);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", options.ContentSecurity);
                }
            }
        }
    }
}
