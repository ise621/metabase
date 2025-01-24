using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class AddManagerRelationToInstitution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant")
                .OldAnnotation("Npgsql:Enum:institution_representative_role", "owner,maintainer,assistant");

            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                schema: "metabase",
                table: "institution_representative",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                schema: "metabase",
                table: "institution",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_ManagerId",
                schema: "metabase",
                table: "institution",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_institution_institution_ManagerId",
                schema: "metabase",
                table: "institution");

            migrationBuilder.DropIndex(
                name: "IX_institution_ManagerId",
                schema: "metabase",
                table: "institution");

            migrationBuilder.DropColumn(
                name: "Pending",
                schema: "metabase",
                table: "institution_representative");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "metabase",
                table: "institution");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:institution_representative_role", "owner,maintainer,assistant")
                .OldAnnotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant");
        }
    }
}