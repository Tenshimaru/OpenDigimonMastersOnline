using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWorldOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TEST22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_DigimonBaseInfo_Type",
                schema: "Asset",
                table: "DigimonBaseInfo",
                column: "Type");

            migrationBuilder.CreateTable(
                name: "Encyclopedia",
                schema: "Character",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    DigimonEvolutionId = table.Column<long>(type: "bigint", nullable: false),
                    IsRewardAllowed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRewardReceived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encyclopedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encyclopedia_Evolution_DigimonEvolutionId",
                        column: x => x.DigimonEvolutionId,
                        principalSchema: "Asset",
                        principalTable: "Evolution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Encyclopedia_Tamer_CharacterId",
                        column: x => x.CharacterId,
                        principalSchema: "Character",
                        principalTable: "Tamer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncyclopediaEvolutions",
                schema: "Character",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterEncyclopediaId = table.Column<long>(type: "bigint", nullable: false),
                    DigimonBaseType = table.Column<int>(type: "int", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncyclopediaEvolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncyclopediaEvolutions_DigimonBaseInfo_DigimonBaseType",
                        column: x => x.DigimonBaseType,
                        principalSchema: "Asset",
                        principalTable: "DigimonBaseInfo",
                        principalColumn: "Type",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EncyclopediaEvolutions_Encyclopedia_CharacterEncyclopediaId",
                        column: x => x.CharacterEncyclopediaId,
                        principalSchema: "Character",
                        principalTable: "Encyclopedia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Encyclopedia_CharacterId",
                schema: "Character",
                table: "Encyclopedia",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Encyclopedia_DigimonEvolutionId",
                schema: "Character",
                table: "Encyclopedia",
                column: "DigimonEvolutionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EncyclopediaEvolutions_CharacterEncyclopediaId",
                schema: "Character",
                table: "EncyclopediaEvolutions",
                column: "CharacterEncyclopediaId");

            migrationBuilder.CreateIndex(
                name: "IX_EncyclopediaEvolutions_DigimonBaseType",
                schema: "Character",
                table: "EncyclopediaEvolutions",
                column: "DigimonBaseType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncyclopediaEvolutions",
                schema: "Character");

            migrationBuilder.DropTable(
                name: "Encyclopedia",
                schema: "Character");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DigimonBaseInfo_Type",
                schema: "Asset",
                table: "DigimonBaseInfo");
        }
    }
}
