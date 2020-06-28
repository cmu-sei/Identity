// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using AutoMapper;
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.EntityFrameworkCore;
using Identity.Accounts.Options;
using Identity.Accounts.Services;
using Microsoft.Extensions.Logging;

namespace Tests.Common
{
    public class TestContext : IDisposable
    {
        public TestContext(
            AccountDbContext ctx,
            ILoggerFactory loggerFactory,
            IMapper mapper
        )
        {
            _ctx = ctx;
            _loggerFactory = loggerFactory;
            Mapper = mapper;

            _options = new AccountOptions();
            _options.Password.History = 3;
            _options.Password.ComplexityText = "At least 8 characters containing uppercase and lowercase letters, numbers, and symbols";
            _options.Password.ComplexityExpression = @"(?=^.{8,}$)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[`~!@#$%^&*\(\)\-_=+\[\]\{\}\\|;:'"",<\.>/?\ ]).*$";
            _options.Registration.AllowedDomains = @"^.+@.*jam\.net|test\.com$";
            _options.Registration.AllowedDomains = @"jam.net test.com";
            // _options.Registration.RequireNameFromCertificate = true;
            _options.Authentication.LockThreshold = 5;

        }

        protected AccountOptions _options = null;
        public AccountOptions Options { get { return _options; }}
        private readonly AccountDbContext _ctx = null;
        private readonly ILoggerFactory _loggerFactory;
        public AccountDbContext Context { get { return _ctx;}}
        protected IMapper Mapper { get; }
        //private IProfileService _profile;

        #region Services

        // public IProfileService SetProfile(string name = "Somebody", bool IsAdmin = false)
        // {
        //     _profile = new ProfileService
        //     {
        //         Profile = new Profile
        //         {
        //             Id = Guid.NewGuid().ToString(),
        //             Name = name,
        //             IsAdmin = IsAdmin
        //         }
        //     };
        //     return _profile;
        // }

        public AccountService GetAccountService()
        {
            return (AccountService)FindService(typeof(AccountService)) ??
                new AccountService(
                    new AccountStore(_ctx, null),
                    Options,
                    new TestLogger<AccountService>(),
                    TestCertificates.CertStoreFactory(),
                    null,
                    Mapper,
                    null
                );
        }
        public OverrideService GetOverrideService()
        {
            return (OverrideService)FindService(typeof(OverrideService)) ??
                new OverrideService(
                    new OverrideStore(_ctx, null),
                    Mapper
                );
        }
        public PropertyService GetPropertyService()
        {
            return (PropertyService)FindService(typeof(PropertyService)) ??
                new PropertyService(
                    new PropertyStore(_ctx, null),
                    Mapper
                );
        }

        private Dictionary<string, object> _svcCache = new Dictionary<string, object>();
        //private Dictionary<string, Profile> _actors = new Dictionary<string, Profile>();

        private object FindService(Type t)
        {
            if (_svcCache.ContainsKey(t.Name))
                return _svcCache[t.Name];
            return null;
        }

        // private object GetService(Type t, object store)
        // {
        //     object svc = FindService(t);
        //     if (svc == null)
        //     {
        //         svc = Activator.CreateInstance(
        //             t,
        //             store,
        //             Activator.CreateInstance(typeof(TestLogger<>).MakeGenericType(t))
        //         );
        //         _svcCache.Add(t.Name, svc);
        //     }
        //     return svc;
        // }

        #endregion

        public void Dispose()
        {
            if (_ctx != null)
                _ctx.Dispose();
        }
    }
}
