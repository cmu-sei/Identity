// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using Identity.Clients.Abstractions;
using Identity.Clients.Models;

namespace Identity.Clients.Tests
{
    public class ModelFactory
    {
        public static NewClient Client(string name)
        {
            return new NewClient
            {
                //Name = name + "-client",
                DisplayName = name + " Test Client",
                // Events = new ClientEvent[]
                // {
                //     new ClientEvent { Type = "event1" },
                //     new ClientEvent { Type = "event2" },
                // },
                // Urls = new ClientUri[]
                // {
                //     new ClientUri { Type = ClientUriType.ClientUri, Value=String.Format("http://{0}.client", name) },
                //     new ClientUri { Type = ClientUriType.LogoUri, Value=String.Format("http://{0}.client/logo.png", name) },
                // },
                // Flags = ClientFlag.Published | ClientFlag.AllowAccessTokensViaBrowser | ClientFlag.AllowRememberConsent
            };
        }

        public static NewResource Resource()
        {
            return new NewResource
            {
                //Name = "sketch-common",
                Type = ResourceType.Api,
                DisplayName = "Sketch Common Scope",
                Description = "Honored by most extensions as a public access scope.",
                Enabled = true,
                Default = true,
                Required = false,
                Emphasize = false,
                ShowInDiscoveryDocument = true
            };
        }

        public static ChangedResource ChangedResource(Resource source)
        {
            return new ChangedResource
            {
                Id = source.Id,
                Name = source.Name,
                DisplayName = source.DisplayName,
                Description = source.Description,
                Enabled = source.Enabled,
                Default = source.Default,
                Emphasize = source.Emphasize,
                Required = source.Required,
                ShowInDiscoveryDocument = true,
            };
        }
    }
}