﻿// <auto-generated />
using System;
using Identity.Accounts.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IdentityServer.Data.Migrations.PostgreSQL.AccountDb
{
    [DbContext(typeof(AccountDbContextPostgreSQL))]
    partial class AccountDbContextPostgreSQLModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Identity.Accounts.Data.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AuthenticationFailures")
                        .HasColumnType("integer");

                    b.Property<string>("GlobalId")
                        .HasColumnType("character varying(36)")
                        .HasMaxLength(36);

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<int>("LockedMinutes")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("WhenAuthenticated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("WhenCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("WhenLastAuthenticated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("WhenLocked")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("WhereAuthenticated")
                        .HasColumnType("character varying(48)")
                        .HasMaxLength(48);

                    b.Property<string>("WhereLastAuthenticated")
                        .HasColumnType("character varying(48)")
                        .HasMaxLength(48);

                    b.HasKey("Id");

                    b.HasIndex("GlobalId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountCode", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<DateTime>("WhenCreated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Hash");

                    b.ToTable("AccountCodes");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Key")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Value")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountProperties");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountToken", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("WhenCreated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Hash");

                    b.HasIndex("UserId");

                    b.ToTable("AccountTokens");
                });

            modelBuilder.Entity("Identity.Accounts.Data.OverrideCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("character varying(64)")
                        .HasMaxLength(64);

                    b.Property<string>("Description")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<DateTime>("WhenCreated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("OverrideCodes");
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountProperty", b =>
                {
                    b.HasOne("Identity.Accounts.Data.Account", "Account")
                        .WithMany("Properties")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Identity.Accounts.Data.AccountToken", b =>
                {
                    b.HasOne("Identity.Accounts.Data.Account", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
