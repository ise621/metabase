using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class AddReflexiveAssociationsToComponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                schema: "metabase",
                table: "component_concretization_and_generalization");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "metabase",
                table: "component_concretization_and_generalization");

            migrationBuilder.CreateTable(
                name: "component_assembly",
                schema: "metabase",
                columns: table => new
                {
                    AssembledComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartComponentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component_assembly", x => new { x.AssembledComponentId, x.PartComponentId });
                    table.ForeignKey(
                        name: "FK_component_assembly_component_AssembledComponentId",
                        column: x => x.AssembledComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_component_assembly_component_PartComponentId",
                        column: x => x.PartComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "component_variant",
                schema: "metabase",
                columns: table => new
                {
                    OfComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToComponentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component_variant", x => new { x.OfComponentId, x.ToComponentId });
                    table.ForeignKey(
                        name: "FK_component_variant_component_OfComponentId",
                        column: x => x.OfComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_component_variant_component_ToComponentId",
                        column: x => x.ToComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_component_assembly_PartComponentId",
                schema: "metabase",
                table: "component_assembly",
                column: "PartComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_component_variant_ToComponentId",
                schema: "metabase",
                table: "component_variant",
                column: "ToComponentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "component_assembly",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "component_variant",
                schema: "metabase");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "metabase",
                table: "component_concretization_and_generalization",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "xmin",
                schema: "metabase",
                table: "component_concretization_and_generalization",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}