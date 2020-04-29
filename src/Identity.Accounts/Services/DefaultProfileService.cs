// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Collections.Generic;
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

        public async Task<Claim[]> GetClaimsAsync(string globalId, string name)
        {
            List<Claim> claims = new List<Claim>();
            var profile = await GetProfileAsync(globalId, name) as Dictionary<string, string>;
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

        public async Task<object> GetProfileAsync(string globalId, string name)
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
                profile.Add(ClaimTypes.Email, account.GetProperty(ClaimTypes.Email) ?? $"{account.GlobalId}@{_options.Profile.Domain}");
                profile.Add(ClaimTypes.EmailVerified, (!profile[ClaimTypes.Email].StartsWith(account.GlobalId)).ToString().ToLower());

                //organization scope
                profile.Add(ClaimTypes.Org, account.GetProperty(ClaimTypes.Org));
                profile.Add(ClaimTypes.Unit, account.GetProperty(ClaimTypes.Unit));

                // set avatar defaults
                if (!string.IsNullOrEmpty(_options.Profile.ImageServerUrl))
                {
                    profile.Add(ClaimTypes.Avatar,
                        $"{_options.Profile.ImageServerUrl}/{_options.Profile.AvatarPath}/{account.GlobalId}");

                    profile.Add(ClaimTypes.OrgLogo,
                        $"{_options.Profile.ImageServerUrl}/{_options.Profile.OrgLogoPath}/{account.GetProperty(ClaimTypes.OrgLogo)}");

                    profile.Add(ClaimTypes.UnitLogo,
                        $"{_options.Profile.ImageServerUrl}/{_options.Profile.UnitLogoPath}/{account.GetProperty(ClaimTypes.UnitLogo)}");
                }
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
