// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Accounts.Abstractions
{
    public interface IProfileService
    {
        Task<Claim[]> GetClaimsAsync(string globalId, string name);
        Task<object> GetProfileAsync(string globalId, string name);
        Task<bool> IsActive(string globalId);
        Task AddProfileAsync(string globalId, string name);
    }

}
