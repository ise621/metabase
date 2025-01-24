using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeNpqsqlToVersion7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RedemptionDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                schema: "metabase",
                table: "OpenIddictAuthorizations",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Validity",
                schema: "metabase",
                table: "method",
                type: "tstzrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tsrange",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Availability",
                schema: "metabase",
                table: "method",
                type: "tstzrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tsrange",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Availability",
                schema: "metabase",
                table: "component",
                type: "tstzrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tsrange",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RedemptionDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                schema: "metabase",
                table: "OpenIddictTokens",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                schema: "metabase",
                table: "OpenIddictAuthorizations",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Validity",
                schema: "metabase",
                table: "method",
                type: "tsrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tstzrange",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Availability",
                schema: "metabase",
                table: "method",
                type: "tsrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tstzrange",
                oldNullable: true);

            migrationBuilder.AlterColumn<NpgsqlRange<DateTime>>(
                name: "Availability",
                schema: "metabase",
                table: "component",
                type: "tsrange",
                nullable: true,
                oldClrType: typeof(NpgsqlRange<DateTime>),
                oldType: "tstzrange",
                oldNullable: true);
        }
    }
}