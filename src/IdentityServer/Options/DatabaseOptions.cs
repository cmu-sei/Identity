// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace IdentityServer.Options
{
    public class DatabaseOptions
    {
        public string Provider { get; set; } = "InMemory";
        public string ConnectionString { get; set; } = "IdentityServer";
        public string SeedFile { get; set; }
    }
}
