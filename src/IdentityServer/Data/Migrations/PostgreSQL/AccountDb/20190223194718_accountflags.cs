// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.PostgreSQL.AccountDb
{
    public partial class accountflags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Accounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Accounts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'bio' WHERE \"Key\" = 'biography';");
            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'org' WHERE \"Key\" = 'company';");
            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'orgunit' WHERE \"Key\" = 'unit';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Accounts");

            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'biography' WHERE \"Key\" = 'bio';");
            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'company' WHERE \"Key\" = 'org';");
            migrationBuilder.Sql("UPDATE \"AccountProperties\" SET \"Key\" = 'unit' WHERE \"Key\" = 'orgunit';");
        }
    }
}
