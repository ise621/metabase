using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class MoveEnumTypesToMetabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:metabase.component_category", "material,layer,unit")
                .Annotation("Npgsql:Enum:metabase.database_verification_state", "pending,verified")
                .Annotation("Npgsql:Enum:metabase.institution_operating_state", "operating,not_operating")
                .Annotation("Npgsql:Enum:metabase.institution_representative_role", "owner,assistant")
                .Annotation("Npgsql:Enum:metabase.institution_state", "pending,verified")
                .Annotation("Npgsql:Enum:metabase.method_category", "measurement,calculation")
                .Annotation("Npgsql:Enum:metabase.prime_surface", "inside,outside")
                .Annotation("Npgsql:Enum:metabase.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
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

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "metabase",
                table: "method",
                type: "metabase.standardizer[]",
                nullable: true,
                oldClrType: typeof(Standardizer[]),
                oldType: "standardizer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<MethodCategory[]>(
                name: "Categories",
                schema: "metabase",
                table: "method",
                type: "metabase.method_category[]",
                nullable: false,
                oldClrType: typeof(MethodCategory[]),
                oldType: "method_category[]");

            migrationBuilder.AlterColumn<InstitutionRepresentativeRole>(
                name: "Role",
                schema: "metabase",
                table: "institution_representative",
                type: "metabase.institution_representative_role",
                nullable: false,
                oldClrType: typeof(InstitutionRepresentativeRole),
                oldType: "institution_representative_role");

            migrationBuilder.AlterColumn<InstitutionState>(
                name: "State",
                schema: "metabase",
                table: "institution",
                type: "metabase.institution_state",
                nullable: false,
                oldClrType: typeof(InstitutionState),
                oldType: "institution_state");

            migrationBuilder.AlterColumn<InstitutionOperatingState>(
                name: "OperatingState",
                schema: "metabase",
                table: "institution",
                type: "metabase.institution_operating_state",
                nullable: false,
                oldClrType: typeof(InstitutionOperatingState),
                oldType: "institution_operating_state");

            migrationBuilder.AlterColumn<DatabaseVerificationState>(
                name: "VerificationState",
                schema: "metabase",
                table: "database",
                type: "metabase.database_verification_state",
                nullable: false,
                oldClrType: typeof(DatabaseVerificationState),
                oldType: "database_verification_state");

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "metabase",
                table: "data_format",
                type: "metabase.standardizer[]",
                nullable: true,
                oldClrType: typeof(Standardizer[]),
                oldType: "standardizer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<PrimeSurface>(
                name: "PrimeSurface",
                schema: "metabase",
                table: "component_assembly",
                type: "metabase.prime_surface",
                nullable: true,
                oldClrType: typeof(PrimeSurface),
                oldType: "prime_surface",
                oldNullable: true);

            migrationBuilder.AlterColumn<ComponentCategory[]>(
                name: "Categories",
                schema: "metabase",
                table: "component",
                type: "metabase.component_category[]",
                nullable: false,
                oldClrType: typeof(ComponentCategory[]),
                oldType: "component_category[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                .OldAnnotation("Npgsql:Enum:metabase.component_category", "material,layer,unit")
                .OldAnnotation("Npgsql:Enum:metabase.database_verification_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:metabase.institution_operating_state", "operating,not_operating")
                .OldAnnotation("Npgsql:Enum:metabase.institution_representative_role", "owner,assistant")
                .OldAnnotation("Npgsql:Enum:metabase.institution_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:metabase.method_category", "measurement,calculation")
                .OldAnnotation("Npgsql:Enum:metabase.prime_surface", "inside,outside")
                .OldAnnotation("Npgsql:Enum:metabase.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "metabase",
                table: "method",
                type: "standardizer[]",
                nullable: true,
                oldClrType: typeof(Standardizer[]),
                oldType: "metabase.standardizer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<MethodCategory[]>(
                name: "Categories",
                schema: "metabase",
                table: "method",
                type: "method_category[]",
                nullable: false,
                oldClrType: typeof(MethodCategory[]),
                oldType: "metabase.method_category[]");

            migrationBuilder.AlterColumn<InstitutionRepresentativeRole>(
                name: "Role",
                schema: "metabase",
                table: "institution_representative",
                type: "institution_representative_role",
                nullable: false,
                oldClrType: typeof(InstitutionRepresentativeRole),
                oldType: "metabase.institution_representative_role");

            migrationBuilder.AlterColumn<InstitutionState>(
                name: "State",
                schema: "metabase",
                table: "institution",
                type: "institution_state",
                nullable: false,
                oldClrType: typeof(InstitutionState),
                oldType: "metabase.institution_state");

            migrationBuilder.AlterColumn<InstitutionOperatingState>(
                name: "OperatingState",
                schema: "metabase",
                table: "institution",
                type: "institution_operating_state",
                nullable: false,
                oldClrType: typeof(InstitutionOperatingState),
                oldType: "metabase.institution_operating_state");

            migrationBuilder.AlterColumn<DatabaseVerificationState>(
                name: "VerificationState",
                schema: "metabase",
                table: "database",
                type: "database_verification_state",
                nullable: false,
                oldClrType: typeof(DatabaseVerificationState),
                oldType: "metabase.database_verification_state");

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "metabase",
                table: "data_format",
                type: "standardizer[]",
                nullable: true,
                oldClrType: typeof(Standardizer[]),
                oldType: "metabase.standardizer[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<PrimeSurface>(
                name: "PrimeSurface",
                schema: "metabase",
                table: "component_assembly",
                type: "prime_surface",
                nullable: true,
                oldClrType: typeof(PrimeSurface),
                oldType: "metabase.prime_surface",
                oldNullable: true);

            migrationBuilder.AlterColumn<ComponentCategory[]>(
                name: "Categories",
                schema: "metabase",
                table: "component",
                type: "component_category[]",
                nullable: false,
                oldClrType: typeof(ComponentCategory[]),
                oldType: "metabase.component_category[]");
        }
    }
}