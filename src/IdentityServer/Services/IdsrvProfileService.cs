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

                var claims  = (await _profileSvc.GetClaimsAsync(id, name)).ToList();
                context.AddRequestedClaims(claims);

                // TODO: include client requested access token claims
                //      as opposed to the following hard-coded entries:
                //always include name and role claim so they are in access token too.
                if (context.Caller == "ClaimsProviderAccessToken")
                {
                    if (
                        context.Client.AllowedScopes.Contains(JwtClaimTypes.Role)
                    ) {
                        context.IssuedClaims.Add(
                            new Claim(
                                JwtClaimTypes.Role,
                                claims.FirstOrDefault(
                                    c => c.Type == JwtClaimTypes.Role
                                )?.Value ?? "member"
                            )
                        );
                    }

                    var required = new string[] { JwtClaimTypes.Name }; //, JwtClaimTypes.Role };
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
