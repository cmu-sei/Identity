// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Threading.Tasks;
using IdentityServer4.Models;
using Identity.Clients.Services;
using Identity.Clients.Abstractions;
using System.Linq;
using System;
using Identity.Clients.Extensions;
using IdentityServer.Extensions;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Services
{
    public class IdsrvClientStore : IdentityServer4.Stores.IClientStore
    {

        public IdsrvClientStore(
            ClientService svc,
            ResourceService resources,
            ILogger<IdsrvClientStore> logger
        )
        {
            _svc = svc;
            _resources = resources;
            _logger = logger;
        }

        private readonly ClientService _svc;
        private readonly ResourceService _resources;
        private readonly ILogger<IdsrvClientStore> _logger;

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            try
            {

                var model = await _svc.LoadDetail(clientId);

                var allowedScopes = _resources.LoadAll().Result
                    .Where(r => r.Type == ResourceType.Identity && r.Default)
                    .Select(r => r.Name)
                    .ToArray();

                foreach (string scope in allowedScopes)
                    if (model.Scopes.HasValue() && !model.Scopes.Contains(scope))
                        model.Scopes += " " + scope;

                string clientUrl = model.RedirectUrls
                    .Select(u => u.Value)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(clientUrl))
                {
                    var url = new Uri(clientUrl);
                    clientUrl = url.AbsoluteUri.Substring(0, url.AbsoluteUri.Length - url.AbsolutePath.Length);
                }

                Client client = new Client
                {
                    // defaults
                    // BackChannelLogoutSessionRequired = true,
                    // FrontChannelLogoutSessionRequired = true,
                    // IncludeJwtId = false,
                    // EnableLocalLogin = true,
                    // RequireClientSecret = true,
                    // AccessTokenType = AccessTokenType.Jwt,
                    // ProtocolType = "oidc",
                    // AllowPlainTextPkce = false,
                    RequireClientSecret = model.Secrets.Any(),

                    // client
                    Enabled = model.Enabled,
                    ClientId = model.Name,
                    ClientName = model.DisplayName,
                    Description = model.Description,
                    AllowedGrantTypes = model.Grants.Split(" "),
                    AllowedScopes = model.Scopes?.Split(" "),
                    PairWiseSubjectSalt = model.PairWiseSubjectSalt,
                    RequirePkce = model.RequirePkce,
                    AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,

                    // consent behavior
                    RequireConsent = model.RequireConsent,
                    AllowRememberConsent = model.ConsentLifetime.ToSeconds() > 0,
                    ConsentLifetime = model.ConsentLifetime.ToSeconds() > 0
                        ? model.ConsentLifetime.ToSeconds()
                        : (int?)null,

                    // token behavior
                    AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                    IdentityTokenLifetime = model.IdentityTokenLifetime.ToSeconds(),
                    AccessTokenLifetime = model.AccessTokenLifetime.ToSeconds(),
                    AuthorizationCodeLifetime = model.AuthorizationCodeLifetime.ToSeconds(),

                    // refresh behavior
                    AllowOfflineAccess = model.AllowOfflineAccess,
                    UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
                    AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime.ToSeconds(),
                    SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime.ToSeconds(),
                    RefreshTokenUsage = model.UseOneTimeRefreshTokens ? TokenUsage.OneTimeOnly : TokenUsage.ReUse,
                    RefreshTokenExpiration = model.SlidingRefreshTokenLifetime.ToSeconds() > 0 ? TokenExpiration.Sliding : TokenExpiration.Absolute,

                    // client claims
                    AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                    ClientClaimsPrefix = model.ClientClaimsPrefix,
                    Claims = model.Claims.Select(c => new IdentityServer4.Models.ClientClaim(c.Type, c.Value)).ToArray(),

                    // urls
                    ClientUri = model.Url,
                    LogoUri = model.LogoUrl,
                    BackChannelLogoutUri = model.BackChannelLogoutUrl,
                    FrontChannelLogoutUri = model.FrontChannelLogoutUrl,
                    RedirectUris = model.RedirectUrls.Select(u => u.Value).ToArray(),
                    PostLogoutRedirectUris = model.PostLogoutUrls.Select(u => u.Value).ToArray(),
                    AllowedCorsOrigins = model.CorsUrls.Select(u => u.Value).ToArray(),

                    // secrets
                    ClientSecrets = model.Secrets.Select(s => new Secret{
                        Value = s.Value,
                        Expiration = s.Expiration,
                        Type = s.Type,
                        Description = s.Description
                    }).ToArray(),
                };

                if (client.AllowedCorsOrigins.Count == 0 && clientUrl.HasValue())
                    client.AllowedCorsOrigins = new string[] { clientUrl };

                return client;

            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
            }

            return null;
        }
    }
}
