// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Collections.Generic;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(type: "int4", nullable: false),
                    AccessTokenLifetime = table.Column<int>(type: "int4", nullable: false),
                    AccessTokenType = table.Column<int>(type: "int4", nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(type: "int4", nullable: false),
                    ClientClaimsPrefix = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    ConsentLifetime = table.Column<int>(type: "int4", nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Enabled = table.Column<bool>(type: "bool", nullable: false),
                    EnlistCode = table.Column<string>(type: "text", nullable: true),
                    Flags = table.Column<int>(type: "int4", nullable: false),
                    GlobalId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    IdentityTokenLifetime = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PairWiseSubjectSalt = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ProtocolType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    RefreshTokenExpiration = table.Column<int>(type: "int4", nullable: false),
                    RefreshTokenUsage = table.Column<int>(type: "int4", nullable: false),
                    SlidingRefreshTokenLifetime = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ClientId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Data = table.Column<string>(type: "varchar(50000)", maxLength: 50000, nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp", nullable: true),
                    SubjectId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Default = table.Column<bool>(type: "bool", nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Emphasize = table.Column<bool>(type: "bool", nullable: false),
                    Enabled = table.Column<bool>(type: "bool", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Required = table.Column<bool>(type: "bool", nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(type: "bool", nullable: false),
                    Type = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientClaim_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEvents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientManagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SubjectId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientManagers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Expiration = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSecrets_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Type = table.Column<int>(type: "int4", nullable: false),
                    Value = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientResources",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    ResourceId = table.Column<int>(type: "int4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientResources", x => new { x.ClientId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ClientResources_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientResources_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ResourceId = table.Column<int>(type: "int4", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceClaims_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientEventHandlers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClientEventId = table.Column<int>(type: "int4", nullable: false),
                    ClientId = table.Column<int>(type: "int4", nullable: false),
                    Enabled = table.Column<bool>(type: "bool", nullable: false),
                    Uri = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEventHandlers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientEventHandlers_ClientEvents_ClientEventId",
                        column: x => x.ClientEventId,
                        principalTable: "ClientEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientEventHandlers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientClaim_ClientId",
                table: "ClientClaim",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEventHandlers_ClientEventId",
                table: "ClientEventHandlers",
                column: "ClientEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEventHandlers_ClientId",
                table: "ClientEventHandlers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEvents_ClientId",
                table: "ClientEvents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientManagers_ClientId",
                table: "ClientManagers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientResources_ResourceId",
                table: "ClientResources",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_GlobalId",
                table: "Clients",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Name",
                table: "Clients",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientSecrets_ClientId",
                table: "ClientSecrets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUris_ClientId",
                table: "ClientUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceClaims_ResourceId",
                table: "ResourceClaims",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientClaim");

            migrationBuilder.DropTable(
                name: "ClientEventHandlers");

            migrationBuilder.DropTable(
                name: "ClientManagers");

            migrationBuilder.DropTable(
                name: "ClientResources");

            migrationBuilder.DropTable(
                name: "ClientSecrets");

            migrationBuilder.DropTable(
                name: "ClientUris");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "ResourceClaims");

            migrationBuilder.DropTable(
                name: "ClientEvents");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
