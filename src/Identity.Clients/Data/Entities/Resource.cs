// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Generic;
using Identity.Clients.Abstractions;
using Identity.Clients.Data.Abstractions;

namespace Identity.Clients.Data
{
    public class Resource : IEntity, IEntityPrimary
    {
        public int Id { get; set; }
        public ResourceType Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EnlistCode { get; set; }
        public bool Enabled { get; set; }
        public bool Default { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public string Scopes { get; set; }
        public ICollection<ApiSecret> Secrets { get; set; } = new List<ApiSecret>();
        public ICollection<ResourceManager> Managers { get; set; } = new List<ResourceManager>();

        [Obsolete]
        public ICollection<ClientResource> Clients { get; set; }
    }
}
