// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Identity.Accounts.Options;
using Identity.Clients.Abstractions;
using IdentityModel;
using IdentityServer.Extensions;
using IdentityServer.Options;
using IdentityServer.Services;
using IdentityServer4;
using IdentityServer4.Hosting.LocalApiAuthentication;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;

namespace IdentityServer
{
    public class Startup
    {
        IConfiguration Configuration { get; }
        private IHostEnvironment _env;
        private string _certificatePath = "";
        private string _certificatePass = "";
        private Options.AuthorizationOptions _authOptions = new Options.AuthorizationOptions();
        private Options.BrandingOptions _branding;
        private Options.CacheOptions _cacheOptions;
        private Options.DatabaseOptions _database;
        private Options.HeaderOptions _headers;
        private JAvatar.Options _javatar;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;

            _env = env;

            _authOptions = Configuration.GetSection("Authorization").Get<Options.AuthorizationOptions>() ?? new Options.AuthorizationOptions();

            _branding = Configuration.GetSection("Branding").Get<Options.BrandingOptions>() ?? new BrandingOptions();

            _headers = configuration.GetSection("Headers").Get<HeaderOptions>() ?? new HeaderOptions();

            _cacheOptions = Configuration.GetSection("Cache").Get<CacheOptions>() ?? new CacheOptions();
            _cacheOptions.SharedFolder = Path.Combine(env.ContentRootPath, _cacheOptions.SharedFolder ?? "");

            _database = Configuration.GetSection("Database").Get<DatabaseOptions>() ?? new DatabaseOptions();

            var accountOptions = Configuration.GetSection("Account").Get<AccountOptions>() ?? new AccountOptions();
            _certificatePath = Path.Combine(env.ContentRootPath, accountOptions.Authentication.SigningCertificate ?? "signer.pfx");
            _certificatePass = accountOptions.Authentication.SigningCertificatePassword;

            _javatar = Configuration.GetSection("JAvatar").Get<JAvatar.Options>() ?? new JAvatar.Options();
            _javatar.Folders = new JAvatar.ImageFolder[]
            {
                new JAvatar.ImageFolder { Name = accountOptions.Profile.AvatarPath, NameMode = JAvatar.FileNameMode.Subject },
                new JAvatar.ImageFolder { Name = accountOptions.Profile.OrgLogoPath, NameMode = JAvatar.FileNameMode.SubjectAppend, Browseable = true },
                new JAvatar.ImageFolder { Name = accountOptions.Profile.UnitLogoPath, NameMode = JAvatar.FileNameMode.SubjectAppend, Browseable = true }
            };

            // TODO: move this up into javatar library
            foreach (var folder in _javatar.Folders)
                Directory.CreateDirectory(Path.Combine("wwwroot/javatar", folder.Name));

            string png = Path.Combine("wwwroot/javatar", accountOptions.Profile.OrgLogoPath, "default.png");
            if (!File.Exists(png))
                File.Copy("wwwroot/javatar-o-default.png", png);

            png = Path.Combine("wwwroot/javatar", accountOptions.Profile.UnitLogoPath, "default.png");
            if (!File.Exists(png))
                File.Copy("wwwroot/javatar-u-default.png", png);

            png = Path.Combine("wwwroot/javatar", accountOptions.Profile.AvatarPath, "default.png");
            if (!File.Exists(png) && accountOptions.Profile.UseDefaultAvatar)
                File.Copy("wwwroot/javatar-p-default.png", png);


            // By default, Microsoft has some legacy claim mapping that converts
            // standard JWT claims into proprietary ones. This removes those mappings.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddFeatureFolder()
#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                });

            #region Options

            services.AddSingleton<Options.BrandingOptions>(_branding);
            services.AddSingleton<Options.AuthorizationOptions>(_authOptions);
            services.AddSingleton<Options.SecurityHeaderOptions>(_headers.Security);

            #endregion

            services.AddRouting(options => options.LowercaseUrls = true);

            // use lax cookies in dev so secure isn't required
            if (_env.IsDevelopment())
                services.ConfigureLaxCookies();

            services.ConfigureForwarding(_headers.Forwarding);

            services.AddCors(
                opt => opt.AddPolicy(
                    _headers.Cors.Name,
                    _headers.Cors.Build()
                )
            );

            if (_branding.IncludeSwagger)
                services.AddSwagger(_authOptions, _branding);

            services.AddCache(() => _cacheOptions);

            services.AddDataProtection()
                .SetApplicationName(Assembly.GetEntryAssembly().GetName().Name)
                .PersistKeys(() => _cacheOptions);

            services.AddAppMailClient(
                () => Configuration.GetSection("AppMail")
                    .Get<AppMailClient.Options>()
            );

            services.AddJAvatar(() => _javatar);

            #region IdentityServer

