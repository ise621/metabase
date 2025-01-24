using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class AddPendingColumnToComponentManufacturer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                schema: "metabase",
                table: "component_manufacturer",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending",
                schema: "metabase",
                table: "component_manufacturer");
        }
    }
}