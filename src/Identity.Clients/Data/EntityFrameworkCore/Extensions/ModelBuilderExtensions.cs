// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Microsoft.EntityFrameworkCore;

namespace Identity.Clients.Data.EntityFrameworkCore.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureClientContext(this ModelBuilder builder)
        {
            builder.Entity<Client>(client =>
            {
                client.Property(x => x.GlobalId).HasMaxLength(50).IsRequired();
                client.Property(x => x.ProtocolType).HasMaxLength(200).IsRequired();
                client.Property(x => x.Name).HasMaxLength(50);
                client.Property(x => x.DisplayName).HasMaxLength(100);
                client.Property(x => x.Description).HasMaxLength(1000);
                client.Property(x => x.ClientClaimsPrefix).HasMaxLength(20);
                client.Property(x => x.PairWiseSubjectSalt).HasMaxLength(50);

                client.HasIndex(x => x.Name).IsUnique();
                client.HasIndex(x => x.GlobalId).IsUnique();

            });

            builder.Entity<ClientUri>(uri =>
            {
                uri.Property(x => x.Value).HasMaxLength(200);
            });

            builder.Entity<ClientManager>(uri =>
            {
                uri.Property(x => x.SubjectId).HasMaxLength(50);
            });

            builder.Entity<ClientEvent>(claim =>
            {
                claim.Property(x => x.Type).HasMaxLength(50);
            });

            builder.Entity<ClientEventHandler>(claim =>
            {
                claim.Property(x => x.Uri).HasMaxLength(200);
            });

            builder.Entity<ClientSecret>(secret =>
            {
                secret.Property(x => x.Value).HasMaxLength(50).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(50);
                secret.Property(x => x.Description).HasMaxLength(200);
            });

            builder.Entity<ClientClaim>(claim =>
            {
                claim.Property(x => x.Type).HasMaxLength(50).IsRequired();
                claim.Property(x => x.Value).HasMaxLength(200).IsRequired();
            });

            builder.Entity<ClientResource>(cr =>
            {
                cr.HasKey(x => new { x.ClientId, x.ResourceId });
            });

            builder.Entity<PersistedGrant>(grant =>
            {
                grant.Property(x => x.Key).HasMaxLength(200).ValueGeneratedNever();
                grant.Property(x => x.Type).HasMaxLength(50).IsRequired();
                grant.Property(x => x.SubjectId).HasMaxLength(200);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.CreationTime).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                grant.Property(x => x.Data).HasMaxLength(50000).IsRequired();
                grant.HasKey(x => x.Key);
                grant.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
            });

            builder.Entity<Resource>(resource =>
            {
                resource.Property(x => x.Name).HasMaxLength(50).IsRequired();
                resource.HasIndex(x => x.Name).IsUnique();
                resource.Property(x => x.DisplayName).HasMaxLength(100);
                resource.Property(x => x.Description).HasMaxLength(1000);
                resource.Property(x => x.Scopes).HasMaxLength(200);
            });

            builder.Entity<ApiSecret>(secret =>
            {
                secret.Property(x => x.Value).HasMaxLength(50).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(50);
                secret.Property(x => x.Description).HasMaxLength(200);
            });


        }
    }
}
