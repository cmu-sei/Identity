using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class PopulateScopesColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Resources\" SET \"Scopes\" = \"Name\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
