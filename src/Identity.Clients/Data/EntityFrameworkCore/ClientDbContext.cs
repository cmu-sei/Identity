// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Microsoft.EntityFrameworkCore;
using Identity.Clients.Data.EntityFrameworkCore.Extensions;
using System;

namespace Identity.Clients.Data.EntityFrameworkCore
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureClientContext();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        [Obsolete]
        public DbSet<ClientResource> ClientResources { get; set; }
    }

    public class ClientDbContextPostgreSQL: ClientDbContext
    {
        public ClientDbContextPostgreSQL(DbContextOptions<ClientDbContextPostgreSQL> options) : base(options) {}
    }

    public class ClientDbContextSqlServer: ClientDbContext
    {
        public ClientDbContextSqlServer(DbContextOptions<ClientDbContextSqlServer> options) : base(options) {}
    }

    public class ClientDbContextInMemory: ClientDbContext
    {
        public ClientDbContextInMemory(DbContextOptions<ClientDbContextInMemory> options) : base(options) {}
    }
}
