using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class PopulateScopesColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Resources\" SET \"Scopes\" = CASE \"Name\" " +
                    "WHEN 'role' THEN 'role' " +
                    "WHEN 'organization' THEN 'org orgunit picture_o picture_ou' " +
                    "WHEN 'profile' THEN 'name family_name given_name username picture updated_at affiliate' " +
                    "WHEN 'email' THEN 'email email_verified' " +
                    "WHEN 'openid' THEN 'sub' " +
                    "ELSE \"Name\" END;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
