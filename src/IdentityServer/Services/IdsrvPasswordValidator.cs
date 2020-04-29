// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;
using Identity.Accounts.Abstractions;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServer.Services
{
    public class IdsrvPasswordValidator : IResourceOwnerPasswordValidator
    {
        public IdsrvPasswordValidator(
            IAccountService accountService
        ){
            _accounts = accountService;
        }

        private readonly IAccountService _accounts;

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var account = await _accounts.AuthenticateWithCredentialAsync(
                new Identity.Accounts.Models.Credentials
                {
                    Username = context.UserName,
                    Password = context.Password
                },
                ""
            );

            if (account != null)
                context.Result = new GrantValidationResult(account.GlobalId, "rop");
            else
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid credential");
        }
    }
}
