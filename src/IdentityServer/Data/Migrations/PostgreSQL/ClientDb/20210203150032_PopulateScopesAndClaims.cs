using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class PopulateScopesAndClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Resources\" SET \"Scopes\" = \"Name\", " +
                    "\"UserClaims\" = CASE \"Name\" " +
                    "WHEN 'role' THEN 'role' " +
                    "WHEN 'organization' THEN 'org orgunit picture_o picture_ou' " +
                    "WHEN 'profile' THEN 'name family_name given_name username picture updated_at affiliate' " +
                    "WHEN 'email' THEN 'email email_verified' " +
                    "WHEN 'openid' THEN 'sub' " +
                    "ELSE NULL END;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
