// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Identity.Accounts.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Accounts.Data.EntityFrameworkCore
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions options)
        : base(options)
        {
        }

        protected  AccountDbContext() { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Account>(b =>
            {
                b.HasIndex(o => o.GlobalId);
                b.Property(p => p.GlobalId).HasMaxLength(36);
                b.Property(p => p.WhereAuthenticated).HasMaxLength(48);
                b.Property(p => p.WhereLastAuthenticated).HasMaxLength(48);
            });

            builder.Entity<AccountToken>(b =>
            {
                b.HasKey(o => o.Hash);
                b.Property(o => o.Hash).HasMaxLength(64);
            });

            builder.Entity<AccountCode>(b =>
            {
                b.HasKey(o => o.Hash);
                b.Property(o => o.Hash).HasMaxLength(64);
            });

            builder.Entity<AccountProperty>(b =>
            {
                b.Property(p => p.Key).HasMaxLength(64);
                b.Property(p => p.Value).HasMaxLength(256);
            });

            builder.Entity<OverrideCode>(b =>
            {
                b.Property(p => p.Code).HasMaxLength(64);
                b.Property(p => p.Description).HasMaxLength(256);
            });
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountToken> AccountTokens { get; set; }
        public DbSet<AccountCode> AccountCodes { get; set; }
        public DbSet<AccountProperty> AccountProperties { get; set; }
        public DbSet<OverrideCode> OverrideCodes { get; set; }
    }

    public class AccountDbContextPostgreSQL: AccountDbContext
    {
        public AccountDbContextPostgreSQL(DbContextOptions<AccountDbContextPostgreSQL> options) : base(options) {}
    }

    public class AccountDbContextSqlServer: AccountDbContext
    {
        public AccountDbContextSqlServer(DbContextOptions<AccountDbContextSqlServer> options) : base(options) {}
    }

    public class AccountDbContextInMemory: AccountDbContext
    {
        public AccountDbContextInMemory(DbContextOptions<AccountDbContextInMemory> options) : base(options) {}
    }
}
