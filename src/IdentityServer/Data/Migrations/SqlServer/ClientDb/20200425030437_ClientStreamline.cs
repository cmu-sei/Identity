// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.SqlServer.ClientDb
{
    public partial class ClientStreamline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Clients"" SET ""ConsentLifetime"" = 1296000 WHERE ""ConsentLifetime"" is NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientEventHandlers_ClientEvents_ClientEventId",
                table: "ClientEventHandlers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientEventHandlers_Clients_ClientId",
                table: "ClientEventHandlers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientEvents_Clients_ClientId",
                table: "ClientEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientManagers_Clients_ClientId",
                table: "ClientManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientSecrets_Clients_ClientId",
                table: "ClientSecrets");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUris_Clients_ClientId",
                table: "ClientUris");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceClaims_Resources_ResourceId",
                table: "ResourceClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceClaims",
                table: "ResourceClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUris",
                table: "ClientUris");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientSecrets",
                table: "ClientSecrets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientManagers",
                table: "ClientManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEvents",
                table: "ClientEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEventHandlers",
                table: "ClientEventHandlers");

            migrationBuilder.RenameTable(
                name: "ResourceClaims",
                newName: "ResourceClaim");

            migrationBuilder.RenameTable(
                name: "ClientUris",
                newName: "ClientUri");

            migrationBuilder.RenameTable(
                name: "ClientSecrets",
                newName: "ClientSecret");

            migrationBuilder.RenameTable(
                name: "ClientManagers",
                newName: "ClientManager");

            migrationBuilder.RenameTable(
                name: "ClientEvents",
                newName: "ClientEvent");

            migrationBuilder.RenameTable(
                name: "ClientEventHandlers",
                newName: "ClientEventHandler");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceClaims_ResourceId",
                table: "ResourceClaim",
                newName: "IX_ResourceClaim_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUris_ClientId",
                table: "ClientUri",
                newName: "IX_ClientUri_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientSecrets_ClientId",
                table: "ClientSecret",
                newName: "IX_ClientSecret_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientManagers_ClientId",
                table: "ClientManager",
                newName: "IX_ClientManager_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEvents_ClientId",
                table: "ClientEvent",
                newName: "IX_ClientEvent_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEventHandlers_ClientId",
                table: "ClientEventHandler",
                newName: "IX_ClientEventHandler_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEventHandlers_ClientEventId",
                table: "ClientEventHandler",
                newName: "IX_ClientEventHandler_ClientEventId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Resources",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnlistCode",
                table: "Resources",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ConsentLifetime",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grants",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scopes",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientClaim",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceClaim",
                table: "ResourceClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUri",
                table: "ClientUri",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientSecret",
                table: "ClientSecret",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientManager",
                table: "ClientManager",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEvent",
                table: "ClientEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEventHandler",
                table: "ClientEventHandler",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ResourceManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceManager_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Name",
                table: "Resources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceManager_ResourceId",
                table: "ResourceManager",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEvent_Clients_ClientId",
                table: "ClientEvent",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEventHandler_ClientEvent_ClientEventId",
                table: "ClientEventHandler",
                column: "ClientEventId",
                principalTable: "ClientEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEventHandler_Clients_ClientId",
                table: "ClientEventHandler",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientManager_Clients_ClientId",
                table: "ClientManager",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientSecret_Clients_ClientId",
                table: "ClientSecret",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUri_Clients_ClientId",
                table: "ClientUri",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceClaim_Resources_ResourceId",
                table: "ResourceClaim",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientEvent_Clients_ClientId",
                table: "ClientEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientEventHandler_ClientEvent_ClientEventId",
                table: "ClientEventHandler");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientEventHandler_Clients_ClientId",
                table: "ClientEventHandler");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientManager_Clients_ClientId",
                table: "ClientManager");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientSecret_Clients_ClientId",
                table: "ClientSecret");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUri_Clients_ClientId",
                table: "ClientUri");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceClaim_Resources_ResourceId",
                table: "ResourceClaim");

            migrationBuilder.DropTable(
                name: "ResourceManager");

            migrationBuilder.DropIndex(
                name: "IX_Resources_Name",
                table: "Resources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResourceClaim",
                table: "ResourceClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUri",
                table: "ClientUri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientSecret",
                table: "ClientSecret");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientManager",
                table: "ClientManager");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEventHandler",
                table: "ClientEventHandler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEvent",
                table: "ClientEvent");

            migrationBuilder.DropColumn(
                name: "EnlistCode",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Grants",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Scopes",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "ResourceClaim",
                newName: "ResourceClaims");

            migrationBuilder.RenameTable(
                name: "ClientUri",
                newName: "ClientUris");

            migrationBuilder.RenameTable(
                name: "ClientSecret",
                newName: "ClientSecrets");

            migrationBuilder.RenameTable(
                name: "ClientManager",
                newName: "ClientManagers");

            migrationBuilder.RenameTable(
                name: "ClientEventHandler",
                newName: "ClientEventHandlers");

            migrationBuilder.RenameTable(
                name: "ClientEvent",
                newName: "ClientEvents");

            migrationBuilder.RenameIndex(
                name: "IX_ResourceClaim_ResourceId",
                table: "ResourceClaims",
                newName: "IX_ResourceClaims_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUri_ClientId",
                table: "ClientUris",
                newName: "IX_ClientUris_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientSecret_ClientId",
                table: "ClientSecrets",
                newName: "IX_ClientSecrets_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientManager_ClientId",
                table: "ClientManagers",
                newName: "IX_ClientManagers_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEventHandler_ClientId",
                table: "ClientEventHandlers",
                newName: "IX_ClientEventHandlers_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEventHandler_ClientEventId",
                table: "ClientEventHandlers",
                newName: "IX_ClientEventHandlers_ClientEventId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientEvent_ClientId",
                table: "ClientEvents",
                newName: "IX_ClientEvents_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Resources",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ConsentLifetime",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientClaim",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResourceClaims",
                table: "ResourceClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUris",
                table: "ClientUris",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientSecrets",
                table: "ClientSecrets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientManagers",
                table: "ClientManagers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEventHandlers",
                table: "ClientEventHandlers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEvents",
                table: "ClientEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEventHandlers_ClientEvents_ClientEventId",
                table: "ClientEventHandlers",
                column: "ClientEventId",
                principalTable: "ClientEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEventHandlers_Clients_ClientId",
                table: "ClientEventHandlers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientEvents_Clients_ClientId",
                table: "ClientEvents",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientManagers_Clients_ClientId",
                table: "ClientManagers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientSecrets_Clients_ClientId",
                table: "ClientSecrets",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUris_Clients_ClientId",
                table: "ClientUris",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceClaims_Resources_ResourceId",
                table: "ResourceClaims",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
