// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Data.Extensions;
using Identity.Accounts.Extensions;
using Identity.Accounts.Options;

namespace Identity.Accounts.Services
{
    public class DefaultProfileService : IProfileService
    {
        public DefaultProfileService(
            IAccountStore store,
            AccountOptions options
        ) {
            _store = store;
            _options = options;
        }

        private readonly IAccountStore _store;
        private readonly AccountOptions _options;

        public async Task<Claim[]> GetClaimsAsync(string globalId, string name, string url)
        {
            List<Claim> claims = new List<Claim>();
            var profile = await GetProfileAsync(globalId, name, url) as Dictionary<string, ClaimValue>;
            if (profile != null)
            {
                foreach (string prop in profile.Keys)
                {
                    if (profile[prop].Value.HasValue())
                    {
                        claims.Add(new Claim(prop, profile[prop].Value, profile[prop].Type));
                    }
                }
            }

            // add role claim
            var account = await _store.LoadByGuid(globalId);
            claims.Add(new Claim(ClaimTypes.Role, account.Role.ToString().ToLower()));

            return claims.ToArray();
        }

        public async Task<object> GetProfileAsync(string globalId, string name, string url)
        {
            var account = await _store.LoadByGuid(globalId);
            var profile = new Dictionary<string, ClaimValue>();
            profile.Add(ClaimTypes.Subject, new ClaimValue { Value = globalId, Type = ClaimValueTypes.String});

            if (account == null)
            {
                profile.Add(ClaimTypes.Name, new ClaimValue { Value = name, Type = ClaimValueTypes.String});
            }
            else
            {
                var uri = new Uri(url);
                string[] segments = uri.Host.Split('.');
                int x = segments.Length > 2
                    ? segments.Length - 2
                    : 0;
                string domain = string.Join('.', segments.Skip(x));

                //profile scope
                profile.Add(ClaimTypes.Name, new ClaimValue { Value = account.GetProperty(ClaimTypes.Name), Type = ClaimValueTypes.String});
                profile.Add(ClaimTypes.Username, new ClaimValue { Value = account.GetProperty(ClaimTypes.Username), Type = ClaimValueTypes.String});
                profile.Add(ClaimTypes.UpdatedAt, new ClaimValue { Value = account.UpdatedAt.ToString(), Type = ClaimValueTypes.String});

                if (profile.ContainsKey(ClaimTypes.Name))
                {
                    var nameparts = profile[ClaimTypes.Name].Value.Split(new char[] { '.', ' '});
                    if (nameparts.Length > 1)
                    {
                        profile.Add(ClaimTypes.GivenName, new ClaimValue { Value = nameparts[0], Type = ClaimValueTypes.String});
                        profile.Add(ClaimTypes.FamilyName, new ClaimValue { Value = profile[ClaimTypes.Name].Value.Substring(nameparts[0].Length + 1), Type = ClaimValueTypes.String});
                    }
                }

                //email scope
                profile.Add(ClaimTypes.Email, new ClaimValue { Value = account.GetProperty(ClaimTypes.Email) ?? $"{account.GlobalId}@{domain}", Type = ClaimValueTypes.String});
                profile.Add(ClaimTypes.EmailVerified, new ClaimValue { Value = (!profile[ClaimTypes.Email].Value.StartsWith(account.GlobalId)).ToString().ToLower(), Type = ClaimValueTypes.Boolean});

                //organization scope
                profile.Add(ClaimTypes.Org, new ClaimValue { Value = account.GetProperty(ClaimTypes.Org), Type = ClaimValueTypes.String});
                profile.Add(ClaimTypes.Unit, new ClaimValue { Value = account.GetProperty(ClaimTypes.Unit), Type = ClaimValueTypes.String});

                // set avatar defaults
                string imageServerUrl = _options.Profile.ImageServerUrl ?? url + _options.Profile.ImagePath;

                profile.Add(ClaimTypes.Avatar,
                    new ClaimValue { Value = $"{imageServerUrl}/{_options.Profile.AvatarPath}/{account.GlobalId}", Type = ClaimValueTypes.String});

                profile.Add(ClaimTypes.OrgLogo,
                    new ClaimValue { Value = $"{imageServerUrl}/{_options.Profile.OrgLogoPath}/{account.GetProperty(ClaimTypes.OrgLogo)}", Type = ClaimValueTypes.String});

                profile.Add(ClaimTypes.UnitLogo,
                    new ClaimValue { Value = $"{imageServerUrl}/{_options.Profile.UnitLogoPath}/{account.GetProperty(ClaimTypes.UnitLogo)}", Type = ClaimValueTypes.String});
            }

            return profile;
        }

        public async Task AddProfileAsync(string globalId, string name)
        {
            await Task.Delay(0);
        }

        public async Task<bool> IsActive(string globalId)
        {
            var account = await _store.LoadByGuid(globalId);
            return account?.Status == AccountStatus.Enabled;
        }
    }
}
