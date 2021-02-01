// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using IdentityServer4.Models;

namespace IdentityServer.Models
{
    public class DbSeedModel
    {
        public DbSeedUser[] Users { get; set; } = new DbSeedUser[] {};
        public DbSeedClient[] Clients { get; set; } = new DbSeedClient[] {};
        public DbSeedApi[] ApiResources { get; set; } = new DbSeedApi[] {};
        public IdentityResource[] IdentityResources { get; set; } = new IdentityResource[] {};
        public IdentityResource[] GrantResources { get; set; } = new IdentityResource[] {};
        public string DefaultClientFlags { get; set; }
    }

    public class DbSeedUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string GlobalId { get; set; }
    }

    public class DbSeedKVP
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class DbSeedClient : Identity.Clients.Data.Client
    {
        public string SeedFlags { get; set; }
        public string SeedGrant { get; set; }
        public string SeedScopes { get; set; }
        public string SeedSecret { get; set; }
        public DbSeedKVP[] SeedHandlers { get; set; } = new DbSeedKVP[] {};
    }

    public class DbSeedApi : Identity.Clients.Data.Resource
    {
        public string SeedSecret { get; set; }
    }
}