            services.AddIdentityServer(options =>
            {
                if (_env.IsDevelopment())
                {
                    options.IssuerUri = "localhost";
                }

                if (_authOptions.CookieLifetimeMinutes > 0)
                    options.Authentication.CookieLifetime = new TimeSpan(0, _authOptions.CookieLifetimeMinutes, 0);

                options.Authentication.CookieSlidingExpiration = _authOptions.CookieSlidingExpiration;
            })
                .AddConfiguredSigningCredential(_certificatePath, _certificatePass)
                .AddClientStore<IdsrvClientStore>()
                .AddResourceStore<IdsrvResourceStore>()
                .AddProfileService<IdsrvProfileService>();

            services.AddScoped<IResourceOwnerPasswordValidator, IdsrvPasswordValidator>();
            services.AddScoped<IdentityServer4.Stores.IPersistedGrantStore, IdsrvGrantStore>();
            services.AddScoped<Services.CookieService>();
            services.AddOidcStateDataFormatterCache();

            services.AddIdentityAccounts()
                .WithDefaultStores(
                    _database.Provider,
                    _database.ConnectionString,
                    Assembly.GetExecutingAssembly().GetName().Name
                )
                .WithConfiguration(
                    () => Configuration.GetSection("Account"),
                    _env.ContentRootPath
                );

            services.AddIdentityClients()
                .WithDefaultStores(
                    _database.Provider,
                    _database.ConnectionString,
                    Assembly.GetExecutingAssembly().GetName().Name
                )
                .WithProfileService(
                    svc => svc.AddScoped<Identity.Clients.Abstractions.IProfileService, ProfileService>()
                )
                .WithDispatchService(
                    svc => {
                        svc.AddHttpClient<IEventDispatcher,EventDispatcher>()
                            .ConfigureHttpClient(opt => {
                            })
                            .AddPolicyHandler(
                                HttpPolicyExtensions.HandleTransientHttpError()
                                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                .WaitAndRetryAsync(3,
                                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                )
                            );
                    }
                );


            services.AddSingleton<AutoMapper.IMapper>(
                new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.AddAccountMaps();
                    cfg.AddClientMaps();
                }).CreateMapper()
            );

            #endregion

            #region Authentication

            var authBuilder = services.AddAuthentication()
            .AddLocalApi(config =>
            {
                config.ExpectedScope = AppConstants.Audience;

                config.Events = new LocalApiAuthenticationEvents
                {
                    OnClaimsTransformation = (ctx) =>
                    {
                        var identity = ctx.Principal.Identities.First();

                        if (identity.FindAll(JwtClaimTypes.Scope)
                            .Where(c => c.Value.Equals(AppConstants.PrivilegedAudience))
                            .Any()
                        )
                        {
                            identity.AddClaim(new Claim(JwtClaimTypes.Role, AppConstants.ManagerRole));
                        }


                        return Task.FromResult(0);
                    }
                };
            });

            foreach (var oidc in _authOptions.ExternalOidc)
            {
                authBuilder.AddOpenIdConnect(oidc.Scheme, config =>
                {
                    config.Authority = oidc.Authority;
                    config.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    config.ClientId = oidc.ClientId;
                    config.DisableTelemetry = true;
                    foreach (string scope in oidc.Scopes.Split(" "))
                    {
                        config.Scope.Add(scope);
                    }
                });
            }
            #endregion

            #region Authorization

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppConstants.PrivilegedPolicy, builder =>
                {
                    builder.AddAuthenticationSchemes(
                        IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme,
                        IdentityServer4.IdentityServerConstants.LocalApi.AuthenticationScheme
                    );
                    builder.RequireRole(
                        AppConstants.ManagerRole,
                        AppConstants.AdminRole
                    );
                });

                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    IdentityServer4.IdentityServerConstants.LocalApi.AuthenticationScheme,
                    IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme
                ).RequireAuthenticatedUser().Build();
            });

            #endregion

        }

        public void Configure(IApplicationBuilder app)
        {

            app.UseExceptionHandler("/Home/Error");

            app.UseJsonExceptions();

            app.UseCookiePolicy();

            if (_headers.LogHeaders)
                app.UseHeaderInspection();

            if (!string.IsNullOrEmpty(_headers.Forwarding.TargetHeaders))
                app.UseForwardedHeaders();

            if (_headers.UseHsts)
                app.UseHsts();

            app.UsePathBase(_branding.PathBase);

            app.UseRewriter(
                new RewriteOptions()
                    .AddRewrite(@"^oauth/(.*)", "connect/$1", true)
                    .AddRewrite(@"^api/v4/user", "api/profile/alt", true)
                    .AddRedirect(@"^.well-known/change-password", "account/password")
            );

            app.UseStaticFiles();

            if (_branding.IncludeSwagger)
                app.UseConfiguredSwagger(_authOptions, _branding);

            app.UseRouting();

            app.UseCors(_headers.Cors.Name);

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapJAvatar(_javatar.RoutePrefix).RequireAuthorization();

                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
