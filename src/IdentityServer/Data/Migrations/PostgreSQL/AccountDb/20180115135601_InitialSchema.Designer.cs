// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

// <auto-generated />
using Identity.Accounts.Abstractions;
using Identity.Accounts.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace IdentityServer.Data.Migrations.PostgreSQL.AccountDb
{
    [DbContext(typeof(AccountDbContextPostgreSQL))]
    [Migration("20180115135601_InitialSchema")]
    partial class InitialSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("Identity.Accounts.Data.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuthenticationFailures");

                    b.Property<string>("GlobalId")
                        .HasMaxLength(36);

                    b.Property<int>("LockedMinutes");

                    b.Property<int>("Role");

                    b.Property<int>("Status");

                    b.Property<DateTime>("WhenAuthenticated");

                    b.Property<DateTime>("WhenCreated");

                    b.Property<DateTime>("WhenLastAuthenticated");

                    b.Property<DateTime>("WhenLocked");

                    b.Property<string>("WhereAuthenticated")
                        .HasMaxLength(36);

                    b.Property<string>("WhereLastAuthenticated")
                        .HasMaxLength(36);

                    b.HasKey("Id");

                    b.HasIndex("GlobalId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountCode", b =>
                {
                    b.Property<string>("Hash")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<int>("Code");

                    b.Property<DateTime>("WhenCreated");

                    b.HasKey("Hash");

                    b.ToTable("AccountCodes");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<string>("Key")
                        .HasMaxLength(64);

                    b.Property<string>("Value")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountProperties");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountToken", b =>
                {
                    b.Property<string>("Hash")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<int>("Type");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("WhenCreated");

                    b.HasKey("Hash");

                    b.HasIndex("UserId");

                    b.ToTable("AccountTokens");
                });

            modelBuilder.Entity("Identity.Accounts.Data.OverrideCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(64);

                    b.Property<string>("Description")
                        .HasMaxLength(256);

                    b.Property<DateTime>("WhenCreated");

                    b.HasKey("Id");

                    b.ToTable("OverrideCodes");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountProperty", b =>
                {
                    b.HasOne("Identity.Accounts.Data.Account", "Account")
                        .WithMany("Properties")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountToken", b =>
                {
                    b.HasOne("Identity.Accounts.Data.Account", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
