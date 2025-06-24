using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWorldOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Encyclopedia_DigimonEvolutionId",
                schema: "Character",
                table: "Encyclopedia");
            try
            {
                migrationBuilder.AlterColumn<decimal>(
                    name: "WOSCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "WISCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "WASCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "THSCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "STSCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "SCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "NESCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "LISCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "LASCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "ICSCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "FISCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.AlterColumn<decimal>(
                    name: "DASCD",
                    schema: "Asset",
                    table: "TitleStatus",
                    type: "numeric(38,17)",
                    nullable: false,
                    oldClrType: typeof(decimal),
                    oldType: "numeric(18,0)");

                migrationBuilder.UpdateData(
                    schema: "Config",
                    table: "Hash",
                    keyColumn: "Id",
                    keyValue: 1L,
                    column: "CreatedAt",
                    value: new DateTime(2024, 10, 23, 6, 18, 40, 575, DateTimeKind.Local).AddTicks(8653));

                migrationBuilder.UpdateData(
                    schema: "Routine",
                    table: "Routine",
                    keyColumn: "Id",
                    keyValue: 1L,
                    column: "CreatedAt",
                    value: new DateTime(2024, 10, 23, 6, 18, 40, 581, DateTimeKind.Local).AddTicks(9805));
            }
            catch
            {

            }

            migrationBuilder.CreateIndex(
                name: "IX_Encyclopedia_DigimonEvolutionId",
                schema: "Character",
                table: "Encyclopedia",
                column: "DigimonEvolutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Encyclopedia_DigimonEvolutionId",
                schema: "Character",
                table: "Encyclopedia");

            migrationBuilder.AlterColumn<decimal>(
                name: "WOSCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WISCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WASCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "THSCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "STSCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NESCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LISCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LASCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ICSCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FISCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DASCD",
                schema: "Asset",
                table: "TitleStatus",
                type: "numeric(18,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,17)");

            migrationBuilder.UpdateData(
                schema: "Config",
                table: "Hash",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 10, 23, 6, 1, 48, 285, DateTimeKind.Local).AddTicks(9961));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 10, 23, 6, 1, 48, 292, DateTimeKind.Local).AddTicks(1934));

            migrationBuilder.CreateIndex(
                name: "IX_Encyclopedia_DigimonEvolutionId",
                schema: "Character",
                table: "Encyclopedia",
                column: "DigimonEvolutionId",
                unique: true);
        }
    }
}
