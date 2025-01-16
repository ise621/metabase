using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimeSurfaceAndDirectionAndSurfaceLayersToComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Description",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimeDirection_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimeDirection_Reference_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "PrimeDirection_Reference_Publication_Authors",
                schema: "metabase",
                table: "component",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_Doi",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_Urn",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Locator",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Standardizer[]>(
                name: "PrimeDirection_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component",
                type: "metabase.standardizer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeDirection_Reference_Standard_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimeDirection_Reference_Standard_Year",
                schema: "metabase",
                table: "component",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Description",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimeSurface_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimeSurface_Reference_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "PrimeSurface_Reference_Publication_Authors",
                schema: "metabase",
                table: "component",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_Doi",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_Urn",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Locator",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Standardizer[]>(
                name: "PrimeSurface_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component",
                type: "metabase.standardizer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeSurface_Reference_Standard_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimeSurface_Reference_Standard_Year",
                schema: "metabase",
                table: "component",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Description",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SwitchableLayers_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SwitchableLayers_Reference_Exists",
                schema: "metabase",
                table: "component",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "SwitchableLayers_Reference_Publication_Authors",
                schema: "metabase",
                table: "component",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_Doi",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_Urn",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Locator",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Section",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Standardizer[]>(
                name: "SwitchableLayers_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component",
                type: "metabase.standardizer[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwitchableLayers_Reference_Standard_Title",
                schema: "metabase",
                table: "component",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SwitchableLayers_Reference_Standard_Year",
                schema: "metabase",
                table: "component",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimeDirection_Description",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Authors",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Doi",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_Urn",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Locator",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeDirection_Reference_Standard_Year",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Description",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Authors",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Doi",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_Urn",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Locator",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "PrimeSurface_Reference_Standard_Year",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Description",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Exists",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_ArXiv",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Authors",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Doi",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_Urn",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Publication_WebAddress",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Abstract",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Locator",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Numeration_MainNumber",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Numeration_Prefix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Numeration_Suffix",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Section",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Standardizers",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Title",
                schema: "metabase",
                table: "component");

            migrationBuilder.DropColumn(
                name: "SwitchableLayers_Reference_Standard_Year",
                schema: "metabase",
                table: "component");
        }
    }
}
