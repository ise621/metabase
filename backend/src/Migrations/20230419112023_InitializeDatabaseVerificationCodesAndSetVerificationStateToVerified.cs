using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class InitializeDatabaseVerificationCodesAndSetVerificationStateToVerified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // [Manual migration customization](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding#manual-migration-customization)
            // [Change data in migration Up method](https://stackoverflow.com/questions/33516705/change-data-in-migration-up-method-entity-framework/33516868#33516868)
            migrationBuilder.Sql(@"UPDATE metabase.database SET ""VerificationState"" = 'verified';");
            // migrationBuilder.UpdateData(
            //     table: "database", 
            //     keyColumn: "Id", 
            //     keyValue: null,
            //     column: "VerificationCode", 
            //     value: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE metabase.database SET ""VerificationState"" = 'pending';");
        }
    }
}