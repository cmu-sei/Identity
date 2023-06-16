// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Extensions;
using Identity.Clients.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Clients.Services
{
    public class ClientService
    {
        private readonly IProfileService _profile;
        private readonly IClientStore _store;
        private readonly IResourceStore _resources;
        private readonly ILogger _logger;
        IMapper Mapper { get; }

        public ClientService(
            IClientStore store,
            IResourceStore resourceStore,
            IProfileService profile,
            ILogger<ClientService> logger,
            IMapper mapper
        ) {
            _logger = logger;
            _store = store;
            _profile = profile;
            _resources = resourceStore;
            Mapper = mapper;
        }

        public async Task<ClientSummary[]> Find(SearchModel search)
        {
            var query = _store.List(search.Term);


            if (search.Filter.Contains("published"))
            {
                query = query.Where(c => c.Flags.HasFlag(ClientFlag.Published));
            }
            else if (!_profile.IsPrivileged)
            {
                query = query.Where(c => c.Managers.Any(m => m.SubjectId == _profile.Id));
            }

            query = query.OrderBy(c => c.DisplayName);

            if (search.Skip > 0)
                query = query.Skip(search.Skip);

            if (search.Take > 0)
                query = query.Take(search.Take);

            var clients = await query.ToArrayAsync();

            return Mapper.Map<ClientSummary[]>(clients);
        }

        public async Task<Client> Load(int id)
        {
            var entity = await _store.Load(id);
            if (entity == null)
                throw new InvalidOperationException();

            if (!CanManage(entity))
                throw new InvalidOperationException();

            return Mapper.Map<Client>(entity);
        }

        public async Task<Client> Load(string id)
        {
            var entity = await _store.Load(id);
            if (entity == null)
                throw new InvalidOperationException();

            var model = Mapper.Map<Client>(entity);

            return model;
        }

        public async Task<ClientDetail> LoadDetail(string id)
        {
            var entity = await _store.Load(id);
            if (entity == null)
                throw new InvalidOperationException();

            var model = Mapper.Map<ClientDetail>(entity);

            return model;
        }

        public async Task<string[]> LoadUris()
        {
            // TODO: consider caching
            var urls = await _store.List()
                .Where(c => c.Enabled)
                .SelectMany(c => c.Urls)
                .Select(u => u.Value)
                .Distinct()
                .ToArrayAsync();

            return urls;
        }

        public async Task<bool> IsValidClientUrl(string returnUrl)
        {
            if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out Uri uri))
                return false;

            string target = $"{uri.Scheme}://{uri.Host}";

            var validClientUrls = (await LoadUris())
                .Where(u => Uri.IsWellFormedUriString(u, UriKind.Absolute))
                .Select(u => new Uri(u))
                .Select(u => $"{u.Scheme}://{u.Host}")
                .Distinct()
                .ToArray();

            return validClientUrls.Contains(target);
        }

        public async Task<ClientSummary> Add(NewClient model)
        {
            int rand = new Random().Next();

            var entity = new Data.Client();
            entity.Name = model.Name ?? $"new-client-{rand.ToString("x")}";
            entity.DisplayName = model.DisplayName ?? entity.Name;
            entity.Grants = "client_credentials";

            entity.Enabled = _profile.IsPrivileged;

            if (!_profile.IsPrivileged)
                entity.Managers.Add(new Data.ClientManager { SubjectId = _profile.Id, Name = _profile.Name });

            await _store.Add(entity);

            return Mapper.Map<ClientSummary>(entity);
        }

        public async Task<Client> Update(Client model)
        {
            var entity = await _store.Load(model.Id);
            if (!CanManage(entity))
                throw new InvalidOperationException();

            // strings
            entity.Name = model.Name.Trim();
            entity.DisplayName = model.DisplayName?.Trim();
            entity.Description = model.Description?.Trim();
            entity.ClientClaimsPrefix = model.ClientClaimsPrefix?.Trim() ?? "client_";

            // simple-timespans
            entity.ConsentLifetime = model.ConsentLifetime.ToSeconds();
            entity.IdentityTokenLifetime = model.IdentityTokenLifetime.ToSeconds();
            entity.AccessTokenLifetime = model.AccessTokenLifetime.ToSeconds();
            entity.AuthorizationCodeLifetime = model.AuthorizationCodeLifetime.ToSeconds();
            entity.AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime.ToSeconds();
            entity.SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime.ToSeconds();

            // flags
            entity.Flags = ClientFlag.EnableLocalLogin;
            if (model.RequirePkce) { entity.Flags |= ClientFlag.RequirePkce; }
            if (model.RequireConsent) { entity.Flags |= ClientFlag.RequireConsent; }
            if (model.AlwaysIncludeUserClaimsInIdToken) { entity.Flags |= ClientFlag.AlwaysIncludeUserClaimsInIdToken; }
            if (model.AllowOfflineAccess) { entity.Flags |= ClientFlag.AllowOfflineAccess; }
            if (model.UpdateAccessTokenClaimsOnRefresh) { entity.Flags |= ClientFlag.UpdateAccessTokenClaimsOnRefresh; }
            if (model.UseOneTimeRefreshTokens) { entity.Flags |= ClientFlag.UseOneTimeRefreshTokens; }
            if (model.Published) { entity.Flags |= ClientFlag.Published; }
            if (model.AlwaysSendClientClaims) { entity.Flags |= ClientFlag.AlwaysSendClientClaims; }

            // urls
            UpdateUrl(entity, ClientUriType.ClientUri, model.Url?.Trim());
            UpdateUrl(entity, ClientUriType.LogoUri, model.LogoUrl?.Trim());
            UpdateUrl(entity, ClientUriType.FrontChannelLogoutUri, model.FrontChannelLogoutUrl?.Trim());
            UpdateUrl(entity, ClientUriType.BackChannelLogoutUri, model.BackChannelLogoutUrl?.Trim());
            UpdateUrls(entity, ClientUriType.RedirectUri, model.RedirectUrls);
            UpdateUrls(entity, ClientUriType.PostLogoutRedirectUri, model.PostLogoutUrls);
            UpdateUrls(entity, ClientUriType.CorsUri, model.CorsUrls);

            // client claims
            UpdateClaims(entity, model.Claims);

            // secrets
            UpdateSecrets(entity, model.Secrets);

            // managers
            UpdateManagers(entity, model.Managers);

            // validate scopes
            entity.Grants = model.Grants;
            if (!entity.Grants.Contains("client_credentials"))
                entity.Grants += " client_credentials";

            if (entity.Grants.Contains("implicit"))
            {
                entity.Flags |= ClientFlag.AllowAccessTokensViaBrowser;
                entity.Flags |= ClientFlag.AllowOfflineAccess;
                entity.Flags ^= ClientFlag.AllowOfflineAccess;
            }

            await ValidateScopes(entity, model.Scopes?.Trim());

            // only admins can enable
            if (_profile.IsPrivileged)
                entity.Enabled = model.Enabled;

            try
            {
                await _store.Update(entity);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message.ToLower().Contains("unique"))
                    throw new ClientNameNotUniqueException();
                else
                    throw new ClientUpdateException();
            }

            return Mapper.Map<Client>(entity);
        }

        private async Task ValidateScopes(Data.Client entity, string scopes)
        {
            if (entity.Scopes == scopes)
                return;
            if (String.IsNullOrEmpty(scopes))
            {
                entity.Scopes = scopes;
                return;
            }

            var resources = await _resources.GetAll();
            var validscopes = new List<string>();

            foreach (string name in scopes.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            {
                var resource = resources.SingleOrDefault(r => r.Scopes.Split(' ').Contains(name.Replace("*", ""))); //TODO: scope now more than name

                if (resource != null &&
                    (resource.Default || _profile.IsPrivileged || resource.Managers.Any(m => m.SubjectId == _profile.Id))
                )
                {
                    validscopes.Add(name);
                }
            }

            entity.Scopes = string.Join(" ", validscopes);
        }

        private void UpdateUrl(Data.Client entity, ClientUriType uriType, string value)
        {
            var target = entity.Urls.FirstOrDefault(u => u.Type == uriType);
            if (target != null)
            {
                if (string.IsNullOrEmpty(value))
                    entity.Urls.Remove(target);
                else
                    target.Value = value;
            }
            else
            {
                entity.Urls.Add(new Data.ClientUri {
                    Type = uriType,
                    Value = value
                });
            }
        }

        private void UpdateUrls(Data.Client entity, ClientUriType uriType, IEnumerable<ClientUri> urls)
        {
            foreach (var url in urls)
            {
                url.Value = url.Value.Trim();
                if (url.Id > 0)
                {
                    var target = entity.Urls.SingleOrDefault(u => u.Id == url.Id);
                    if (target != null)
                    {
                        if (string.IsNullOrEmpty(url.Value) || url.Deleted)
                            entity.Urls.Remove(target);
                        else
                            target.Value = url.Value;
                    }
                }
                else
                {
                    entity.Urls.Add(new Data.ClientUri {
                        Type = uriType,
                        Value = url.Value,
                    });
                }
            }
        }

        private void UpdateClaims(Data.Client entity, IEnumerable<ClientClaim> claims)
        {
            foreach (var claim in claims)
            {
                claim.Type = claim.Type.Trim();
                claim.Value = claim.Value.Trim();

                if (claim.Id > 0)
                {
                    var target = entity.Claims.SingleOrDefault(u => u.Id == claim.Id);
                    if (target != null)
                    {
                        if (string.IsNullOrEmpty(claim.Type) || string.IsNullOrEmpty(claim.Value) || claim.Deleted)
                            entity.Claims.Remove(target);
                        else
                        {
                            target.Value = claim.Value;
                            target.Type = claim.Type;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(claim.Type) && !string.IsNullOrEmpty(claim.Value))
                {
                    entity.Claims.Add(new Data.ClientClaim
                    {
                        Type = claim.Type,
                        Value = claim.Value
                    });
                }
            }
        }

        private void UpdateSecrets(Data.Client entity, IEnumerable<ClientSecret> secrets)
        {
            foreach (var secret in secrets.Where(s => s.Deleted))
            {
                var target = entity.Secrets.SingleOrDefault(s => s.Id == secret.Id);
                if (target != null)
                    entity.Secrets.Remove(target);
            }
        }

        private void UpdateManagers(Data.Client entity, IEnumerable<ClientManager> managers)
        {
            foreach (var manager in managers.Where(s => s.Deleted))
            {
                var target = entity.Managers.SingleOrDefault(s => s.Id == manager.Id);
                if (target != null)
                    entity.Managers.Remove(target);
            }
        }

        public async Task Delete(int id)
        {
            if (! await CanManage(id))
                throw new InvalidOperationException();

            await _store.Delete(id);
        }

        private bool CanManage(Data.Client client)
        {
            return client != null && (_profile.IsPrivileged
            || client.GlobalId == _profile.Id
            || client.Managers.Any(m => m.SubjectId == _profile.Id));
        }

        private async Task<bool> CanManage(int id)
        {
            return _profile.IsPrivileged || await _store.CanManage(id, _profile.Id);
        }

        public async Task<ClientSecret> AddSecret(int clientId)
        {
            var entity = await _store.Load(clientId);
            if (!await CanManage(clientId))
                throw new InvalidOperationException();

            string val = Guid.NewGuid().ToString("N");
            entity.Secrets.Add(new Data.ClientSecret
            {
                ClientId = clientId,
                Type = SecretTypes.SharedSecret,
                Value = val.Sha256(),
                Description = $"Added by {_profile.Name} at {DateTime.UtcNow}"
            });

            await _store.Update(entity);

            return new ClientSecret
            {
                Id = entity.Secrets.Last().Id,
                // Description = secret.Description,
                Value = val
            };
        }

        public async Task<string> NewEnlistCode(int id)
        {
            var entity = await _store.Load(id);
            if (!CanManage(entity))
                throw new InvalidOperationException();

            string code = Guid.NewGuid().ToString("N");
            entity.EnlistCode = code;
            await _store.Update(entity);
            return code;
        }

        public async Task Enlist(string code)
        {
            var client = await _store.LoadByEnlistCode(code);

            if (client == null)
                return;

            var entity = client.Managers.Where(m => m.SubjectId == _profile.Id).SingleOrDefault();
            if (entity == null)
            {
                client.Managers.Add(new Data.ClientManager
                {
                    SubjectId = _profile.Id,
                    Name = _profile.Name,
                    ClientId = client.Id
                });
                await _store.Update(client);
            };
        }

    }
}
