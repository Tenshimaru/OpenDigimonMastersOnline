using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigitalWorldOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeckBuff : Migration
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

            migrationBuilder.CreateTable(
                name: "DeckBookInfo",
                schema: "Asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Explain = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckBookInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeckBuff",
                schema: "Asset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupIdX = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    Explain = table.Column<string>(type: "varchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckBuff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeckBuffOption",
                schema: "Asset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupIdX = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    AtType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Option = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Value = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Prob = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Time = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeckBuffId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckBuffOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeckBuffOption_DeckBuff_DeckBuffId",
                        column: x => x.DeckBuffId,
                        principalSchema: "Asset",
                        principalTable: "DeckBuff",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBookInfo",
                columns: new[] { "Id", "Explain", "Name", "OptionId" },
                values: new object[] { 1L, "None.", "None", 0 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBookInfo",
                columns: new[] { "Id", "Explain", "Name", "OptionId", "Type" },
                values: new object[,]
                {
                    { 2L, "Additional damage to normal attack is applied.", "Additional damage to attack", 1, 1 },
                    { 3L, "Additional damage to skill attack is applied.", "Additional damage to skill", 2, 2 },
                    { 4L, "Increase critical hit damage.", "Increase critical hit damage", 3, 3 },
                    { 5L, "Reset skills' cooldown time when activated.", "Reset skill cooldown time", 4, 4 },
                    { 6L, "Increase digimon's Max HP.", "Increase Max HP", 5, 5 },
                    { 7L, "Increase normal attacking speed.", "Increase attack speed", 6, 6 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuff",
                columns: new[] { "Id", "Explain", "GroupIdX", "GroupName" },
                values: new object[,]
                {
                    { 1, "The Four Holy Beasts are four Ultimate digimons that reign over the north, south, east, and west of the digital world. Qinglongmon(Lightning), Xuanwumon(Water), Zhuqiaomon(Fire), and Baihumon(Iron), the Four Holy Beasts have, unlike other normal digimons, twelve cores and four eyes respectively.", 1001, "Four Holy Beasts" },
                    { 2, "Two or more Jogress digimons can be combined to evolve into an entirely different digimon. A digimon successfully jogressed has great power and an essential role in the digital world.", 1002, "Fusion to evolve into the higher" },
                    { 3, "They are the three great archangels who are the leaders of all the angelic digimons in the digital world. They are Seraphimon who executes god's law, Cherubimon who protects wisdom and god's area, Ophanimon who delivers god's love. Also, they lead angelic army consists of man-type, animal-type, and woman-type angelic digimons. Seraphimon is the highest of the three, and thus of all the angelic digimons.", 1004, "Three Archangels" },
                    { 4, "Digimon adventure's 8 main characters' partner digimons. Tai's partner Agumon, Matthew's partner Gabumon, Izzy's partner Tentomon, Kido Jou's partner Gomamon, T.K.'s partner Patamon, Mimi's Palmon, Sora's partner Biyomon, Kari's partner Gatomon are the members.", 1008, "Main characters of the adventure" },
                    { 5, "This is the group of the digimons with wings who have become Burst Mode digimons overcoming the power of Mega digimons.\nTheir wings influence all their actions from battle to moving.\nThe wings can be the shield of the Digital World full of good will,\nbut sometimes they can also be the blade raising the wind of darkness.", 1009, ">Burst Ultimate Wing" },
                    { 6, "The digimon looks like the food one can easily see in daily life.\nMushroom, clam, fish, squid,\nand pumpkin can make really great seafood stew if mixed harmoniously.\n※Jogress Food※\nEvolve into the best seafood stew!", 1010, "Seafood Stew" },
                    { 7, "This is the group of the digimons using big and nice blades to lead the battle.\nThey are usually masters of swords so that it's very beautiful to watch they play with their\nswords.\nAs the digimons walk only on the sword's way,\nthey commonly hold fast to their justice just like sharp blades.", 1011, "Sharp Blue Sword Wave" },
                    { 8, "This is the group of the digimons using firearms like guns and cannons to win battles.\nSome of them unleash stylish attacks with pistols,\nthe others deal heavy damage with huge cannon installed on their body.\nGun using digimons are usually cheerful, whereas\nCannon using digimons are taciturn.", 1012, "Instant Red Gun Fire" },
                    { 9, "This is the group of the digimons that you want to be with.\nWhen you come back home,  if one of the digimons from the deck welcomes you,\nit will be your happy day.\nWhen you are uncomfortable, you can be calmed if you touch their shaggy hair.", 1013, "So cute♥" },
                    { 10, "Appearing at the latter part of the digimon adventure, they are powerful dark forces who can compress all the areas into one place. Four dark masters have Ultimate digimons as servants.", 1016, "Four Dark Masters" },
                    { 11, "It is the final boss of Digimon Adventure\nand came from beyond the Wall of Fire to have the final fight with the Digidestined.\nIt wanted to help the world but it is a collective agent of the resentment of the Digimons that\ndied out and are left behind by evolution.\nThe egos of resentment disappeared in the darkness\nand decided to attack the Digital World to give the Digidestined and the other Digimons in the\nworld of light the same pain it suffered.", 1017, "Ego of the Darkness" },
                    { 12, "The Final Sprit Evolusion\nHyper Spirit Evolusion Digimon", 1019, "Hyper Spirit Evolution" },
                    { 13, "Digimon Group that Told of Oriental legends, strongest destructive god and the god which governs over regeneration, Susanoomon and Hyper Spirit Evolution Digimons.", 1020, "Ancient Spirit Evolution" },
                    { 14, "A group of legendary vaccine knights that declared to be natural enemy of evil virus Digimons.", 1021, "Legendary Knights of Vaccine" },
                    { 15, "Community of 3 different Holy Knights that are alike.", 1022, "OMEGA" },
                    { 16, "Community of 3 different Holy Knights that are alike and Digimon after transcendence.", 1023, "MEGA-OMEGA" }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 1, 1, null, 1001, 3, 30 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 2, 0, null, 1001 },
                    { 3, 0, null, 1001 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob" },
                values: new object[] { 4, 1, 2, null, 1002, 4, 700 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 5, 0, null, 1002 },
                    { 6, 0, null, 1002 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 7, 1, 3, null, 1004, 1, 300, 5, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 8, 0, null, 1004 },
                    { 9, 0, null, 1004 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 10, 1, null, 1008, 6, 8 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 11, 0, null, 1008 },
                    { 12, 0, null, 1008 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 13, 1, null, 1009, 6, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 14, 0, null, 1009 },
                    { 15, 0, null, 1009 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 16, 1, null, 1010, 5, 10 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 17, 0, null, 1010 },
                    { 18, 0, null, 1010 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 19, 1, 3, null, 1011, 1, 600, 10, 10 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 20, 0, null, 1011 },
                    { 21, 0, null, 1011 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 22, 2, 3, null, 1012, 2, 700, 12, 8 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 23, 0, null, 1012 },
                    { 24, 0, null, 1012 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 25, 1, null, 1013, 30, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 26, 0, null, 1013 },
                    { 27, 0, null, 1013 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 28, 2, 3, null, 1016, 2, 500, 10, 12 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 29, 0, null, 1016 },
                    { 30, 0, null, 1016 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 31, 1, 3, null, 1017, 1, 7000, 5, 12 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 32, 1, null, 1017, 5, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[] { 33, 0, null, 1017 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 34, 1, 3, null, 1019, 1, 5000, 5, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[,]
                {
                    { 35, 1, null, 1019, 5, 10 },
                    { 36, 1, null, 1019, 3, 2 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 37, 1, 3, null, 1020, 1, 4000, 5, 25 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[,]
                {
                    { 38, 1, null, 1020, 5, 15 },
                    { 39, 1, null, 1020, 3, 5 },
                    { 40, 1, null, 1021, 6, 15 },
                    { 41, 1, null, 1021, 5, 15 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 42, 1, 3, null, 1021, 1, 3000, 7, 20 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 43, 1, null, 1022, 2, 12 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX" },
                values: new object[,]
                {
                    { 44, 0, null, 1022 },
                    { 45, 0, null, 1022 }
                });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 46, 1, null, 1023, 2, 15 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "AtType", "Condition", "DeckBuffId", "GroupIdX", "Option", "Prob", "Time", "Value" },
                values: new object[] { 47, 1, 3, null, 1023, 3, 300, 10, 100 });

            migrationBuilder.InsertData(
                schema: "Asset",
                table: "DeckBuffOption",
                columns: new[] { "Id", "Condition", "DeckBuffId", "GroupIdX", "Option", "Value" },
                values: new object[] { 48, 1, null, 1023, 6, 15 });

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
                name: "IX_DeckBuff_GroupIdX",
                schema: "Asset",
                table: "DeckBuff",
                column: "GroupIdX",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeckBuffOption_DeckBuffId",
                schema: "Asset",
                table: "DeckBuffOption",
                column: "DeckBuffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeckBookInfo",
                schema: "Asset");

            migrationBuilder.DropTable(
                name: "DeckBuffOption",
                schema: "Asset");

            migrationBuilder.DropTable(
                name: "DeckBuff",
                schema: "Asset");

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
                value: new DateTime(2024, 11, 19, 9, 0, 55, 69, DateTimeKind.Local).AddTicks(6899));

            migrationBuilder.UpdateData(
                schema: "Routine",
                table: "Routine",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2024, 11, 19, 9, 0, 55, 77, DateTimeKind.Local).AddTicks(9986));
        }
    }
}
