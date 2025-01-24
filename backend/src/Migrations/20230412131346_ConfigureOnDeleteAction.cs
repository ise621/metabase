using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureOnDeleteAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format");

            migrationBuilder.DropForeignKey(
                name: "FK_database_institution_OperatorId",
                schema: "metabase",
                table: "database");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution");

            migrationBuilder.DropForeignKey(
                name: "FK_method_institution_ManagerId",
                schema: "metabase",
                table: "method");

            migrationBuilder.AddForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_database_institution_OperatorId",
                schema: "metabase",
                table: "database",
                column: "OperatorId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_method_institution_ManagerId",
                schema: "metabase",
                table: "method",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format");

            migrationBuilder.DropForeignKey(
                name: "FK_database_institution_OperatorId",
                schema: "metabase",
                table: "database");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution");

            migrationBuilder.DropForeignKey(
                name: "FK_method_institution_ManagerId",
                schema: "metabase",
                table: "method");

            migrationBuilder.AddForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_database_institution_OperatorId",
                schema: "metabase",
                table: "database",
                column: "OperatorId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_method_institution_ManagerId",
                schema: "metabase",
                table: "method",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}