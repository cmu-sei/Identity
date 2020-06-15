// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.EntityFrameworkCore;
using Identity.Accounts.Options;
using Identity.Clients.Abstractions;
using Identity.Clients.Data;
using Identity.Clients.Data.EntityFrameworkCore;
using IdentityServer.Models;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer.Extensions
{
    public static class DatabaseExtensions
    {
        public static IHost InitializeDatabase(this IHost appHost)
        {
            using (var scope = appHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger("DatabaseInitialization");
                IConfiguration config = services.GetRequiredService<IConfiguration>();
                IHostEnvironment env = services.GetService<IHostEnvironment>();
                AccountDbContext accountDb =  services.GetRequiredService<AccountDbContext>();
                ClientDbContext clientDb = services.GetRequiredService<ClientDbContext>();
                AccountOptions accountOptions = services.GetService<IOptionsSnapshot<AccountOptions>>().Value;
                var secretOptions = config.GetSection("SeedData").Get<DbSeedKVP[]>() ?? new DbSeedKVP[] { };

                if (!accountDb.Database.IsInMemory())
                    accountDb.Database.Migrate();

                if (!clientDb.Database.IsInMemory())
                    clientDb.Database.Migrate();

                // manual migration
                foreach (var client in clientDb.Clients
                    .Include(c => c.Resources).ThenInclude(r => r.Resource)
                    .ToArray())
                    {
                        if (string.IsNullOrEmpty(client.Grants))
                        client.Grants = string.Join(" ", client.Resources
                            .Where(r => r.Resource.Type == ResourceType.Grant)
                            .Select(r => r.Resource.Name)
                            .ToArray());

                        if (string.IsNullOrEmpty(client.Scopes))
                        client.Scopes = string.Join(" ", client.Resources
                            .Where(r => r.Resource.Type != ResourceType.Grant)
                            .Select(r => r.Resource.Name)
                            .ToArray());
                    }
                clientDb.SaveChanges();

                var seedGrants = new IdentityResource[] {
                    new IdentityResource { Name = "client_credentials", DisplayName = "Client Credentials", Enabled = true },
                    new IdentityResource { Name = "authorization_code", DisplayName = "Code Grant", Enabled = true },
                    new IdentityResource { Name = "hybrid", DisplayName = "Hybrid Grant", Enabled = true },
                    new IdentityResource { Name = "implicit", DisplayName = "Implicit Grant", Enabled = true },
                    new IdentityResource { Name = "password", DisplayName = "Resource Owner Grant", Enabled = true }
                }.ToList();

                var seedScopes = new IdentityResource[] {
                    new IdentityResources.OpenId(){ DisplayName = "User Id"},
                    new IdentityResources.Email(){ DisplayName = "Email Address"},
                    new IdentityResource {
                        Name= "profile",
                        DisplayName = "Profile Info",
                        Enabled = true,
                        UserClaims = new string[] { "name", "family_name", "given_name", "username", "picture", "updated_at", "affiliate" }
                    },
                    new IdentityResource {
                        Name= "organization",
                        DisplayName = "Organization",
                        Enabled = true,
                        UserClaims = new string[] { "org", "orgunit", "picture_o", "picture_ou" }
                    },
                    new IdentityResource {
                        Name= "role",
                        DisplayName = "Role",
                        Enabled = true,
                        UserClaims = new string[] { "role" }
                    }
                }.ToList();

                var seedApi = new ApiResource[] {
                    new ApiResource { Name = "identity-api" },
                    new ApiResource { Name = "identity-api-privileged" },
                }.ToList();

                var seedClients = new List<DbSeedClient>(); //[] {
                    // new DbSeedClient {
                    //     Name = "IdentityApiSwaggerClient",
                    //     DisplayName = "Identity OpenApi",
                    //     GlobalId = Guid.NewGuid().ToString(),
                    //     Flags = ClientFlag.AllowRememberConsent | ClientFlag.EnableLocalLogin,
                    //     SeedGrant = "implicit",
                    //     SeedScopes = "openid identity-api",
                    //     Enabled = true,
                    //     Urls = new ClientUri[] {
                    //         new ClientUri { Type = ClientUriType.RedirectUri, Value = "http://localhost:5000/api/oauth2-redirect.html"}
                    //     }
                    // }
                // }.ToList();

                var seedUsers = new List<DbSeedUser>();

                if (
                    !string.IsNullOrEmpty(accountOptions.OverrideCode)
                    && !accountDb.OverrideCodes
                        .Where(o => o.Code == accountOptions.OverrideCode)
                        .Any()
                ){
                    accountDb.OverrideCodes.Add(
                        new Identity.Accounts.Data.OverrideCode{
                            Code = accountOptions.OverrideCode,
                            Description = "Applied at Startup",
                            WhenCreated = DateTime.UtcNow
                        }
                    );
                    accountDb.SaveChanges();
                }

                if (!string.IsNullOrEmpty(accountOptions.AdminEmail))
                {
                    seedUsers.Add(new DbSeedUser {
                        Username = accountOptions.AdminEmail,
                        GlobalId = accountOptions.AdminGuid,
                        Password = accountOptions.AdminPassword
                    });
                }

                string seedFile = Path.Combine(
                    env.ContentRootPath,
                    config.GetValue<string>("Database:SeedFile", "seed-data.json")
                );
                if (File.Exists(seedFile)) {

                    DbSeedModel seedData = JsonConvert.DeserializeObject<DbSeedModel>(File.ReadAllText(seedFile));
                    seedGrants.AddRange(seedData.GrantResources);
                    seedScopes.AddRange(seedData.IdentityResources);
                    seedApi.AddRange(seedData.ApiResources);
                    seedClients.AddRange(seedData.Clients);
                    seedUsers.AddRange(seedData.Users);
                }

                if (seedUsers.Count() > 0)
                {
                    IAccountService accountSvc = services.GetRequiredService<IAccountService>();

                    foreach (DbSeedUser seedUser in seedUsers)
                    {
                        try
                        {
                            // override seed-data with config data for password
                            string secret = secretOptions
                                .Where(i => i.Key == seedUser.Username || i.Key == seedUser.GlobalId)
                                .FirstOrDefault()?.Value ?? seedUser.Password;

                            accountSvc.RegisterUsername(
                                seedUser.Username,
                                seedUser.Password,
                                seedUser.GlobalId
                            ).Wait();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Failed to register {seedUser.Username}");
                        }
                    }
                }

                foreach (var resource in seedGrants)
                {
                    if (!clientDb.Resources.Any(r =>
                        r.Type == ResourceType.Grant
                        && r.Name == resource.Name
                    ))
                    {
                        var entity = new Identity.Clients.Data.Resource
                        {
                            Type = ResourceType.Grant,
                            Name = resource.Name,
                            DisplayName = resource.DisplayName,
                            Description = resource.Description
                        };
                        clientDb.Resources.Add(entity);
                        entity.Claims.Add(new ResourceClaim { Type = resource.Name });
                        entity.Claims.Add(new ResourceClaim { Type = "client_credentials" });
                    }
                }

                foreach (IdentityServer4.Models.ApiResource resource in seedApi)
                {
                    if (!clientDb.Resources.Any(r =>
                        r.Type == ResourceType.Api
                        && r.Name == resource.Name
                    ))
                    {
                        var entity = new Identity.Clients.Data.Resource
                        {
                            Type = ResourceType.Api,
                            Name = resource.Name,
                            DisplayName = resource.DisplayName ?? resource.Name,
                            Description = resource.Description,
                            Enabled = true,
                        };

                        clientDb.Resources.Add(entity);

                        if (resource.Scopes.Count == 0)
                            resource.Scopes.Add(new Scope(resource.Name, resource.DisplayName));

                        foreach (var s in resource.Scopes)
                            entity.Claims.Add(new ResourceClaim { Type = s.Name });

                        clientDb.SaveChanges();
                    }
                }

                foreach (var resource in seedScopes)
                {
                    if (!clientDb.Resources.Any(r =>
                        r.Type == ResourceType.Identity
                        && r.Name == resource.Name
                    ))
                    {
                        var entity = new Identity.Clients.Data.Resource
                        {
                            Type = ResourceType.Identity,
                            Name = resource.Name,
                            DisplayName = resource.DisplayName ?? resource.Name,
                            Description = resource.Description,
                            Enabled = resource.Enabled,
                            Emphasize = resource.Emphasize,
                            Required = resource.Required,
                            Default = "openid profile organization".Contains(resource.Name)
                        };
                        clientDb.Resources.Add(entity);
                        foreach (var s in resource.UserClaims)
                            entity.Claims.Add(new ResourceClaim { Type = s });
                    }
                }

                clientDb.SaveChanges();

                foreach (var client in seedClients)
                {
                    if (!clientDb.Clients.Any(c => c.Name == client.Name))
                    {
                        if (Enum.TryParse<ClientFlag>(client.SeedFlags, out ClientFlag flags))
                            client.Flags = flags;

                        client.GlobalId = Guid.NewGuid().ToString("N");
                        client.EnlistCode = Guid.NewGuid().ToString("N");

                        //combine any secrets from config and seed-data (as secrets may be provided separately in the config)
                        var clientSecrets =
                            secretOptions.Where(i => i.Key == client.Name)
                            .Select(s => s.Value)
                            .Union(new string[] { client.SeedSecret })
                            .ToArray();

                        foreach (var secret in clientSecrets)
                            if (!String.IsNullOrEmpty(secret))
                                client.Secrets.Add(
                                    new ClientSecret
                                    {
                                        Type = "SharedSecret",
                                        Value = secret.Sha256(),
                                        Description = "Added by Admin at " + DateTime.UtcNow.ToString("u")
                                    }
                                );

                        foreach (var item in client.SeedHandlers)
                        {
                            string[] src = item.Key.Split("::");
                            if (src.Length > 1)
                            {
                                var srcClient = clientDb.Clients
                                    .Include(c => c.Events)
                                    .Where(c => c.Name == src[0])
                                    .FirstOrDefault();

                                if (srcClient != null)
                                {
                                    var srcEvent = srcClient.Events
                                        .Where(e => e.Type == src[1])
                                        .FirstOrDefault();

                                    if (srcEvent != null)
                                        client.EventHandlers.Add(
                                            new ClientEventHandler
                                            {
                                                ClientEventId = srcEvent.Id,
                                                Uri = item.Value,
                                                Enabled = true
                                            }
                                        );
                                }
                            }
                        }

                        client.Grants = client.SeedGrant;
                        client.Scopes = client.SeedScopes;

                        //convert derived entity to base entity
                        Identity.Clients.Data.Client entity = JsonConvert.DeserializeObject<Identity.Clients.Data.Client>(JsonConvert.SerializeObject(client));
                        clientDb.Clients.Add(entity);
                        clientDb.SaveChanges();
                    }
                }

            }
            return appHost;
        }
    }
}
