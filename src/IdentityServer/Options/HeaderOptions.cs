// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace IdentityServer.Options
{
    public class HeaderOptions
    {
        public bool LogHeaders { get; set; }
        public bool UseHsts { get; set; }
        public CorsPolicyOptions Cors { get; set; } = new CorsPolicyOptions();
        public SecurityHeaderOptions Security { get; set; } = new SecurityHeaderOptions();
        public ForwardHeaderOptions Forwarding { get; set; } = new ForwardHeaderOptions();
    }

    public class ForwardHeaderOptions
    {
        public int ForwardLimit { get; set; } = 1;
        public string KnownProxies { get; set; }
        public string KnownNetworks { get; set; }
        public string TargetHeaders { get; set; }
    }

    public class SecurityHeaderOptions
    {
        public string ContentSecurity { get; set; } = "default-src 'self' 'unsafe-inline'";
        public string XContentType { get; set; } = "nosniff";
        public string XFrame { get; set; } = "SAMEORIGIN";
    }

    public class CorsPolicyOptions
    {
        public string Name { get; set; } = "default";
        public string[] Origins { get; set; } = new string[]{};
        public string[] Methods { get; set; } = new string[]{};
        public string[] Headers { get; set; } = new string[]{};
        public bool AllowCredentials { get; set; }

        [Obsolete]
        public bool AllowAnyOrigin { get; set; }

        [Obsolete]
        public bool AllowAnyMethod { get; set; }

        [Obsolete]
        public bool AllowAnyHeader { get; set; }

        public CorsPolicy Build()
        {
            CorsPolicyBuilder policy = new CorsPolicyBuilder();

            ApplyAlls();

            var origins = Origins.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (origins.Any()) {
                if (origins.First() == "*") policy.AllowAnyOrigin(); else policy.WithOrigins(origins);
                if (AllowCredentials && origins.First() != "*") policy.AllowCredentials(); else policy.DisallowCredentials();
            }

            var methods = Methods.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (methods.Any()) {
                if (methods.First() == "*") policy.AllowAnyMethod(); else policy.WithMethods(methods);
            }

            var headers = Headers.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (headers.Any()) {
                if (headers.First() == "*") policy.AllowAnyHeader(); else policy.WithHeaders(headers);
            }

            return policy.Build();
        }

        [Obsolete]
        private void ApplyAlls()
        {
            if (AllowAnyOrigin) Origins = new string[] { "*" };
            if (AllowAnyMethod) Methods = new string[] { "*" };
            if (AllowAnyHeader) Headers = new string[] { "*" };
        }
    }
}
