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
            var profile = await GetProfileAsync(globalId, name, url) as Dictionary<string, string>;
            if (profile != null)
            {
                foreach (string prop in profile.Keys)
                {
                    if (profile[prop].HasValue())
                    {
                        claims.Add(new Claim(prop, profile[prop]));
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
            var profile = new Dictionary<string,string>();
            profile.Add(ClaimTypes.Subject, globalId);

            if (account == null)
            {
                profile.Add(ClaimTypes.Name, name);
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
                profile.Add(ClaimTypes.Name,  account.GetProperty(ClaimTypes.Name));
                profile.Add(ClaimTypes.Username, account.GetProperty(ClaimTypes.Username));
                profile.Add(ClaimTypes.UpdatedAt, account.UpdatedAt.ToString());

                if (profile.ContainsKey(ClaimTypes.Name))
                {
                    var nameparts = profile[ClaimTypes.Name].Split(new char[] { '.', ' '});
                    if (nameparts.Length > 1)
                    {
                        profile.Add(ClaimTypes.GivenName, nameparts[0]);
                        profile.Add(ClaimTypes.FamilyName, profile[ClaimTypes.Name].Substring(nameparts[0].Length + 1));
                    }
                }

                //email scope
                profile.Add(ClaimTypes.Email, account.GetProperty(ClaimTypes.Email) ?? $"{account.GlobalId}@{domain}");
                profile.Add(ClaimTypes.EmailVerified, (!profile[ClaimTypes.Email].StartsWith(account.GlobalId)).ToString().ToLower());

                //organization scope
                profile.Add(ClaimTypes.Org, account.GetProperty(ClaimTypes.Org));
                profile.Add(ClaimTypes.Unit, account.GetProperty(ClaimTypes.Unit));

                // set avatar defaults
                string imageServerUrl = _options.Profile.ImageServerUrl ?? url + _options.Profile.ImagePath;

                profile.Add(ClaimTypes.Avatar,
                    $"{imageServerUrl}/{_options.Profile.AvatarPath}/{account.GlobalId}");

                profile.Add(ClaimTypes.OrgLogo,
                    $"{imageServerUrl}/{_options.Profile.OrgLogoPath}/{account.GetProperty(ClaimTypes.OrgLogo)}");

                profile.Add(ClaimTypes.UnitLogo,
                    $"{imageServerUrl}/{_options.Profile.UnitLogoPath}/{account.GetProperty(ClaimTypes.UnitLogo)}");
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
