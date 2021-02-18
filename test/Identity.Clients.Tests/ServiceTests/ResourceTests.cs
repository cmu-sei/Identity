// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using Identity.Clients.Abstractions;
using Identity.Clients.Models;
using Xunit;

namespace Identity.Clients.Tests
{
    public class ResourceServiceTests : CoreTest
    {

        [Fact]
        public void AdminCanManageResources()
        {
            // using (var test = CreateTestContext())
            // {
            //     test.SetProfile("JeffM", true);
            //     var resource = test.GetResourceService().Add(ModelFactory.Resource()).Result;
            //     Assert.NotNull(resource);

            //     var claimSvc = test.GetResourceClaimService();
            //     var claim = claimSvc.Add(new NewResourceClaim
            //     {
            //         ResourceId = resource.Id,
            //         Type = resource.Name
            //     }).Result;
            //     Assert.NotNull(claim);

            //     claimSvc.Update(new ChangedResourceClaim
            //     {
            //         Id = claim.Id,
            //         Type = resource.Name + "-changed"
            //     }).Wait();
            //     Assert.NotNull(claim);

            //     var changedResource = ModelFactory.ChangedResource(resource);
            //     changedResource.DisplayName += "-changed-";
            //     test.GetResourceService().Update(changedResource).Wait();
            //     Assert.NotNull(resource);

            //     var list = test.GetResourceService().List().Result;
            //     Assert.NotEmpty(list);

            //     var client = test.GetClientService().Add(ModelFactory.Client("jam")).Result;
            //     test.GetClientService().AddResource(new ClientResourceMap
            //     {
            //         ClientId = client.Id,
            //         ResourceId = resource.Id
            //     }).Wait();
            //     client = test.GetClientService().Load(client.Id).Result;
            //     Assert.NotNull(client);
            // }
        }

        [Fact]
        public void CanSeedDb()
        {
            using (var db = CreateContext())
            using (var test = CreateTestContext())
            {
                var resources = new Data.Resource[]
                {
                    new Data.Resource{
                        Type = ResourceType.Api,
                        Name = "sketch-common",
                        Scopes = "sketch-common"
                    },
                    new Data.Resource{
                        Type = ResourceType.Identity,
                        Name = "openid",
                        Scopes = "openid"
                    },
                    new Data.Resource{
                        Type = ResourceType.Identity,
                        Name = "profile",
                        Scopes = "name picture timezone"
                    },
                    new Data.Resource{
                        Type = ResourceType.Identity,
                        Name = "email",
                        Scopes = "email email_verified"
                    },
                    new Data.Resource{
                        Type = ResourceType.Identity,
                        Name = "organization",
                        Scopes = "name logo"
                    },
                    new Data.Resource{
                        Type = ResourceType.Grant,
                        Name = "implicit",
                        Scopes = "implicit client_credentials"
                    },
                    new Data.Resource{
                        Type = ResourceType.Grant,
                        Name = "hybrid",
                        Scopes = "hybrid client_credentials"
                    },
                    new Data.Resource{
                        Type = ResourceType.Grant,
                        Name = "code",
                        Scopes = "code client_credentials"
                    },
                    new Data.Resource{
                        Type = ResourceType.Grant,
                        Name = "client",
                        Scopes = "client_credentials"
                    }
                };

                db.Resources.AddRange(resources);
                db.SaveChanges();

                var list = test.GetResourceService().List(new SearchModel()).Result;
                Assert.NotNull(list);
            }
        }
    }
}
