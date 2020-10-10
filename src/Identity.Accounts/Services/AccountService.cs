// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Data.Extensions;
using Identity.Accounts.Exceptions;
using Identity.Accounts.Extensions;
using Identity.Accounts.Models;
using Identity.Accounts.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace Identity.Accounts.Services
{
    public class AccountService : IAccountService
    {
        public AccountService(
            IAccountStore store,
            AccountOptions options,
            ILogger<AccountService> logger,
            IIssuerService issuerService,
            IProfileService profileService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor = null
        ){
            _options = options;
            _logger = logger;
            _certStore = issuerService;
            _profileService = profileService ?? new DefaultProfileService(store, _options);
            _rand = new Random();
            _store = store;
            Mapper = mapper;

            var http = httpContextAccessor?.HttpContext;

            string url = http is HttpContext
                ? $"{http.Request.Scheme}://{http.Request.Host.Value}{http.Request.PathBase}"
                : "";

            _serviceUrl = options.Profile.ImageServerUrl ?? url + options.Profile.ImagePath;

        }

        protected readonly IAccountStore _store;
        protected readonly AccountOptions _options;
        protected readonly ILogger _logger;
        protected readonly IIssuerService _certStore;
        protected readonly IProfileService _profileService;
        protected IMapper Mapper { get; }
        protected Random _rand;
        string _serviceUrl = "";

        #region Registration

        public async Task<Account> RegisterWithCredentialsAsync(Credentials credentials)
        {
            return await RegisterWithCredentialsAsync(credentials, Guid.NewGuid().ToString());
        }

        public async Task<Account> RegisterWithCredentialsAsync(Credentials credentials, string globalId)
        {
            if (!IsDomainValid(credentials.Username))
                throw new RegistrationDomainException();

            if (!IsPasswordComplex(credentials.Password))
                throw new PasswordComplexityException();

            // NOTE: consumers responsible for verifying username

            Data.Account account = await Register(
                credentials.Username,
                credentials.DisplayName,
                AccountTokenType.Credential,
                credentials.IsAffiliate,
                globalId
            );

            if (_options.Registration.StoreEmail && credentials.Username.IsEmailAddress())
                UpdateProperty(account, ClaimTypes.Email, credentials.Username);

            await UpdatePasswordAsync(account, credentials.Password);

            return Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                });
        }

        public async Task<UsernameRegistration[]> RegisterUsernames(UsernameRegistrationModel model)
        {
           var results = new List<UsernameRegistration>();
            foreach (string userMailto in model.Usernames)
            {
                if (string.IsNullOrWhiteSpace(userMailto))
                    continue;

                var result = new UsernameRegistration(userMailto);
                try
                {
                    await RegisterUsername(userMailto, model.Password);
                }
                catch (Exception ex)
                {
                    result.Message = ex.GetType().Name.Split(".").Last().Replace("Exception", "");
                }
                results.Add(result);
            }
            return results.ToArray();
        }

        public async Task<Account> RegisterUsername(string userMailto, string password, string globalId = null)
        {

            var registration = new UsernameRegistration(userMailto);

            Data.Account account = await Register(
                registration.Username,
                registration.DisplayName,
                AccountTokenType.Credential,
                registration.IsAffiliate,
                globalId ?? Guid.NewGuid().ToString()
            );

            if (_options.Registration.StoreEmail && registration.Username.IsEmailAddress())
            {
                UpdateProperty(account, ClaimTypes.Email, registration.Username);
                await _store.Update(account);
            }

            if (password.HasValue())
            {
                await UpdatePasswordAsync(account, password);
            }

            return Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                });
        }

        public async Task<Account> RegisterWithCertificateAsync(X509Certificate2 certificate)
        {
            _certStore.Validate(certificate);  //throws on invalid
            return await RegisterWithValidatedSubjectAsync(certificate.Subject);
        }

        public async Task<Account> RegisterWithValidatedSubjectAsync(string subject)
        {
            var detail = new CertificateSubjectDetail(subject);
            Data.Account account = await Register(
                detail.ExternalId,
                detail.DisplayName,
                AccountTokenType.Certificate,
                detail.IsAffiliate
            );
            return Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                });
        }

        public async Task<Account> RegisterExternalUser(ClaimsPrincipal principal, string location)
        {
            // add account & properties
            string updated_at = principal.FindFirst(ClaimTypes.UpdatedAt)?.Value;
            if (!DateTime.TryParse(updated_at, out DateTime lastUpdate))
                lastUpdate = DateTime.MinValue;

            var subClaim = principal.FindFirst(ClaimTypes.Subject);
            var account = await _store.LoadByGuid(subClaim.Value);
            if (account == null)
            {
                account = new Data.Account {
                    GlobalId = subClaim.Value,
                    WhenCreated = DateTime.UtcNow,
                    UpdatedAt = lastUpdate
                };

                await _store.Add(account);

                await SetAccountNames(account, principal.FindFirst(ClaimTypes.Name)?.Value ?? "anonymous", false);
                UpdateProperty(account, "origin", subClaim.Issuer);
                UpdateExternalUserProfile(account, principal);
            }
            else
            {
                // sync props
                if (lastUpdate.CompareTo(account.UpdatedAt) > 0)
                {
                    UpdateExternalUserProfile(account, principal);
                    account.UpdatedAt = lastUpdate;
                }
            }
            return await CompleteAuthentication(account, location);
        }

        private void UpdateExternalUserProfile(Data.Account account, ClaimsPrincipal principal)
        {
            UpdateProperty(account, ClaimTypes.Name, principal.FindFirst(ClaimTypes.Name)?.Value);
            UpdateProperty(account, ClaimTypes.Avatar, principal.FindFirst(ClaimTypes.Avatar)?.Value);
            UpdateProperty(account, ClaimTypes.Org, principal.FindFirst(ClaimTypes.Org)?.Value);
            UpdateProperty(account, ClaimTypes.IdAffiliate, principal.FindFirst(ClaimTypes.IdAffiliate)?.Value);
            UpdateProperty(account, ClaimTypes.Unit, principal.FindFirst(ClaimTypes.Unit)?.Value);
            UpdateProperty(account, ClaimTypes.OrgLogo, principal.FindFirst(ClaimTypes.OrgLogo)?.Value);
            UpdateProperty(account, ClaimTypes.UnitLogo, principal.FindFirst(ClaimTypes.UnitLogo)?.Value);
        }

        protected async Task<Data.Account> Register(string accountName, string name, AccountTokenType type, bool id_affiliate, string globalId = "")
        {
            if (!accountName.HasValue())
                throw new AccountTokenInvalidException();

            if (!IsTokenUniqueAsync(accountName).Result)
                throw new AccountNotUniqueException();

            Data.Account account = new Data.Account {
                GlobalId = (globalId.HasValue()) ? globalId : Guid.NewGuid().ToString(),
                WhenCreated = DateTime.UtcNow,
                WhenAuthenticated = DateTime.UtcNow
            };

            if (!await HasAccounts())
                account.Role = AccountRole.Administrator;

            await _store.Add(account);

            account.Tokens.Add(new Data.AccountToken {
                Hash = accountName.ToNormalizedSha256(),
                WhenCreated = DateTime.UtcNow,
                Type = type
            });
            await _store.Update(account);

            if (_options.Registration.StoreName)
            {
                await SetAccountNames(account, name, id_affiliate);
            }

            await _profileService.AddProfileAsync(account.GlobalId, name);

            return account;
        }

        private async Task SetAccountNames(Data.Account account, string name, bool id_affiliate)
        {
            UpdateProperty(account, "name", name);
            UpdateProperty(account, "username", $"{name.ToAccountSlug()}.{account.Id.ToString("x4")}");
            if (id_affiliate)
                UpdateProperty(account, ClaimTypes.IdAffiliate, "true");
            await _store.Update(account);
        }

        #endregion

        #region Authentication

        public async Task<Account> AuthenticateWithCredentialAsync(Credentials creds, string location)
        {
            var account = await GetValidAccountAsync(creds); //throws on error

            return await CompleteAuthentication(account, location);
        }

        public async Task<bool> TestCredentialsAsync(Credentials creds)
        {
            var account = await GetValidAccountAsync(creds);
            return account != null;
        }

        private async Task<Data.Account> GetValidAccountAsync(Credentials creds)
        {
            Data.Account account = await _store.LoadByToken(creds.Username);

            if (account == null)
                throw new AuthenticationFailureException();

            if (account.Status == AccountStatus.Disabled)
                throw new AccountDisabledException();

            if (account.IsLocked())
            {
                string duration = account.LockDurationSeconds().ToString();
                throw new AccountLockedException(duration);
            }

            if (account.HasExpiredPassword(_options.Password.Age))
                throw new PasswordExpiredException();

            if (!account.VerifyPassword(creds.Password))
            {
                account.Lock(_options.Authentication.LockThreshold);
                await _store.Update(account);
                throw new AuthenticationFailureException();
            }

            return account;
        }

        public async Task<Account> AuthenticateWithCertificateAsync(X509Certificate2 certificate, string location)
        {
            _certStore.Validate(certificate);  //throws on invalid
            return await AuthenticateWithValidatedSubjectAsync(certificate.Subject, location);
        }

        public async Task<Account> AuthenticateWithValidatedSubjectAsync(string subject, string location)
        {
            var detail = new CertificateSubjectDetail(subject);

            Data.Account account = await _store.LoadByToken(detail.ExternalId);

            if (account == null)
            {
                if (detail.DeprecatedExternalId.HasValue())
                {
                    account = await _store.LoadByToken(detail.DeprecatedExternalId);
                    if (account != null)
                    {
                        var token = account.Tokens.Where(t => t.Hash == detail.DeprecatedExternalId.ToNormalizedSha256()).Single();
                        account.Tokens.Remove(token);
                        account.Tokens.Add(new Data.AccountToken
                        {
                            Type = AccountTokenType.Certificate,
                            Hash = detail.ExternalId.ToNormalizedSha256(),
                            WhenCreated = DateTime.UtcNow,
                        });

                        await _store.Update(account);
                    }
                }

                if (account == null)
                {
                    account = await Register(detail.ExternalId, detail.DisplayName, AccountTokenType.Certificate, detail.IsAffiliate);
                }
            }

            return await CompleteAuthentication(account, location);
        }

        public async Task<Account> AuthenticateWithCodeAsync(Credentials creds, string location, Func<string, string, Task<bool>> dupeChecker = null)
        {
            Data.Account account = await _store.LoadByToken(creds.Username);
            if (account == null)
                throw new AuthenticationFailureException();

            if (account.Status == AccountStatus.Disabled)
                throw new AccountDisabledException();

            if (account.IsLocked())
            {
                string duration = account.LockDurationSeconds().ToString();
                _logger.LogDebug("{0} lock duration is {1}", account.Id, duration);
                throw new AccountLockedException(duration);
            }

            if (! await ValidateAccountCodeAsync(creds.Username, creds.Code)
                && !(await ValidateAccountTOTPAsync(account.GlobalId, creds.Code, dupeChecker)).Valid)
            {
                account.Lock(_options.Authentication.LockThreshold);
                await _store.Update(account);
                throw new AccountTokenInvalidException();
            }

            return await CompleteAuthentication(account, location);
        }

        public async Task<Account> AuthenticateWithResetAsync(Credentials creds, string location)
        {
            Data.Account account = await _store.LoadByToken(creds.Username);
            if (account == null)
                throw new AuthenticationFailureException();

            if (account.Status == AccountStatus.Disabled)
                throw new AccountDisabledException();

            if (!IsPasswordComplex(creds.Password))
                throw new PasswordComplexityException();

            if (account.HasHistoricalPassword(creds.Password, _options.Password.History))
                throw new PasswordHistoryException();

            // NOTE: consumers responsible for verifiying username
            // if (! await ValidateAccountCodeAsync(creds.Username, creds.Code)
            //     && !IsInitialReset(account, creds.Code)
            // )
            // {
            //     account.Lock(_options.Authentication.LockThreshold);
            //     await _store.Update(account);
            //     throw new AccountNotConfirmedException();
            // }

            await UpdatePasswordAsync(account, creds.Password);

            return await CompleteAuthentication(account, location);
        }

        protected async Task<Account> CompleteAuthentication(Data.Account account, string location)
        {
            account.SetAuthenticated(location);
            OnUserAuthenticated(account);
            await _store.Update(account);
            return Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                });
        }

        protected virtual void OnUserAuthenticated(Data.Account account)
        {
        }

        public async Task<Account> AddAccountCertificate(string accountId, X509Certificate2 cert)
        {
            _certStore.Validate(cert);
            return await AddAccountValidatedSubject(accountId, cert.Subject);
        }

        public async Task<Account> AddAccountValidatedSubject(string accountId, string subject)
        {
            var detail = new CertificateSubjectDetail(subject);
            return await AddCertificateToken(accountId, detail.ExternalId);
        }

        /// <summary>
        /// Add a certificate to an existing account.
        /// </summary>
        /// <param name="certificateToken"></param>
        /// <param name="name"></param>
        /// <param name="creds"></param>
        /// <returns></returns>
        /// <remarks>
        ///
        /// </remarks>
        protected async Task<Account> AddCertificateToken(string accountId, string certificateToken)
        {
            Data.Account account = await _store.LoadByGuid(accountId);
            if (account == null)
                throw new AuthenticationFailureException();

            if (account.Status == AccountStatus.Disabled)
                throw new AccountDisabledException();

            if (! await IsTokenUniqueAsync(certificateToken))
                throw new AccountNotUniqueException();

            account.Tokens.Add(new Data.AccountToken
            {
                Type = AccountTokenType.Certificate,
                Hash = certificateToken.ToNormalizedSha256(),
                WhenCreated = DateTime.UtcNow
            });

            await _store.Update(account);

            return await CompleteAuthentication(account, "");
        }

        /// <summary>
        /// Merge two accounts.
        /// </summary>
        /// <remarks>
        /// Useful when somebody registers a new certificate
        /// that doesn't match the previous cert's key. Merging the new account
        /// into the old account brings the new certificate token under the old
        /// account id.
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task MergeAccounts(AccountMergeModel model)
        {
            _logger.LogDebug($"Attempting merge of {model.DefunctGlobalId} into {model.ActiveGlobalId}");

            if (!model.DefunctGlobalId.HasValue()
                || !model.ActiveGlobalId.HasValue()
                || model.DefunctGlobalId == model.ActiveGlobalId
            ) {
                throw new AccountNotFoundException();
            }

            var defunct = await _store.LoadByGuid(model.DefunctGlobalId);
            var active = await _store.LoadByGuid(model.ActiveGlobalId);

            if (defunct == null || active == null)
                throw new AccountNotFoundException();

            var tokens = defunct.Tokens
                .Where(t =>
                    t.Type == AccountTokenType.Certificate ||
                    t.Type == AccountTokenType.Credential)
                .ToArray();

            foreach (var token in tokens)
            {
                defunct.Tokens.Remove(token);
                token.UserId = active.Id;
                active.Tokens.Add(token);
            }

            foreach (var property in defunct.Properties
                .Where(p => p.Key == ClaimTypes.Email))
            {
                defunct.Properties.Remove(property);
                property.AccountId = active.Id;
                active.Properties.Add(property);
            }

            await _store.Update(active);
            await _store.Delete(defunct.Id);

            _logger.LogInformation($"Merged account tokens from {model.DefunctGlobalId} into {model.ActiveGlobalId}");

        }
        #endregion

        #region Token Generation

        public async Task<AccountCode> GenerateAccountCodeAsync(string accountName, bool mustExist = true)
        {
            if (mustExist && await IsTokenUniqueAsync(accountName))
                return new AccountCode();

            return await GenerateAccountCodeWithHashAsync(accountName.ToNormalizedSha256());
        }

        public async Task<AccountCode> GenerateAccountCodeAsync(int id)
        {
            var account = await _store.Load(id);
            var token = account.Tokens.Where(t => t.Type == AccountTokenType.Credential).FirstOrDefault();
            if (token == null)
                return new AccountCode();

            return await GenerateAccountCodeWithHashAsync(token.Hash);
        }

        public async Task<AccountCode> GenerateAccountCodeWithHashAsync(string accountHash)
        {
            Data.AccountCode accountCode = new Data.AccountCode
            {
                Hash = accountHash,
                Code = _rand.Next(100000, 1000000),
                WhenCreated = DateTime.UtcNow
            };
            await _store.Save(accountCode);
            return Mapper.Map<AccountCode>(accountCode);
        }

        public async Task<string> GenerateAccountTOTPAsync(string globalId, string issuer = null)
        {
            var account = await _store.LoadByGuid(globalId);

            if (account == null)
                return "";

            var token = account.Tokens.Where(t => t.Type == AccountTokenType.TOTP).FirstOrDefault();

            if (token != null)
            {
                account.Tokens.Remove(token);
            }

            token = new Data.AccountToken
            {
                Type = AccountTokenType.TOTP,
                Hash = Guid.NewGuid().ToString().ToNormalizedSha1(),
                WhenCreated = DateTime.UtcNow,
                UserId = account.Id
            };

            account.Tokens.Add(token);
            await _store.Update(account);

            string secret = Base32Encoding.ToString(Encoding.UTF8.GetBytes(token.Hash)).Replace("=","").ToLower();
            string username = account.GetProperty("username");
            string result = $"otpauth://totp/{username}?secret={secret}"; //&algorithm=SHA256";
            if (!string.IsNullOrEmpty(issuer))
                result += $"&issuer={issuer}";
            return result;
        }

        public async Task<TotpResult> ValidateAccountTOTPAsync(string globalId, string code, Func<string, string, Task<bool>> dupeChecker = null)
        {
            var result = new TotpResult
            {
                Code = code,
                Timestamp = DateTime.UtcNow
            };

            if (!Int32.TryParse(code, out int number))
                return result;

            var account = await _store.LoadByGuid(globalId);
            if (account != null)
            {
                var token = account.Tokens.Where(t => t.Type == AccountTokenType.TOTP).FirstOrDefault();
                if (token != null)
                {
                    var otp = new OtpNet.Totp(Encoding.UTF8.GetBytes(token.Hash)); //, mode: OtpHashMode.Sha256);
                    if (otp.VerifyTotp(code, out long matchedStep, new OtpNet.VerificationWindow(1, 1)))
                    {
                        result.Valid = (dupeChecker != null)
                            ? await dupeChecker.Invoke(globalId, matchedStep.ToString())
                            : true;
                    }
                }
            }
            return result;
        }

        public async Task<bool> ValidateAccountCodeAsync(string accountName, string code)
        {
            // check global override
            var overrideCode = await _store.GetOverrideCode(code);
            if (overrideCode != null)
                return true;

            // check initial password reset
            if (
                _options.Password.InitialResetCode.HasValue() &&
                code == _options.Password.InitialResetCode
            ){
                var account = await _store.LoadByToken(accountName);
                if (account != null && !account.Tokens.Where(t => t.Type == AccountTokenType.Password).Any())
                {
                    return true;
                }
            }

            // check account code
            if (Int32.TryParse(code, out int dynamicCode))
            {
                var token = await _store.GetAccountCode(accountName);
                if (token != null)
                {
                    bool result = token.Code == dynamicCode && !IsExpired(token.WhenCreated);
                    await _store.Delete(token);
                    return result;
                }
            }

            return false;
        }

        public bool IsInitialReset(Data.Account account, string code)
        {
            return !account.Tokens.Any(t => t.Type == AccountTokenType.Password)
                && code == _options.Password.InitialResetCode;
        }

        #endregion

        #region Account Modification

        /// <summary>
        /// Run any fix up procedures that aren't handled in migrations
        /// </summary>
        /// <returns></returns>
        public async Task FixAccounts()
        {
            if (_options.Registration.StoreName)
                await _store.FixUsernames();
        }

        /// <summary>
        /// Change account password.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ChangePasswordAsync(string accountId, ChangedPassword model)
        {
            Data.Account account = await _store.LoadByGuid(accountId);

            if (account == null)
                throw new AuthenticationFailureException();

            if (account.Status == AccountStatus.Disabled)
                throw new AccountDisabledException();

            if (!IsPasswordComplex(model.Value))
                throw new PasswordComplexityException();

            if (account.HasHistoricalPassword(model.Value, _options.Password.History))
                throw new PasswordHistoryException();

            if (account.Tokens.Any(t => t.Type == AccountTokenType.Password) &&
                !account.VerifyPassword(model.CurrentPassword))
                throw new AuthenticationFailureException();

            await UpdatePasswordAsync(account, model.Value);
        }

        private async Task UpdatePasswordAsync(Data.Account account, string password)
        {
            account.TrimPasswordHistory(_options.Password.History);
            await _store.Update(account);

            account.Tokens.Add(new Data.AccountToken
            {
                UserId = account.Id,
                Hash = account.GeneratePasswordHash(password),
                WhenCreated = DateTime.UtcNow,
                Type = AccountTokenType.Password
            });
            await _store.Update(account);
        }

        public async Task ClearPassword(string accountId)
        {
            Data.Account account = await _store.LoadByGuid(accountId);

            if (account == null)
                throw new AuthenticationFailureException();

            foreach (var token in account.Tokens.Where(t => t.Type == AccountTokenType.Password).ToArray())
            {
                account.Tokens.Remove(token);
            }

            await _store.Update(account);
        }
        /// <summary>
        /// Add a username to an existing account.
        /// Primarily an admin function as it bypasses domain checks.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task AddAccountUsernameAsync(string accountId, string username)
        {
            if (! await IsTokenUniqueAsync(username))
                throw new AccountNotUniqueException();

            var account = await _store.LoadByGuid(accountId);

            if (account == null)
                throw new InvalidOperationException();

            account.Tokens.Add(new Data.AccountToken
            {
                Hash = username.ToNormalizedSha256(),
                Type = AccountTokenType.Credential,
                WhenCreated = DateTime.UtcNow
            });

            if (_options.Registration.StoreEmail && username.IsEmailAddress())
            {
                account.Properties.Add(new Data.AccountProperty
                {
                    Key = ClaimTypes.Email,
                    Value = username
                });
            }
                // UpdateProperty(account, ClaimTypes.Email, username);

            await _store.Update(account);
        }

        /// <summary>
        /// Add or Update an email address to an existing account.
        /// If multiple addresses are disallowed, the existing address
        /// will replaced.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task AddorUpdateAccountAsync(string accountId, string username)
        {
            if (! await IsTokenUniqueAsync(username))
                throw new AccountNotUniqueException();

            var account = await _store.LoadByGuid(accountId);

            if (account == null)
                throw new InvalidOperationException();

            if (!IsDomainValid(username) &&
                !account.Tokens.Any(t => t.Type == AccountTokenType.Certificate))
                throw new RegistrationDomainException();

            // if multiples not allowed, remove existing
            var existing = account.Tokens.FirstOrDefault(t => t.Type == AccountTokenType.Credential);
            if (existing != null && !_options.Registration.AllowMultipleUsernames)
            {
                account.Tokens.Remove(existing);
                var prop = account.Properties.FirstOrDefault(p => p.Key == ClaimTypes.Email);
                if (prop != null)
                    account.Properties.Remove(prop);
            }

            account.Tokens.Add(new Data.AccountToken
            {
                Hash = username.ToNormalizedSha256(),
                Type = AccountTokenType.Credential,
                WhenCreated = DateTime.UtcNow
            });

            if (_options.Registration.StoreEmail && username.IsEmailAddress())
            {
                account.Properties.Add(new Data.AccountProperty
                {
                    Key = ClaimTypes.Email,
                    Value = username
                });
            }

            await _store.Update(account);
        }

        /// <summary>
        /// Removes an account token (email address, certificate id)
        /// if it isn't the last one. Not recommended.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public async Task RemoveAccountAsync(int accountId, string accountName)
        {
            Data.Account account = await _store.Load(accountId);

            if (account == null)
                throw new AccountNotFoundException();

            if (account.Tokens.Count(t =>
                t.Type == AccountTokenType.Certificate ||
                t.Type == AccountTokenType.Credential
                ) < 2
            ) {
                throw new AccountRemovalException();
            }

            var token = account.Tokens
                .Where(t =>
                    t.Hash == accountName.ToNormalizedSha256() ||
                    t.Hash == accountName.ToNormalizedSha1()
                ).SingleOrDefault();

            if (token != null)
            {
                account.Tokens.Remove(token);
                await _store.Update(account);
            }
        }

        private void UpdateProperty(Data.Account account, string key, string val)
        {
            var prop = account.Properties.SingleOrDefault(p => p.Key == key);
            if (!val.HasValue())
            {
                if (prop != null)
                    account.Properties.Remove(prop);
                return;
            }

            if (prop == null)
            {
                prop = new Data.AccountProperty { Key = key };
                account.Properties.Add(prop);
            }
            prop.Value = val;
        }

        public async Task SetProperties(int accountId, AccountProperty[] props)
        {
            Data.Account account = await _store.Load(accountId);
            foreach (var prop in props)
                UpdateProperty(account, prop.Key, prop.Value);
            await _store.Update(account);
        }

        public async Task SetProperty(AccountProperty property)
        {
            var account = await _store.Load(property.AccountId);
            if (account != null)
            {
                await SetProperties(account.Id, new AccountProperty[] { property });
            }
        }

        public async Task SetRole(int accountId, AccountRole role)
        {
            Data.Account account = await _store.Load(accountId);

            if (account == null)
                throw new AccountNotFoundException();

            account.Role = role;

            await _store.Update(account);

        }

        public async Task<Account> SetStatus(int accountId, AccountStatus status)
        {
            Data.Account account = await _store.Load(accountId);

            if (account == null)
                throw new AccountNotFoundException();

            account.Status = status;

            await _store.Update(account);

            return Mapper.Map<Account>(account, opts => {
                opts.Items["serviceUrl"] = _serviceUrl;
                opts.Items["profileOptions"] = _options.Profile;
            });
        }

        public async Task Unlock(int accountId)
        {
            Data.Account account = await _store.Load(accountId);

            if (account == null)
                throw new AccountNotFoundException();

            account.Unlock();

            await _store.Update(account);
        }

        #endregion

        #region Query

        public async Task<Account> FindAsync(int id)
        {
            Data.Account account = await _store.Load(id);
            return (account != null)
                ? Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                })
                : null;
        }

        public async Task<Account> FindByAccountAsync(string accountName)
        {
            Data.Account account = await _store.LoadByToken(accountName);
            return (account != null)
                ? Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                })
                : null;
        }

        public async Task<Account> FindByGuidAsync(string guid)
        {
            Data.Account account = await _store.LoadByGuid(guid);
            return (account != null)
                ? Mapper.Map<Account>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                })
                : null;
        }

        public async Task<AccountProfile> FindProfile(string guid, bool isSelf = false)
        {
            Data.Account account = await _store.LoadByGuid(guid);
            // TODO: consider returning filtered claims here
            return (account != null && (isSelf || account.IsPublic || _options.Profile.ForcePublic))
                ? Mapper.Map<AccountProfile>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                })
                : new AccountProfile();
        }

        public async Task SaveProfile(AccountProfile model)
        {
            Data.Account account = await _store.LoadByGuid(model.GlobalId);
            if (account != null)
            {
                if (model.Name.HasValue()) {
                    UpdateProperty(account, ClaimTypes.Name, model.Name);
                }
                UpdateProperty(account, ClaimTypes.Biography, model.Biography);
                UpdateProperty(account, ClaimTypes.Org, model.Org);
                UpdateProperty(account, ClaimTypes.Unit, model.Unit);

                string logo = Path.GetFileName(model.OrgLogo).Before('?');
                if (logo == _options.Profile.DefaultLogo) logo = "";
                UpdateProperty(account, ClaimTypes.OrgLogo, logo);

                logo = Path.GetFileName(model.UnitLogo).Before('?');
                if (logo == _options.Profile.DefaultLogo) logo = "";
                UpdateProperty(account, ClaimTypes.UnitLogo, logo);

                account.UpdatedAt = DateTime.UtcNow;
                await _store.Update(account);
            }
        }
        public async Task<AlternateAccountProfile> FindAlternateProfile(string guid)
        {
            Data.Account account = await _store.LoadByGuid(guid);
            return (account != null)
                ? Mapper.Map<AlternateAccountProfile>(account, opts => {
                    opts.Items["serviceUrl"] = _serviceUrl;
                    opts.Items["profileOptions"] = _options.Profile;
                })
                : new AlternateAccountProfile();
        }

        public async Task<Account[]> FindUpdated(string since, CancellationToken ct = default(CancellationToken))
        {
            if (!DateTime.TryParse(since, out DateTime target))
            {
                target = DateTime.MinValue;
            }
            var list = await _store.List()
                .Where(a => a.WhenCreated.CompareTo(target)>=0 || a.UpdatedAt.CompareTo(target)>=0)
                .ToArrayAsync(ct);

            return Mapper.Map<Account[]>(list, opts => {
                opts.Items["serviceUrl"] = _serviceUrl;
                opts.Items["profileOptions"] = _options.Profile;
            });
        }

        public async Task<TokenSummary> GetTokenSummary(TokenSummary model)
        {
            var account = await _store.LoadByGuid(model.GlobalId);
            if (account == null)
                throw new AccountNotFoundException();

            Mapper.Map(account, model);

            model.CurrentCertificateRegistered =
                model.CurrentCertificateSubject.HasValue() &&
                ! await IsCertificateUniqueAsync(model.CurrentCertificateSubject);

            model.ComplexityRegex = _options.Password.ComplexityExpression;
            model.ComplexityRequirement = _options.Password.ComplexityText;
            model.AllowMultipleCredentials = _options.Registration.AllowMultipleUsernames;
            model.AllowedDomains = _options.Registration.AllowedDomains;

            return model;
        }

        public async Task<AccountStats> GetStats(DateTime since)
        {
            return await _store.GetStats(since);
        }

        public async Task<Account[]> FindAll(SearchModel search, CancellationToken ct = default(CancellationToken))
        {
            // try direct values
            if (search.Term.HasValue())
            {
                Data.Account account = null;

                if (Guid.TryParse(search.Term, out Guid guid))
                {
                    account = await _store.LoadByGuid(search.Term);
                }
                else if (Int32.TryParse(search.Term, out int id) && id > 0)
                {
                    account = await _store.Load(id);
                }
                else if (search.Term.Length >= 10)
                {
                    account = await _store.LoadByToken(search.Term);
                }

                if (account != null)
                    return Mapper.Map<Account[]>(new Data.Account[] { account }, opts => {
                        opts.Items["serviceUrl"] = _serviceUrl;
                        opts.Items["profileOptions"] = _options.Profile;
                    });
            }

            // query all
            var query = _store.List();

            if (search.Term.HasValue())
            {
                var value = search.Term.ToLower().Trim();

                // if filters
                if (value.StartsWith("#"))
                {
                    switch (value)
                    {
                        case "#admins":
                        query = query.Where(a => a.Role != AccountRole.Member);
                        break;

                        case "#enabled":
                        query = query.Where(a => a.Status == AccountStatus.Enabled);
                        break;

                        case "#disabled":
                        query = query.Where(a => a.Status == AccountStatus.Disabled);
                        break;

                        default:
                        // query = Enumerable.Empty<Data.Account>().AsQueryable();
                        break;
                    }
                }
                else
                {
                    query = query.Where(a =>
                        a.GlobalId.StartsWith(value)
                        || a.Properties.Any(p => p.Value.ToLower().Contains(value))
                    );
                }
            }

            query = query.OrderByDescending(a => a.UpdatedAt);

            if (search.Skip > 0)
                query = query.Skip(search.Skip);

            if (search.Take > 0)
                query = query.Take(search.Take);

            var list = await query.ToArrayAsync(ct);

            return Mapper.Map<Account[]>(list, opts => {
                opts.Items["serviceUrl"] = _serviceUrl;
                opts.Items["profileOptions"] = _options.Profile;
            });
        }

        public bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            System.ComponentModel.DataAnnotations.EmailAddressAttribute attr = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            return attr.IsValid(email);
        }

        public bool IsPasswordComplex(string password)
        {
            return !_options.Password.ComplexityExpression.HasValue()
                || Regex.IsMatch(password, _options.Password.ComplexityExpression);
        }

        public bool IsDomainValid(string accountName)
        {
            if (!_options.Registration.AllowedDomains.HasValue())
                return true;

            var allowed = _options.Registration.AllowedDomains.Split(
                new char[] { ' ', ',', '\t', '|'},
                StringSplitOptions.RemoveEmptyEntries
            ).Select(x =>
                x.StartsWith(".")
                    ? x.Substring(1)
                    : x
            );

            var domain = accountName.Split("@").Last();

            while (!string.IsNullOrEmpty(domain))
            {
                if (allowed.Contains(domain))
                    return true;

                domain = string.Join(".", domain.Split(".").Skip(1).ToArray());
            }

            return false;
        }

        public bool IsExpired(DateTime dt)
        {
            return (_options.Password.ResetTokenExpirationMinutes > 0
                && DateTime.UtcNow.Subtract(dt).TotalMinutes > _options.Password.ResetTokenExpirationMinutes);
        }

        public async Task<bool> IsTokenUniqueAsync(string token)
        {
            return string.IsNullOrEmpty(token)
                ? false
                : await _store.IsTokenUnique(token);
        }

        public async Task<bool> IsCertificateUniqueAsync(string subjectDN)
        {
            var subject = new CertificateSubjectDetail(subjectDN);
            return await IsTokenUniqueAsync(subject.ExternalId);
        }

        public async Task<bool> IsCertificateUniqueAsync(X509Certificate2 cert)
        {
            _certStore.Validate(cert); // throws on error
            var subject = new CertificateSubjectDetail(cert.Subject);
            return await IsTokenUniqueAsync(subject.ExternalId);
        }

        public async Task<bool> HasAccounts()
        {
            return await _store.List().AnyAsync();
        }

        #endregion
    }
}
