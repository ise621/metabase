using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class AddMethodManagerOneToManyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                schema: "metabase",
                table: "user_method_developer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                schema: "metabase",
                table: "method",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                schema: "metabase",
                table: "institution_method_developer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_method_ManagerId",
                schema: "metabase",
                table: "method",
                column: "ManagerId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_method_institution_ManagerId",
                schema: "metabase",
                table: "method");

            migrationBuilder.DropIndex(
                name: "IX_method_ManagerId",
                schema: "metabase",
                table: "method");

            migrationBuilder.DropColumn(
                name: "Pending",
                schema: "metabase",
                table: "user_method_developer");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "metabase",
                table: "method");

            migrationBuilder.DropColumn(
                name: "Pending",
                schema: "metabase",
                table: "institution_method_developer");
        }
    }
}