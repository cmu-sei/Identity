// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace IdentityServer.Options
{
    public class AuthorizationOptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public int CookieLifetimeMinutes { get; set; } = 600;
        public bool CookieSlidingExpiration { get; set; }
        public OAuth2Client SwaggerClient { get; set; }
        public ExternalOidcScheme[] ExternalOidc { get; set; } = new ExternalOidcScheme[]{};
    }

    public class OAuth2Client
    {
        public string AuthorizationUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientSecret { get; set; }

    }

    public class ExternalOidcScheme
    {
        public string Scheme { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scopes { get; set; }

    }
}
