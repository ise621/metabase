using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeToNet9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:metabase.component_category", "layer,material,unit")
                .Annotation("Npgsql:Enum:metabase.institution_operating_state", "not_operating,operating")
                .Annotation("Npgsql:Enum:metabase.institution_representative_role", "assistant,owner")
                .Annotation("Npgsql:Enum:metabase.method_category", "calculation,measurement")
                .OldAnnotation("Npgsql:Enum:metabase.component_category", "material,layer,unit")
                .OldAnnotation("Npgsql:Enum:metabase.institution_operating_state", "operating,not_operating")
                .OldAnnotation("Npgsql:Enum:metabase.institution_representative_role", "owner,assistant")
                .OldAnnotation("Npgsql:Enum:metabase.method_category", "measurement,calculation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:metabase.component_category", "material,layer,unit")
                .Annotation("Npgsql:Enum:metabase.institution_operating_state", "operating,not_operating")
                .Annotation("Npgsql:Enum:metabase.institution_representative_role", "owner,assistant")
                .Annotation("Npgsql:Enum:metabase.method_category", "measurement,calculation")
                .OldAnnotation("Npgsql:Enum:metabase.component_category", "layer,material,unit")
                .OldAnnotation("Npgsql:Enum:metabase.institution_operating_state", "not_operating,operating")
                .OldAnnotation("Npgsql:Enum:metabase.institution_representative_role", "assistant,owner")
                .OldAnnotation("Npgsql:Enum:metabase.method_category", "calculation,measurement");
        }
    }
}