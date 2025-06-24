using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalWorldOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Map_MapId",
                schema: "Config",
                table: "Map",
                column: "MapId");

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    StartDay = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartsAt = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventMaps",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventConfigId = table.Column<long>(type: "bigint", nullable: false),
                    MapId = table.Column<int>(type: "int", nullable: false),
                    Channels = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMaps_Events_EventConfigId",
                        column: x => x.EventConfigId,
                        principalSchema: "Config",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventMaps_Map_MapId",
                        column: x => x.MapId,
                        principalSchema: "Config",
                        principalTable: "Map",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMob",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    ViewRange = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HuntRange = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Class = table.Column<int>(type: "int", nullable: false),
                    ReactionType = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Attribute = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Element = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Family1 = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Family2 = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Family3 = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RespawnInterval = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    EventMapConfigId = table.Column<long>(type: "bigint", nullable: false),
                    DeathTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ResurrectionTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ASValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ARValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ATValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BLValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CTValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DEValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DSValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EVValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HPValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HTValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MSValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    WSValue = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMob_EventMaps_EventMapConfigId",
                        column: x => x.EventMapConfigId,
                        principalSchema: "Config",
                        principalTable: "EventMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMobDropReward",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinAmount = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    MaxAmount = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    MobId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMobDropReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMobDropReward_EventMob_MobId",
                        column: x => x.MobId,
                        principalSchema: "Config",
                        principalTable: "EventMob",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMobExpReward",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TamerExperience = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    DigimonExperience = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    NatureExperience = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    ElementExperience = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    SkillExperience = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    MobId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMobExpReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMobExpReward_EventMob_MobId",
                        column: x => x.MobId,
                        principalSchema: "Config",
                        principalTable: "EventMob",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventMobLocation",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MobConfigId = table.Column<long>(type: "bigint", nullable: false),
                    MapId = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    X = table.Column<int>(type: "int", nullable: false, defaultValue: 5000),
                    Y = table.Column<int>(type: "int", nullable: false, defaultValue: 4500),
                    Z = table.Column<decimal>(type: "numeric(9,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventMobLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventMobLocation_EventMob_MobConfigId",
                        column: x => x.MobConfigId,
                        principalSchema: "Config",
                        principalTable: "EventMob",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventBitsDropReward",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinAmount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxAmount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Chance = table.Column<decimal>(type: "numeric(9,2)", nullable: false, defaultValue: 0m),
                    DropRewardId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBitsDropReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBitsDropReward_EventMobDropReward_DropRewardId",
                        column: x => x.DropRewardId,
                        principalSchema: "Config",
                        principalTable: "EventMobDropReward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventItemDropReward",
                schema: "Config",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MinAmount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MaxAmount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Chance = table.Column<decimal>(type: "numeric(9,2)", nullable: false, defaultValue: 0m),
                    Rank = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DropRewardId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventItemDropReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventItemDropReward_EventMobDropReward_DropRewardId",
                        column: x => x.DropRewardId,
                        principalSchema: "Config",
                        principalTable: "EventMobDropReward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "Config",
                table: "Hash",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 25, 2, 9, 44, 323, DateTimeKind.Local).AddTicks(8133));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "NextRunTime" },
                values: new object[] { new DateTime(2024, 11, 25, 2, 9, 44, 337, DateTimeKind.Local).AddTicks(1554), new DateTime(2024, 11, 26, 0, 0, 0, 0, DateTimeKind.Local) });

            migrationBuilder.CreateIndex(
                name: "IX_EventBitsDropReward_DropRewardId",
                schema: "Config",
                table: "EventBitsDropReward",
                column: "DropRewardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventItemDropReward_DropRewardId",
                schema: "Config",
                table: "EventItemDropReward",
                column: "DropRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMaps_EventConfigId",
                schema: "Config",
                table: "EventMaps",
                column: "EventConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMaps_MapId",
                schema: "Config",
                table: "EventMaps",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMob_EventMapConfigId",
                schema: "Config",
                table: "EventMob",
                column: "EventMapConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMobDropReward_MobId",
                schema: "Config",
                table: "EventMobDropReward",
                column: "MobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventMobExpReward_MobId",
                schema: "Config",
                table: "EventMobExpReward",
                column: "MobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventMobLocation_MobConfigId",
                schema: "Config",
                table: "EventMobLocation",
                column: "MobConfigId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBitsDropReward",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventItemDropReward",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventMobExpReward",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventMobLocation",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventMobDropReward",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventMob",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "EventMaps",
                schema: "Config");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "Config");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Map_MapId",
                schema: "Config",
                table: "Map");

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
                value: new DateTime(2024, 11, 23, 14, 57, 2, 349, DateTimeKind.Local).AddTicks(1812));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "NextRunTime" },
                values: new object[] { new DateTime(2024, 11, 23, 14, 57, 2, 358, DateTimeKind.Local).AddTicks(5767), new DateTime(2024, 11, 24, 0, 0, 0, 0, DateTimeKind.Local) });
        }
    }
}
