// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Collections.Generic;

namespace IdentityServer.Data.Migrations.PostgreSQL.AccountDb
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCodes",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    Code = table.Column<int>(type: "int4", nullable: false),
                    WhenCreated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCodes", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AuthenticationFailures = table.Column<int>(type: "int4", nullable: false),
                    GlobalId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    LockedMinutes = table.Column<int>(type: "int4", nullable: false),
                    Role = table.Column<int>(type: "int4", nullable: false),
                    Status = table.Column<int>(type: "int4", nullable: false),
                    WhenAuthenticated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WhenCreated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WhenLastAuthenticated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WhenLocked = table.Column<DateTime>(type: "timestamp", nullable: false),
                    WhereAuthenticated = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    WhereLastAuthenticated = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverrideCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Description = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    WhenCreated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverrideCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AccountId = table.Column<int>(type: "int4", nullable: false),
                    Key = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    Value = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountProperties_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTokens",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    Type = table.Column<int>(type: "int4", nullable: false),
                    UserId = table.Column<int>(type: "int4", nullable: false),
                    WhenCreated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTokens", x => x.Hash);
                    table.ForeignKey(
                        name: "FK_AccountTokens_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountProperties_AccountId",
                table: "AccountProperties",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_GlobalId",
                table: "Accounts",
                column: "GlobalId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTokens_UserId",
                table: "AccountTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCodes");

            migrationBuilder.DropTable(
                name: "AccountProperties");

            migrationBuilder.DropTable(
                name: "AccountTokens");

            migrationBuilder.DropTable(
                name: "OverrideCodes");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
