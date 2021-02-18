using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.PostgreSQL.ClientDb
{
    public partial class PopulateUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Populate new "Scopes" and "UserClaims" columns
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
            // Split "Value" on '=' to separate into "Type" and "Value" columns
			migrationBuilder.Sql(
                "UPDATE \"ClientClaim\" " +
                    "SET \"Type\" = SPLIT_PART(\"Value\", '=', 1), " +
                        "\"Value\" = SPLIT_PART(\"Value\", '=', 2) " +
                   "WHERE \"Type\" = '' AND SPLIT_PART(\"Value\", '=', 2) != '';"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
