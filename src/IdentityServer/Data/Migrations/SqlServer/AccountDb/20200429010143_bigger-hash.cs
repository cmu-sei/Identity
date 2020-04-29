// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.SqlServer.AccountDb
{
    public partial class biggerhash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""AccountProperties"" SET ""Key"" = 'picture_o' where ""Key"" = 'org';");
            migrationBuilder.Sql(@"UPDATE ""AccountProperties"" SET ""Key"" = 'picture_ou' where ""Key"" = 'orgunit';");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "AccountTokens",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "AccountTokens",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);
        }
    }
}
