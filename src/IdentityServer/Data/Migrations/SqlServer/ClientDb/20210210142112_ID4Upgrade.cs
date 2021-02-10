using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.SqlServer.ClientDb
{
    public partial class ID4Upgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceClaim");

            migrationBuilder.AddColumn<string>(
                name: "Scopes",
                table: "Resources",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserClaims",
                table: "Resources",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiSecret",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Value = table.Column<string>(maxLength: 50, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSecret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiSecret_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiSecret_ResourceId",
                table: "ApiSecret",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiSecret");

            migrationBuilder.DropColumn(
                name: "Scopes",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "UserClaims",
                table: "Resources");

            migrationBuilder.CreateTable(
                name: "ResourceClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceClaim_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceClaim_ResourceId",
                table: "ResourceClaim",
                column: "ResourceId");
        }
    }
}
