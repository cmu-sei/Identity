// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class ClientStreamline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Clients"" SET ""ConsentLifetime"" = 1296000 WHERE ""ConsentLifetime"" is NULL;");
            // migrationBuilder.Sql(@"ALTER SEQUENCE ""Clients_Id_seq"" RENAME TO ""Client_Id_seq"";");
            // migrationBuilder.Sql(@"ALTER SEQUENCE ""Resources_Id_seq"" RENAME TO ""Resource_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ClientSecrets_Id_seq"" RENAME TO ""ClientSecret_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ClientUris_Id_seq"" RENAME TO ""ClientUri_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ClientManagers_Id_seq"" RENAME TO ""ClientManager_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ClientEvents_Id_seq"" RENAME TO ""ClientEvent_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ClientEventHandlers_Id_seq"" RENAME TO ""ClientEventHandler_Id_seq"";");
            migrationBuilder.Sql(@"ALTER SEQUENCE ""ResourceClaims_Id_seq"" RENAME TO ""ResourceClaim_Id_seq"";");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientClaims_Clients_ClientId",
                table: "ClientClaims");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientClaims",
                table: "ClientClaims");

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

            migrationBuilder.RenameTable(
                name: "ClientClaims",
                newName: "ClientClaim");

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

            migrationBuilder.RenameIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaim",
                newName: "IX_ClientClaim_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Resources",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Resources",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<string>(
                name: "EnlistCode",
                table: "Resources",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ConsentLifetime",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<string>(
                name: "Grants",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scopes",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ResourceClaim",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientUri",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientSecret",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientManager",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientEvent",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientEventHandler",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientClaim",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientClaim",
                table: "ClientClaim",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ResourceManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                name: "FK_ClientClaim_Clients_ClientId",
                table: "ClientClaim",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_ClientClaim_Clients_ClientId",
                table: "ClientClaim");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientClaim",
                table: "ClientClaim");

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

            migrationBuilder.RenameTable(
                name: "ClientClaim",
                newName: "ClientClaims");

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

            migrationBuilder.RenameIndex(
                name: "IX_ClientClaim_ClientId",
                table: "ClientClaims",
                newName: "IX_ClientClaims_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "Resources",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Resources",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ConsentLifetime",
                table: "Clients",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Clients",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ResourceClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientUris",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientSecrets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientManagers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientEventHandlers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientEvents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClientClaims",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientClaims",
                table: "ClientClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientClaims_Clients_ClientId",
                table: "ClientClaims",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
