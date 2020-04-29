// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

﻿// <auto-generated />
using System;
using Identity.Clients.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    [DbContext(typeof(ClientDbContextPostgreSQL))]
    partial class ClientDbContextPostgreSQLModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Identity.Clients.Data.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AbsoluteRefreshTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<int>("AccessTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<int>("AccessTokenType")
                        .HasColumnType("integer");

                    b.Property<int>("AuthorizationCodeLifetime")
                        .HasColumnType("integer");

                    b.Property<string>("ClientClaimsPrefix")
                        .HasColumnType("character varying(20)")
                        .HasMaxLength(20);

                    b.Property<int>("ConsentLifetime")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("character varying(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("EnlistCode")
                        .HasColumnType("text");

                    b.Property<int>("Flags")
                        .HasColumnType("integer");

                    b.Property<string>("GlobalId")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Grants")
                        .HasColumnType("text");

                    b.Property<int>("IdentityTokenLifetime")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("PairWiseSubjectSalt")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("ProtocolType")
                        .IsRequired()
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<int>("RefreshTokenExpiration")
                        .HasColumnType("integer");

                    b.Property<int>("RefreshTokenUsage")
                        .HasColumnType("integer");

                    b.Property<string>("Scopes")
                        .HasColumnType("text");

                    b.Property<int>("SlidingRefreshTokenLifetime")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientClaim");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientEvent");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientEventHandler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientEventId")
                        .HasColumnType("integer");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Uri")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientEventId");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientEventHandler");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("SubjectId")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientManager");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientResource", b =>
                {
                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.HasKey("ClientId", "ResourceId");

                    b.HasIndex("ResourceId");

                    b.ToTable("ClientResources");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientSecret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Type")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientSecret");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientUri", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientUri");
                });

            modelBuilder.Entity("Identity.Clients.Data.PersistedGrant", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("character varying(50000)")
                        .HasMaxLength(50000);

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SubjectId")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Key");

                    b.HasIndex("SubjectId", "ClientId", "Type");

                    b.ToTable("PersistedGrants");
                });

            modelBuilder.Entity("Identity.Clients.Data.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("Default")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("character varying(1000)")
                        .HasMaxLength(1000);

                    b.Property<string>("DisplayName")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<bool>("Emphasize")
                        .HasColumnType("boolean");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("EnlistCode")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("Required")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowInDiscoveryDocument")
                        .HasColumnType("boolean");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Resources");
                });

            modelBuilder.Entity("Identity.Clients.Data.ResourceClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceClaim");
                });

            modelBuilder.Entity("Identity.Clients.Data.ResourceManager", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.Property<string>("SubjectId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceManager");
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientClaim", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Claims")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientEvent", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Events")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientEventHandler", b =>
                {
                    b.HasOne("Identity.Clients.Data.ClientEvent", "ClientEvent")
                        .WithMany("Handlers")
                        .HasForeignKey("ClientEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("EventHandlers")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientManager", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Managers")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientResource", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Resources")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Identity.Clients.Data.Resource", "Resource")
                        .WithMany("Clients")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientSecret", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Secrets")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ClientUri", b =>
                {
                    b.HasOne("Identity.Clients.Data.Client", "Client")
                        .WithMany("Urls")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ResourceClaim", b =>
                {
                    b.HasOne("Identity.Clients.Data.Resource", null)
                        .WithMany("Claims")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Clients.Data.ResourceManager", b =>
                {
                    b.HasOne("Identity.Clients.Data.Resource", "Resource")
                        .WithMany("Managers")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
