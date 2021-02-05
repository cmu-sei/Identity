// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Services
{
    public class IdsrvProfileService : IProfileService
    {
        public IdsrvProfileService(
            Identity.Accounts.Abstractions.IProfileService profileSvc,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _profileSvc = profileSvc;
            _http = httpContextAccessor.HttpContext;
        }

        private readonly Identity.Accounts.Abstractions.IProfileService _profileSvc;
        private readonly HttpContext _http;

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string id = context.Subject.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = context.Subject.FindFirst(JwtClaimTypes.Name)?.Value;

            if (!String.IsNullOrEmpty(id))
            {
                string host = $"{_http.Request.Scheme}://{_http.Request.Host.Value}{_http.Request.PathBase}";
                var claims  = (await _profileSvc.GetClaimsAsync(id, name, host)).ToList();
                context.AddRequestedClaims(claims);

                if (context.Caller == "ClaimsProviderAccessToken")
                {
                    // Always include Name in claims
                    var required = new string[] { JwtClaimTypes.Name };
                    foreach (string scope in required)
                    {
                        string val = claims.Where(c => c.Type == scope).Select(c => c.Value).FirstOrDefault() ?? name ?? "anonymous";
                        if (!context.IssuedClaims.Any(c => c.Type == scope))
                            context.IssuedClaims.Add(new Claim(scope, val));
                    }
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = await _profileSvc.IsActive(context.Subject.GetSubjectId());
        }
    }
}
