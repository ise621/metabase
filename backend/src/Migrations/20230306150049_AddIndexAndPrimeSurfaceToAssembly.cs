using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexAndPrimeSurfaceToAssembly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.prime_surface", "inside,outside");

            migrationBuilder.AddColumn<byte>(
                name: "Index",
                schema: "metabase",
                table: "component_assembly",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<PrimeSurface>(
                name: "PrimeSurface",
                schema: "metabase",
                table: "component_assembly",
                type: "prime_surface",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                schema: "metabase",
                table: "component_assembly");

            migrationBuilder.DropColumn(
                name: "PrimeSurface",
                schema: "metabase",
                table: "component_assembly");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.prime_surface", "inside,outside");
        }
    }
}