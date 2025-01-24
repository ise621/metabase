using Microsoft.EntityFrameworkCore.Migrations;

namespace Metabase.Migrations
{
    public partial class UsePendingAndConfirmedAsInstitutionStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.institution_state", "pending,verified")
                .OldAnnotation("Npgsql:Enum:institution_state", "unknown,operative,inoperative");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:institution_state", "unknown,operative,inoperative")
                .OldAnnotation("Npgsql:Enum:public.institution_state", "pending,verified");
        }
    }
}