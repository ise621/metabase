using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class RenameDataFormatsTableAndRemoveIdAndXminFromMethodDeveloperRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataFormats_institution_ManagerId",
                schema: "metabase",
                table: "DataFormats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataFormats",
                schema: "metabase",
                table: "DataFormats");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "metabase",
                table: "user_method_developer");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "metabase",
                table: "user_method_developer");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "metabase",
                table: "institution_method_developer");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "metabase",
                table: "institution_method_developer");

            migrationBuilder.RenameTable(
                name: "DataFormats",
                schema: "metabase",
                newName: "data_format",
                newSchema: "metabase");

            migrationBuilder.RenameIndex(
                name: "IX_DataFormats_ManagerId",
                schema: "metabase",
                table: "data_format",
                newName: "IX_data_format_ManagerId");

            migrationBuilder.AlterColumn<uint>(
                name: "xmin",
                schema: "metabase",
                table: "data_format",
                type: "xid",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "metabase",
                table: "data_format",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_data_format",
                schema: "metabase",
                table: "data_format",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_data_format_institution_ManagerId",
                schema: "metabase",
                table: "data_format");

            migrationBuilder.DropPrimaryKey(
                name: "PK_data_format",
                schema: "metabase",
                table: "data_format");

            migrationBuilder.RenameTable(
                name: "data_format",
                schema: "metabase",
                newName: "DataFormats",
                newSchema: "metabase");

            migrationBuilder.RenameIndex(
                name: "IX_data_format_ManagerId",
                schema: "metabase",
                table: "DataFormats",
                newName: "IX_DataFormats_ManagerId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "metabase",
                table: "user_method_developer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                schema: "metabase",
                table: "user_method_developer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "metabase",
                table: "institution_method_developer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                schema: "metabase",
                table: "institution_method_developer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "xmin",
                schema: "metabase",
                table: "DataFormats",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "xid",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "metabase",
                table: "DataFormats",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataFormats",
                schema: "metabase",
                table: "DataFormats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DataFormats_institution_ManagerId",
                schema: "metabase",
                table: "DataFormats",
                column: "ManagerId",
                principalSchema: "metabase",
                principalTable: "institution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}