using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.SqlServer.AccountDb
{
    public partial class ipv6length : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WhereLastAuthenticated",
                table: "Accounts",
                maxLength: 48,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhereAuthenticated",
                table: "Accounts",
                maxLength: 48,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WhereLastAuthenticated",
                table: "Accounts",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 48,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhereAuthenticated",
                table: "Accounts",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 48,
                oldNullable: true);
        }
    }
}
