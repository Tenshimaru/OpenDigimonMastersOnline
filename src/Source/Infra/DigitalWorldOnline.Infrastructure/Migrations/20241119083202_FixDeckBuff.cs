using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWorldOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDeckBuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckBuffOption_DeckBuff_DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropIndex(
                name: "IX_DeckBuffOption_DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropColumn(
                name: "DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropColumn(
                name: "Option",
                schema: "Asset",
                table: "DeckBuffOption");

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

            migrationBuilder.AddColumn<int>(
                name: "OptionId",
                schema: "Asset",
                table: "DeckBuffOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DeckBuff_GroupIdX",
                schema: "Asset",
                table: "DeckBuff",
                column: "GroupIdX");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DeckBookInfo_OptionId",
                schema: "Asset",
                table: "DeckBookInfo",
                column: "OptionId");

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 1,
                column: "OptionId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 2,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 3,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 4,
                column: "OptionId",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 5,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 6,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 7,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 8,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 9,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 10,
                column: "OptionId",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 11,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 12,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 13,
                column: "OptionId",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 14,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 15,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 16,
                column: "OptionId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 17,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 18,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 19,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 20,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 21,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 22,
                column: "OptionId",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 23,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 24,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 25,
                column: "OptionId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 26,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 27,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 28,
                column: "OptionId",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 29,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 30,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 31,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 32,
                column: "OptionId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 33,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 34,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 35,
                column: "OptionId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 36,
                column: "OptionId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 37,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 38,
                column: "OptionId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 39,
                column: "OptionId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 40,
                column: "OptionId",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 41,
                column: "OptionId",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 42,
                column: "OptionId",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 43,
                column: "OptionId",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 44,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 45,
                column: "OptionId",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 46,
                column: "OptionId",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 47,
                column: "OptionId",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 48,
                column: "OptionId",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "Config",
                table: "Hash",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 19, 10, 32, 1, 334, DateTimeKind.Local).AddTicks(6265));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 19, 10, 32, 1, 341, DateTimeKind.Local).AddTicks(1845));

            migrationBuilder.CreateIndex(
                name: "IX_Tamer_DeckBuffId",
                schema: "Character",
                table: "Tamer",
                column: "DeckBuffId");

            migrationBuilder.CreateIndex(
                name: "IX_DeckBuffOption_GroupIdX",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "GroupIdX");

            migrationBuilder.CreateIndex(
                name: "IX_DeckBuffOption_OptionId",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckBuffOption_DeckBookInfo_OptionId",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "OptionId",
                principalSchema: "Asset",
                principalTable: "DeckBookInfo",
                principalColumn: "OptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeckBuffOption_DeckBuff_GroupIdX",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "GroupIdX",
                principalSchema: "Asset",
                principalTable: "DeckBuff",
                principalColumn: "GroupIdX",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tamer_DeckBuff_DeckBuffId",
                schema: "Character",
                table: "Tamer",
                column: "DeckBuffId",
                principalSchema: "Asset",
                principalTable: "DeckBuff",
                principalColumn: "GroupIdX");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeckBuffOption_DeckBookInfo_OptionId",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropForeignKey(
                name: "FK_DeckBuffOption_DeckBuff_GroupIdX",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropForeignKey(
                name: "FK_Tamer_DeckBuff_DeckBuffId",
                schema: "Character",
                table: "Tamer");

            migrationBuilder.DropIndex(
                name: "IX_Tamer_DeckBuffId",
                schema: "Character",
                table: "Tamer");

            migrationBuilder.DropIndex(
                name: "IX_DeckBuffOption_GroupIdX",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropIndex(
                name: "IX_DeckBuffOption_OptionId",
                schema: "Asset",
                table: "DeckBuffOption");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DeckBuff_GroupIdX",
                schema: "Asset",
                table: "DeckBuff");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DeckBookInfo_OptionId",
                schema: "Asset",
                table: "DeckBookInfo");

            migrationBuilder.DropColumn(
                name: "OptionId",
                schema: "Asset",
                table: "DeckBuffOption");

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

            migrationBuilder.AddColumn<int>(
                name: "DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Option",
                schema: "Asset",
                table: "DeckBuffOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 3 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 2,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 3,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 4 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 5,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 6,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 8,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 9,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 6 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 11,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 12,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 6 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 14,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 15,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 17,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 18,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 20,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 21,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 23,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 24,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 30 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 26,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 27,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 29,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 30,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 33,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 3 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 3 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 6 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 5 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 1 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 44,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 45,
                column: "DeckBuffId",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 3 });

            migrationBuilder.UpdateData(
                schema: "Asset",
                table: "DeckBuffOption",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "DeckBuffId", "Option" },
                values: new object[] { null, 6 });

            migrationBuilder.UpdateData(
                schema: "Config",
                table: "Hash",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 19, 9, 4, 39, 597, DateTimeKind.Local).AddTicks(1490));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 19, 9, 4, 39, 618, DateTimeKind.Local).AddTicks(9161));

            migrationBuilder.CreateIndex(
                name: "IX_DeckBuffOption_DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "DeckBuffId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeckBuffOption_DeckBuff_DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "DeckBuffId",
                principalSchema: "Asset",
                principalTable: "DeckBuff",
                principalColumn: "Id");
        }
    }
}
