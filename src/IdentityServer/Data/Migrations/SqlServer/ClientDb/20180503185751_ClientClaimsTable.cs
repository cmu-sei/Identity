// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace IdentityServer.Data.Migrations.SqlServer.ClientDb
{
    public partial class ClientClaimsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientClaim_Clients_ClientId",
                table: "ClientClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientClaim",
                table: "ClientClaim");

            migrationBuilder.RenameTable(
                name: "ClientClaim",
                newName: "ClientClaims");

            migrationBuilder.RenameIndex(
                name: "IX_ClientClaim_ClientId",
                table: "ClientClaims",
                newName: "IX_ClientClaims_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientClaims",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientClaims_Clients_ClientId",
                table: "ClientClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientClaims",
                table: "ClientClaims");

            migrationBuilder.RenameTable(
                name: "ClientClaims",
                newName: "ClientClaim");

            migrationBuilder.RenameIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaim",
                newName: "IX_ClientClaim_ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientClaim",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientClaim",
                table: "ClientClaim",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientClaim_Clients_ClientId",
                table: "ClientClaim",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
