// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data.EntityFrameworkCore;
using Identity.Clients.Mappers;
using Identity.Clients.Services;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IIdentityClientsServiceBuilder
    {
        IServiceCollection Services { get; }
    }
    public class IdentityClientsServiceBuilder : IIdentityClientsServiceBuilder
    {
        public IdentityClientsServiceBuilder(
            IServiceCollection services
        )
        {
            Services = services;
        }
        public IServiceCollection Services { get; private set; }
    }

    public static class IdentityClientsExtensions
    {
        public static IIdentityClientsServiceBuilder AddIdentityClients(
            this IServiceCollection services
        ){
            services
                .AddScoped<ClientService>()
                .AddScoped<ResourceService>()
                .AddScoped<GrantService>()
                .AddScoped<ImportService>();


            return new IdentityClientsServiceBuilder(services);
        }

        public static IIdentityClientsServiceBuilder WithDefaultStores(
            this IIdentityClientsServiceBuilder builder,
            string provider,
            string connstr,
            string migrationAssembly
        )
        {

            switch (provider.ToLower())
            {

                case "sqlserver":
                // builder.Services.AddEntityFrameworkSqlServer();
                builder.Services.AddDbContext<ClientDbContext, ClientDbContextSqlServer>(
                    db => db.UseSqlServer(connstr, options => options.MigrationsAssembly(migrationAssembly))
                );
                break;

                case "postgresql":
                // builder.Services.AddEntityFrameworkNpgsql();
                builder.Services.AddDbContext<ClientDbContext, ClientDbContextPostgreSQL>(
                    db => db.UseNpgsql(connstr, options => options.MigrationsAssembly(migrationAssembly))
                );
                break;

                default:
                // builder.Services.AddEntityFrameworkInMemoryDatabase();
                builder.Services.AddDbContext<ClientDbContext, ClientDbContextInMemory>(
                    db => db.UseInMemoryDatabase(connstr)
                );
                break;
            }

            builder.Services
                .AddScoped<IClientStore, ClientStore>()
                .AddScoped<IEventStore, EventStore>()
                .AddScoped<IManagerStore, ManagerStore>()
                .AddScoped<IResourceStore, ResourceStore>()
                .AddScoped<ISecretStore, SecretStore>()
                .AddScoped<IUriStore, UriStore>()
                // .AddScoped<IWebhookStore, WebhookStore>()
                .AddScoped<IGrantStore, GrantStore>()
                .AddScoped<IClientClaimStore, ClientClaimStore>();

            return builder;
        }

        public static IIdentityClientsServiceBuilder WithStore(
            this IIdentityClientsServiceBuilder builder,
            Action<IServiceCollection> addRepository
        )
        {
            addRepository(builder.Services);
            return builder;
        }

        public static IIdentityClientsServiceBuilder WithProfileService(
            this IIdentityClientsServiceBuilder builder,
            Action<IServiceCollection> addProfileService
        )
        {
            addProfileService(builder.Services);
            return builder;
        }

        public static IIdentityClientsServiceBuilder WithDispatchService(
            this IIdentityClientsServiceBuilder builder,
            Action<IServiceCollection> addDispatchService
        )
        {
            addDispatchService(builder.Services);
            return builder;
        }

        public static AutoMapper.IMapperConfigurationExpression AddClientMaps(this AutoMapper.IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<ClientMapper>();
            cfg.AddProfile<ResourceMapper>();
            return cfg;
        }
    }
}
