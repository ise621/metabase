using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class AddOperatingState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .Annotation("Npgsql:Enum:public.database_verification_state", "pending,verified")
                .Annotation("Npgsql:Enum:public.institution_operating_state", "operating,not_operating")
                .Annotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant")
                .Annotation("Npgsql:Enum:public.institution_state", "pending,verified")
                .Annotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .Annotation("Npgsql:Enum:public.prime_surface", "inside,outside")
                .Annotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .OldAnnotation("Npgsql:Enum:public.database_verification_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant")
                .OldAnnotation("Npgsql:Enum:public.institution_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .OldAnnotation("Npgsql:Enum:public.prime_surface", "inside,outside")
                .OldAnnotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.AddColumn<InstitutionOperatingState>(
                name: "OperatingState",
                schema: "metabase",
                table: "institution",
                type: "institution_operating_state",
                nullable: false,
                defaultValue: InstitutionOperatingState.OPERATING);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperatingState",
                schema: "metabase",
                table: "institution");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .Annotation("Npgsql:Enum:public.database_verification_state", "pending,verified")
                .Annotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant")
                .Annotation("Npgsql:Enum:public.institution_state", "pending,verified")
                .Annotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .Annotation("Npgsql:Enum:public.prime_surface", "inside,outside")
                .Annotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .OldAnnotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .OldAnnotation("Npgsql:Enum:public.database_verification_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:public.institution_operating_state", "operating,not_operating")
                .OldAnnotation("Npgsql:Enum:public.institution_representative_role", "owner,assistant")
                .OldAnnotation("Npgsql:Enum:public.institution_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .OldAnnotation("Npgsql:Enum:public.prime_surface", "inside,outside")
                .OldAnnotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");
        }
    }
}