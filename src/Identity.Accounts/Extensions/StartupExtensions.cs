// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Reflection;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.Abstractions;
using Identity.Accounts.Data.EntityFrameworkCore;
using Identity.Accounts.Mappers;
using Identity.Accounts.Options;
using Identity.Accounts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IIdentityAccountsServiceBuilder
    {
        IServiceCollection Services { get; }
    }
    public class IdentityAccountsServiceBuilder : IIdentityAccountsServiceBuilder
    {
        public IdentityAccountsServiceBuilder(
            IServiceCollection services
        )
        {
            Services = services;
        }
        public IServiceCollection Services { get; private set; }
    }

    public static class IdentityAccountsExtensions
    {
        public static IIdentityAccountsServiceBuilder AddIdentityAccounts(
            this IServiceCollection services
        ){
            services
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IPropertyService, PropertyService>()
                .AddScoped<IOverrideService, OverrideService>()
                .AddScoped<IProfileService, DefaultProfileService>()
                .AddSingleton<IIssuerService, DefaultIssuerService>();

            return new IdentityAccountsServiceBuilder(services);
        }

        public static IIdentityAccountsServiceBuilder WithConfiguration(
            this IIdentityAccountsServiceBuilder builder,
            Func<IConfigurationSection> accountConfigurationSection,
            string contentRoot
        ){
            var config = accountConfigurationSection();
            var opt = config.Get<AccountOptions>() ?? new AccountOptions();

            builder.Services.AddOptions()
                .Configure<AccountOptions>(config)
                .Configure<CertValidationOptions>(config.GetSection("CertValidation"))
                .AddScoped(sp => sp.GetService<IOptionsMonitor<AccountOptions>>().CurrentValue)
                .AddSingleton<CertValidationOptions>(sp => sp.GetService<IOptions<CertValidationOptions>>().Value)
                .AddSingleton<EnvironmentOptions>(sp => new EnvironmentOptions {
                    ContentRoot = System.IO.Path.Combine(
                        contentRoot,
                        opt.CertValidation.IssuerCertificatesPath ?? "certs"
                    )
                });

            return builder;
        }

        public static IIdentityAccountsServiceBuilder WithDefaultStores(
            this IIdentityAccountsServiceBuilder builder,
            string provider,
            string connstr,
            string migrationAssembly
        )
        {

            switch (provider.ToLower())
            {

                case "sqlserver":
                // builder.Services.AddEntityFrameworkSqlServer();
                builder.Services.AddDbContext<AccountDbContext, AccountDbContextSqlServer>(
                    db => db.UseSqlServer(connstr, options => options.MigrationsAssembly(migrationAssembly))
                );
                break;

                case "postgresql":
                // builder.Services.AddEntityFrameworkNpgsql();
                builder.Services.AddDbContext<AccountDbContext, AccountDbContextPostgreSQL>(
                    db => db.UseNpgsql(connstr, options => options.MigrationsAssembly(migrationAssembly))
                );
                break;

                default:
                // builder.Services.AddEntityFrameworkInMemoryDatabase();
                builder.Services.AddDbContext<AccountDbContext, AccountDbContextInMemory>(
                    db => db.UseInMemoryDatabase(connstr)
                );
                break;
            }

            builder.Services
                .AddScoped<IAccountStore, AccountStore>()
                .AddScoped<IPropertyStore, PropertyStore>()
                .AddScoped<IOverrideStore, OverrideStore>();
            return builder;
        }

        public static IIdentityAccountsServiceBuilder WithStore(
            this IIdentityAccountsServiceBuilder builder,
            Action<IServiceCollection> addRepository
        )
        {
            addRepository(builder.Services);
            return builder;
        }

        public static IIdentityAccountsServiceBuilder WithIssuerService(
            this IIdentityAccountsServiceBuilder builder,
            Action<IServiceCollection> addIssuerService
        )
        {
            addIssuerService(builder.Services);
            return builder;
        }

        public static IIdentityAccountsServiceBuilder WithTokenService(
            this IIdentityAccountsServiceBuilder builder,
            Action<IServiceCollection> addTokenService
        )
        {
            addTokenService(builder.Services);
            return builder;
        }

        public static IIdentityAccountsServiceBuilder WithProfileService(
            this IIdentityAccountsServiceBuilder builder,
            Action<IServiceCollection> addProfileService
        )
        {
            addProfileService(builder.Services);
            return builder;
        }

        public static AutoMapper.IMapperConfigurationExpression AddAccountMaps(this AutoMapper.IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<AccountMapper>();
            return cfg;
        }
    }
}
