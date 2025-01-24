using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationStateAndCodeToDatabaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.database_verification_state", "pending,verified");

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                schema: "metabase",
                table: "database",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DatabaseVerificationState>(
                name: "VerificationState",
                schema: "metabase",
                table: "database",
                type: "database_verification_state",
                nullable: false,
                defaultValue: DatabaseVerificationState.PENDING);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationCode",
                schema: "metabase",
                table: "database");

            migrationBuilder.DropColumn(
                name: "VerificationState",
                schema: "metabase",
                table: "database");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.database_verification_state", "pending,verified");
        }
    }
}