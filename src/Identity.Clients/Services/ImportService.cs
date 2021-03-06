// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Extensions;
using Identity.Clients.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Clients.Services
{
    public class ImportService
    {
        private readonly IProfileService _profile;
        private readonly IClientStore _clients;
        private readonly IResourceStore _resources;
        private readonly ILogger _logger;

        public ImportService(
            IClientStore clientStore,
            IResourceStore resourceStore,
            IProfileService profile,
            ILogger<ClientService> logger
        )
        {
            _logger = logger;
            _profile = profile;
            _clients = clientStore;
            _resources = resourceStore;
        }

        public async Task Import(ResourceImport model)
        {
            var resources = await _resources.GetAll();

            var existing = resources
                .Where(r => r.Type == ResourceType.Api)
                .ToList();

            foreach (var api in model.Apis)
            {
                var current = existing.SingleOrDefault(r => r.Name == api.Name);
                if (current != null)
                    await _resources.Delete(current.Id);

                await _resources.Add(new Data.Resource{
                    Type = ResourceType.Api,
                    Name = api.Name,
                    Enabled = true,
                    Scopes = api.Scopes ?? api.Name,
                    UserClaims = api.UserClaims,
                    Secrets = new Data.ApiSecret[]
                    {
                        new Data.ApiSecret
                        {
                            Type = "SharedSecret",
                            Value = api.Name.Sha256(),
                            Description = "Added by Dev at " + DateTime.UtcNow.ToString("u")
                        }
                    }
                });

            }

            foreach (var client in model.Clients)
                await ImportClient(client, resources);
        }

        private async Task ImportClient(ClientImport client, Data.Resource[] resources)
        {
            string guid = Guid.NewGuid().ToString();
            var clients = await _clients.List().ToArrayAsync();
            var entity = clients.Where(c => c.Name == client.Id).SingleOrDefault();
            if (entity != null)
            {
                guid = entity.GlobalId;
                await _clients.Delete(entity.Id);
            }

            entity = new Data.Client
            {
                Name = client.Id,
                GlobalId = guid,
                Enabled = true,
                DisplayName = client.DisplayName ?? client.Id,
                Grants = client.GrantType,
                Scopes = client.Scopes,
                Flags = ClientFlag.Published | ClientFlag.EnableLocalLogin | ClientFlag.AllowOfflineAccess | ClientFlag.AllowRememberConsent
            };

            if ("implicit hybrid".Contains(client.GrantType))
                entity.Flags |= ClientFlag.AllowAccessTokensViaBrowser;

            var urls = new List<Data.ClientUri>();
            foreach (string u in client.RedirectUrl)
            {
                urls.Add(new Data.ClientUri
                {
                    Type = ClientUriType.RedirectUri,
                    Value = u
                });
            }
            entity.Urls = urls.ToArray();

            if (client.Secret.HasValue())
            {
                entity.Secrets.Add(
                    new Data.ClientSecret
                    {
                        Type = "SharedSecret",
                        Value = client.Secret.Sha256(),
                        Description = "Added by Admin at " + DateTime.UtcNow.ToString("u")
                    }
                );
            }

            await _clients.Add(entity);
        }
    }
}
