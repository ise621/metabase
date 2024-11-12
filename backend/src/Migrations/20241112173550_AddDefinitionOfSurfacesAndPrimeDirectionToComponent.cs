using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class AddDefinitionOfSurfacesAndPrimeDirectionToComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Description",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ab~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ar~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Au~",
                schema: "metabase",
                table: "component",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Doi",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Se~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ti~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Urn",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_We~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Abstr~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Locat~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Numer~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Nume~1",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Nume~2",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Secti~",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Standardizer[]>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Stand~",
                schema: "metabase",
                table: "component",
                type: "metabase.standardizer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Year",
                schema: "metabase",
                table: "component",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Description",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ab~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ar~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Au~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Doi",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Se~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Ti~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_Urn",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Publication_We~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Abstr~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Locat~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Numer~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Nume~1",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Nume~2",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Secti~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Stand~",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "DefinitionOfSurfacesAndPrimeDirection_Reference_Standard_Year",
                schema: "metabase",
                table: "component");
        }
    }
}
