// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;
using Identity.Clients.Data.EntityFrameworkCore;
using Identity.Clients.Services;
using Microsoft.Extensions.Logging;

namespace Identity.Clients.Tests
{
    public class TestContext : IDisposable
    {
        public TestContext(
            ClientDbContext ctx,
            ILoggerFactory loggerFactory,
            IMapper mapper
        )
        {
            _ctx = ctx;
            _loggerFactory = loggerFactory;
            Mapper = mapper;
        }

        private readonly ClientDbContext _ctx = null;
        private readonly ILoggerFactory _loggerFactory;
        public ClientDbContext Context { get { return _ctx;}}
        private IProfileService _profile;
        private readonly IMapper Mapper;

        #region Services

        public IProfileService SetProfile(string name = "Somebody", bool IsAdmin = false)
        {
            _profile = new ProfileService
            {
                Profile = new Profile
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    IsAdmin = IsAdmin
                }
            };
            return _profile;
        }

        public ClientService GetClientService()
        {
            return GetService(typeof(ClientService), new ClientStore(_ctx)) as ClientService;
        }
        // public WebhookService GetWebhookService()
        // {
        //     return GetService4(typeof(WebhookService), new WebhookStore(_ctx)) as WebhookService;
        // }
        // public SecretService GetSecretService()
        // {
        //     return GetService(typeof(SecretService), new SecretStore(_ctx)) as SecretService;
        // }
        // public ManagerService GetManagerService()
        // {
        //     return GetService(typeof(ManagerService), new ManagerStore(_ctx)) as ManagerService;
        // }
        // public EventService GetEventService()
        // {
        //     return GetService(typeof(EventService), new EventStore(_ctx)) as EventService;
        // }
        // public UriService GetUriService()
        // {
        //     return GetService(typeof(UriService), new UriStore(_ctx)) as UriService;
        // }
        public ResourceService GetResourceService()
        {
            return GetService(typeof(ResourceService), new ResourceStore(_ctx)) as ResourceService;
        }
        // public ResourceClaimService GetResourceClaimService()
        // {
        //     return GetService(typeof(ResourceClaimService), new ResourceClaimStore(_ctx)) as ResourceClaimService;
        // }

        private Dictionary<string, object> _svcCache = new Dictionary<string, object>();
        //private Dictionary<string, Profile> _actors = new Dictionary<string, Profile>();

        private object FindService(Type t)
        {
            if (_svcCache.ContainsKey(t.Name))
                return _svcCache[t.Name];
            return null;
        }

        private object GetService(Type t, object store)
        {
            object svc = FindService(t);
            if (svc == null)
            {
                svc = Activator.CreateInstance(
                    t,
                    //new ClientStore(_ctx),
                    store,
                    _profile ?? SetProfile(),
                    Activator.CreateInstance(typeof(TestLogger<>).MakeGenericType(t)),
                    Mapper
                );
                _svcCache.Add(t.Name, svc);
            }
            return svc;
        }
        private object GetService4(Type t, object store)
        {
            object svc = FindService(t);
            if (svc == null)
            {
                svc = Activator.CreateInstance(
                    t,
                    //new ClientStore(_ctx),
                    store,
                    _profile ?? SetProfile(),
                    Activator.CreateInstance(typeof(TestLogger<>).MakeGenericType(t)),
                    Mapper,
                    null
                );
                _svcCache.Add(t.Name, svc);
            }
            return svc;
        }

        #endregion

        public void Dispose()
        {
            if (_ctx != null)
                _ctx.Dispose();
        }
    }
}
