// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Identity.Accounts.Models;

namespace Identity.Accounts.Abstractions
{
    public interface IAccountService
    {
        Task AddorUpdateAccountAsync(string accountId, string username);
        Task<Account> AddAccountCertificate(string guid, X509Certificate2 certificate);
        Task<Account> AddAccountValidatedSubject(string guid, string subject);
        Task<Account> AuthenticateWithCertificateAsync(X509Certificate2 certificate, string location);
        Task<Account> AuthenticateWithCodeAsync(Credentials creds, string location, Func<string, string, Task<bool>> dupeChecker = null);
        Task<Account> AuthenticateWithCredentialAsync(Credentials creds, string location);
        Task<Account> AuthenticateWithResetAsync(Credentials creds, string location);
        Task<Account> AuthenticateWithValidatedSubjectAsync(string subject, string location);
        Task ChangePasswordAsync(string accountId, ChangedPassword model);
        Task ClearPassword(string accountId);
        Task MergeAccounts(AccountMergeModel model);
        Task<Account> FindAsync(int id);
        Task<Account> FindByAccountAsync(string accountName);
        Task<Account> FindByGuidAsync(string guid);
        Task<AccountProfile> FindProfile(string guid, bool isSelf = false);
        Task SaveProfile(AccountProfile model);
        Task<AlternateAccountProfile> FindAlternateProfile(string guid);
        Task<Account[]> FindAll(SearchModel search, CancellationToken ct = default(CancellationToken));
        Task<Account[]> FindUpdated(string since, CancellationToken ct = default(CancellationToken));
        Task<TokenSummary> GetTokenSummary(TokenSummary model);
        Task<AccountStats> GetStats(DateTime since);
        Task<AccountCode> GenerateAccountCodeAsync(int id);
        Task<AccountCode> GenerateAccountCodeAsync(string account, bool mustExist = true);
        Task<string> GenerateAccountTOTPAsync(string guid, string issuer = null);
        Task<bool> HasAccounts();
        bool IsDomainValid(string accountName);
        bool IsExpired(DateTime dt);
        bool IsPasswordComplex(string password);
        Task<bool> IsTokenUniqueAsync(string accountName);
        Task<bool> IsCertificateUniqueAsync(string subjectDN);
        Task<bool> IsCertificateUniqueAsync(X509Certificate2 cert);
        Task<Account> RegisterWithCredentialsAsync(Credentials credentials);
        Task<Account> RegisterWithCredentialsAsync(Credentials credentials, string globalId);
        Task<Account> RegisterExternalUser(ClaimsPrincipal principal, string location);
        Task<UsernameRegistration[]> RegisterUsernames(UsernameRegistrationModel model);
        Task<Account> RegisterUsername(string userMailto, string password, string globalId = null);
        Task<Account> RegisterWithCertificateAsync(X509Certificate2 certificate);
        Task<Account> RegisterWithValidatedSubjectAsync(string subject);
        Task RemoveAccountAsync(int accountId, string accountName);
        Task SetProperties(int accountId, AccountProperty[] properties);
        Task SetProperty(AccountProperty property);
        Task SetRole(int accountId, AccountRole role);
        Task<Account> SetStatus(int accountId, AccountStatus status);
        Task<bool> TestCredentialsAsync(Credentials creds);
        Task Unlock(int accountId);
        Task<bool> ValidateAccountCodeAsync(string accountName, string code);
        // Task<bool> AddOverride(OverrideCode code);
        // Task<bool> RemoveOverride(OverrideCode code);
        Task<TotpResult> ValidateAccountTOTPAsync(string guid, string code, Func<string, string, Task<bool>> dupeChecker = null);
    }

}
