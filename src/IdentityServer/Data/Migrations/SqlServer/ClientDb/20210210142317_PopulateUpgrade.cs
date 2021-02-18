using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.SqlServer.ClientDb
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
                    "SET \"Type\" = LEFT(\"Value\", CHARINDEX('=', \"Value\") - 1), " +
                        "\"Value\" = RIGHT(\"Value\", CHARINDEX('=', REVERSE(\"Value\")) - 1) " +
                   "WHERE \"Type\" = '' AND " +
                        "CHARINDEX('=', \"Value\") > 1 AND " +
                        "CHARINDEX('=', \"Value\") < LEN(\"Value\") AND " +
                        "LEN(\"Value\") - LEN(REPLACE(\"Value\", '=', '')) = 1;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
