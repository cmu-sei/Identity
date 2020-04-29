// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Clients.Abstractions;
using Identity.Clients.Models;
using Xunit;

namespace Identity.Clients.Tests
{
    public class ClientServiceTests : CoreTest
    {
        [Fact]
        public void OwnerCanCreateAndEditClient()
        {
            using (TestContext test = CreateTestContext())
            {
                var svc = test.GetClientService();

                var client = svc.Add(ModelFactory.Client("jam")).Result;
                Assert.NotNull(client);

                Client changed = new Client
                {
                    Id = client.Id,
                    Name = "jam-test-changed",
                    DisplayName = "jam's changed Test Client",
                    // Urls = new ClientUri[]
                    // {
                    //     new ClientUri { Type = ClientUriType.ClientUri, Value="http://localhost" },
                    //     new ClientUri { Type = ClientUriType.LogoUri, Value="http://localhost/logo.png" },
                    // },
                    //Flags = ClientFlag.Published | ClientFlag.AllowAccessTokensViaBrowser | ClientFlag.AllowRememberConsent
                };
                svc.Update(changed).Wait();
                Assert.NotNull(client);
                // Assert.NotEmpty(client.Urls);

                var result = svc.Find(new SearchModel()).Result;
                Assert.NotEmpty(result);
            }
        }

        [Fact]
        public void OwnerCanRetrieveEventHandlerTargets()
        {
            // using (var jack = CreateTestContext())
            // using (var jill = CreateTestContext())
            // {
            //     var jackClient = jack.GetClientService().Add(ModelFactory.Client("jack")).Result;
            //     Assert.NotNull(jackClient);

            //     List<ClientEvent> events = new List<ClientEvent>();
            //     for (int i = 1; i < 3; i++)
            //     {
            //         events.Add(
            //             jack.GetEventService().Add(new NewClientEvent
            //             {
            //                 ClientId = jackClient.Id,
            //                 Type = "event" + i.ToString()
            //             }).Result
            //         );
            //     }

            //     var jillClient = jill.GetClientService()
            //     .Add(new NewClient
            //     {
            //         //Name = "jill-test",
            //         DisplayName = "Jill Test Client",
            //     }).Result;

            //     var handler = new NewClientEventHandler[]
            //     {
            //         new NewClientEventHandler { ClientId = jillClient.Id, Uri = "http://jill/event1", ClientEventId = events[0].Id },
            //         new NewClientEventHandler { ClientId = jillClient.Id, Uri = "http://jill/event2", ClientEventId = events[1].Id }
            //     };
            //     var h0 = jill.GetWebhookService().Add(handler[0]).Result;
            //     var h1 = jill.GetWebhookService().Add(handler[1]).Result;

            //     jillClient = jill.GetClientService().Load(jillClient.Id).Result;
            //     jill.GetWebhookService().Delete(h0.Id).Wait();
            //     var changedHandler = new ChangedClientEventHandler
            //     {
            //         Id = h1.Id,
            //         Uri = "http://jill/event2-changed"
            //     };
            //     jill.GetWebhookService().Update(changedHandler).Wait();

            //     //jack gets all targets of his client's events
            //     var handlers = jack.GetWebhookService().List(jackClient.Name).Result;
            //     Assert.NotEmpty(handlers);
            // }
        }

        [Fact]
        public async Task OwnerCanManageClientSecrets()
        {
            // using (var jack = CreateTestContext())
            // using (var jill = CreateTestContext())
            // {
            //     var jackClient = jack.GetClientService().Add(ModelFactory.Client("jack")).Result;
            //     Assert.NotNull(jackClient);

            //     var s1 = jack.GetSecretService().Add(jackClient.Id).Result;
            //     var s2 = jack.GetSecretService().Add(jackClient.Id).Result;

            //     jack.GetSecretService().Delete(s1.Id).Wait();
            //     jackClient = jack.GetClientService().Load(jackClient.Id).Result;
            //     Assert.True(jackClient.Secrets.Count() == 1);

            //     //jill can't manage jack's secrets
            //     await Assert.ThrowsAsync<InvalidOperationException>(() => jill.GetSecretService().Delete(s2.Id));
            //     await Assert.ThrowsAsync<InvalidOperationException>(() => jill.GetSecretService().Add(jackClient.Id));
            // }
        }
    }
}
