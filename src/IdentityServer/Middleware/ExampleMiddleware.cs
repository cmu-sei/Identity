// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Middleware
{
    public class ExampleMiddleware
    {
        public ExampleMiddleware(
            RequestDelegate next
        )
        {
            _next = next;
        }
        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
        }
    }
}
