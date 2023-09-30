using System;

using Microsoft.EntityFrameworkCore.Migrations;

// ReSharper disable PossibleNullReferenceException

namespace Intersect.Server.Migrations
{

    public partial class StandardizeVocabulary1 : Migration
    {

        private static void CreateNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LastOnline = table.Column<DateTime>(nullable: true),
                    MapId = table.Column<Guid>(nullable: false),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Z = table.Column<int>(nullable: false),
                    Dir = table.Column<int>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Sprite = table.Column<string>(nullable: true),
                    Face = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Exp = table.Column<long>(nullable: false),
                    ClassId = table.Column<Guid>(nullable: false),
                    Vitals = table.Column<string>(nullable: true),
                    StatPoints = table.Column<int>(nullable: false),
                    BaseStats = table.Column<string>(nullable: true),
                    StatPointAllocations = table.Column<string>(nullable: true),
                    Equipment = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_UserId", column: x => x.UserId, principalTable: "Users",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Bank", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Bank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Bank_Bags_BagId", column: x => x.BagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Player_Bank_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Friends", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: true),
                    TargetId = table.Column<Guid>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Friends_Players_OwnerId", column: x => x.OwnerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Player_Friends_Players_TargetId", column: x => x.TargetId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Hotbar", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    ItemOrSpellId = table.Column<Guid>(nullable: false),
                    BagId = table.Column<Guid>(nullable: false),
                    PreferredStatBuffs = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Hotbar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Hotbar_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Items", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Items_Bags_BagId", column: x => x.BagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Player_Items_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Quests", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    QuestId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    TaskProgress = table.Column<int>(nullable: false),
                    Completed = table.Column<bool>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Quests_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Spells", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    SpellId = table.Column<Guid>(nullable: false),
                    SpellCd = table.Column<long>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Spells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Spells_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Switches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    SwitchId = table.Column<Guid>(nullable: false),
                    Value = table.Column<bool>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Switches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Switches_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Variables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    VariableId = table.Column<Guid>(nullable: false),
                    Value = table.Column<long>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Variables_Players_PlayerId", column: x => x.PlayerId,
                        principalTable: "Players", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        private static void CreateNewIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(name: "IX_Player_Bank_BagId", table: "Player_Bank", column: "BagId");

            migrationBuilder.CreateIndex(name: "IX_Player_Bank_PlayerId", table: "Player_Bank", column: "PlayerId");

            migrationBuilder.CreateIndex(name: "IX_Player_Friends_OwnerId", table: "Player_Friends", column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_Friends_TargetId", table: "Player_Friends", column: "TargetId"
            );

            migrationBuilder.CreateIndex(name: "IX_Player_Hotbar_PlayerId", table: "Player_Hotbar", column: "PlayerId");

            migrationBuilder.CreateIndex(name: "IX_Player_Items_BagId", table: "Player_Items", column: "BagId");

            migrationBuilder.CreateIndex(name: "IX_Player_Items_PlayerId", table: "Player_Items", column: "PlayerId");

            migrationBuilder.CreateIndex(name: "IX_Player_Quests_PlayerId", table: "Player_Quests", column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_Quests_QuestId_PlayerId", table: "Player_Quests",
                columns: new[] {"QuestId", "PlayerId"}, unique: true
            );

            migrationBuilder.CreateIndex(name: "IX_Player_Spells_PlayerId", table: "Player_Spells", column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_Switches_PlayerId", table: "Player_Switches", column: "PlayerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Player_Switches_SwitchId_PlayerId", table: "Player_Switches",
                columns: new[] {"SwitchId", "PlayerId"}, unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Player_Variables_PlayerId", table: "Player_Variables", column: "PlayerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Player_Variables_VariableId_PlayerId", table: "Player_Variables",
                columns: new[] {"VariableId", "PlayerId"}, unique: true
            );

            migrationBuilder.CreateIndex(name: "IX_Players_UserId", table: "Players", column: "UserId");
        }

        private static void DropOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Character_Bank");
            migrationBuilder.DropTable("Character_Friends");
            migrationBuilder.DropTable("Character_Hotbar");
            migrationBuilder.DropTable("Character_Items");
            migrationBuilder.DropTable("Character_Quests");
            migrationBuilder.DropTable("Character_Spells");
            migrationBuilder.DropTable("Character_Switches");
            migrationBuilder.DropTable("Character_Variables");
            migrationBuilder.DropTable("Characters");
        }

        private static void TransferDataUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO Players (Id, UserId, Name, LastOnline, MapId, X, Y, Z, Dir, Gender, Sprite, Face, Level, Exp, ClassId, Vitals, StatPoints, BaseStats, StatPointAllocations, Equipment) " +
                "SELECT Id, AccountId, Name, LastOnline, MapId, X, Y, Z, Dir, Gender, Sprite, Face, Level, Exp, ClassId, Vitals, StatPoints, BaseStats, StatPointAllocations, Equipment " +
                "FROM Characters;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Bank (Id, PlayerId, Slot, BagId, ItemId, Quantity, StatBuffs) " +
                "SELECT Id, CharacterId, Slot, BagId, ItemId, Quantity, StatBuffs " +
                "FROM Character_Bank;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Friends (Id, OwnerId, TargetId) " +
                "SELECT Id, OwnerId, TargetId " +
                "FROM Character_Friends;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Hotbar (Id, PlayerId, Slot, ItemOrSpellId, BagId, PreferredStatBuffs) " +
                "SELECT Id, CharacterId, `Index`, ItemOrSpellId, BagId, PreferredStatBuffs " +
                "FROM Character_Hotbar;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Items (Id, PlayerId, Slot, BagId, ItemId, Quantity, StatBuffs) " +
                "SELECT Id, CharacterId, Slot, BagId, ItemId, Quantity, StatBuffs " +
                "FROM Character_Items;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Quests (Id, PlayerId, QuestId, TaskId, TaskProgress, Completed) " +
                "SELECT Id, CharacterId, QuestId, TaskId, TaskProgress, Completed " +
                "FROM Character_Quests;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Spells (Id, PlayerId, Slot, SpellId, SpellCd) " +
                "SELECT Id, CharacterId, Slot, SpellId, SpellCd " +
                "FROM Character_Spells;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Switches (Id, PlayerId, SwitchId, Value) " +
                "SELECT Id, CharacterId, SwitchId, Value " +
                "FROM Character_Switches;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Variables (Id, PlayerId, VariableId, Value) " +
                "SELECT Id, CharacterId, VariableId, Value " +
                "FROM Character_Variables;"
            );
        }

        private static void CreateOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters", columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    MapId = table.Column<Guid>(nullable: false),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    Z = table.Column<int>(nullable: false),
                    Dir = table.Column<int>(nullable: false),
                    Sprite = table.Column<string>(nullable: true),
                    Face = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Vitals = table.Column<string>(nullable: true),
                    BaseStats = table.Column<string>(nullable: true),
                    StatPointAllocations = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    ClassId = table.Column<Guid>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Exp = table.Column<long>(nullable: false),
                    StatPoints = table.Column<int>(nullable: false),
                    Equipment = table.Column<string>(nullable: true),
                    LastOnline = table.Column<DateTime>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_AccountId", column: x => x.AccountId, principalTable: "Users",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Bank", columns: table => new
                {
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Bank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Bank_Bags_BagId", column: x => x.BagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Character_Bank_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Friends", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: true),
                    TargetId = table.Column<Guid>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Friends_Characters_OwnerId", column: x => x.OwnerId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Character_Friends_Characters_TargetId", column: x => x.TargetId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Hotbar", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    ItemOrSpellId = table.Column<Guid>(nullable: false),
                    BagId = table.Column<Guid>(nullable: false),
                    PreferredStatBuffs = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Hotbar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Hotbar_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Items", columns: table => new
                {
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Items_Bags_BagId", column: x => x.BagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Character_Items_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Quests", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    QuestId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    TaskProgress = table.Column<int>(nullable: false),
                    Completed = table.Column<bool>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Quests_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Spells", columns: table => new
                {
                    SpellId = table.Column<Guid>(nullable: false),
                    SpellCd = table.Column<long>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Spells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Spells_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Switches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    SwitchId = table.Column<Guid>(nullable: false),
                    Value = table.Column<bool>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Switches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Switches_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Character_Variables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CharacterId = table.Column<Guid>(nullable: false),
                    VariableId = table.Column<Guid>(nullable: false),
                    Value = table.Column<long>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Character_Variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Variables_Characters_CharacterId", column: x => x.CharacterId,
                        principalTable: "Characters", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        private static void CreateOldIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(name: "IX_Character_Bank_BagId", table: "Character_Bank", column: "BagId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_Bank_CharacterId", table: "Character_Bank", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Friends_OwnerId", table: "Character_Friends", column: "OwnerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Friends_TargetId", table: "Character_Friends", column: "TargetId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Hotbar_CharacterId", table: "Character_Hotbar", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(name: "IX_Character_Items_BagId", table: "Character_Items", column: "BagId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_Items_CharacterId", table: "Character_Items", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Quests_CharacterId", table: "Character_Quests", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Quests_QuestId_CharacterId", table: "Character_Quests",
                columns: new[] {"QuestId", "CharacterId"}, unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Spells_CharacterId", table: "Character_Spells", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Switches_CharacterId", table: "Character_Switches", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Switches_SwitchId_CharacterId", table: "Character_Switches",
                columns: new[] {"SwitchId", "CharacterId"}, unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Variables_CharacterId", table: "Character_Variables", column: "CharacterId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Character_Variables_VariableId_CharacterId", table: "Character_Variables",
                columns: new[] {"VariableId", "CharacterId"}, unique: true
            );

            migrationBuilder.CreateIndex(name: "IX_Characters_AccountId", table: "Characters", column: "AccountId");
        }

        private static void DropNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Players");
            migrationBuilder.DropTable("Player_Bank");
            migrationBuilder.DropTable("Player_Friends");
            migrationBuilder.DropTable("Player_Hotbar");
            migrationBuilder.DropTable("Player_Items");
            migrationBuilder.DropTable("Player_Quests");
            migrationBuilder.DropTable("Player_Spells");
            migrationBuilder.DropTable("Player_Switches");
            migrationBuilder.DropTable("Player_Variables");
        }

        private static void TransferDataDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO Characters (Id, AccountId, Name, LastOnline, MapId, X, Y, Z, Dir, Gender, Sprite, Face, Level, Exp, ClassId, Vitals, StatPoints, BaseStats, StatPointAllocations, Equipment) " +
                "SELECT Id, UserId, Name, LastOnline, MapId, X, Y, Z, Dir, Gender, Sprite, Face, Level, Exp, ClassId, Vitals, StatPoints, BaseStats, StatPointAllocations, Equipment " +
                "FROM Players;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Bank (Id, CharacterId, Slot, BagId, ItemId, Quantity, StatBuffs) " +
                "SELECT Id, PlayerId, Slot, BagId, ItemId, Quantity, StatBuffs " +
                "FROM Player_Bank;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Friends (Id, OwnerId, TargetId) " +
                "SELECT Id, OwnerId, TargetId " +
                "FROM Player_Friends;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Hotbar (Id, CharacterId, `Index`, ItemOrSpellId, BagId, PreferredStatBuffs) " +
                "SELECT Id, PlayerId, Slot, ItemOrSpellId, BagId, PreferredStatBuffs " +
                "FROM Player_Hotbar;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Items (Id, CharacterId, Slot, BagId, ItemId, Quantity, StatBuffs) " +
                "SELECT Id, PlayerId, Slot, BagId, ItemId, Quantity, StatBuffs " +
                "FROM Player_Items;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Quests (Id, CharacterId, QuestId, TaskId, TaskProgress, Completed) " +
                "SELECT Id, PlayerId, QuestId, TaskId, TaskProgress, Completed " +
                "FROM Player_Quests;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Spells (Id, CharacterId, Slot, SpellId, SpellCd) " +
                "SELECT Id, PlayerId, Slot, SpellId, SpellCd " +
                "FROM Player_Spells;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Switches (Id, CharacterId, SwitchId, Value) " +
                "SELECT Id, PlayerId, SwitchId, Value " +
                "FROM Player_Switches;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Character_Variables (Id, CharacterId, VariableId, Value) " +
                "SELECT Id, PlayerId, VariableId, Value " +
                "FROM Player_Variables;"
            );
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            CreateNewTables(migrationBuilder);

            CreateNewIndexes(migrationBuilder);

            TransferDataUp(migrationBuilder);

            DropOldTables(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            CreateOldTables(migrationBuilder);

            CreateOldIndexes(migrationBuilder);

            TransferDataDown(migrationBuilder);

            DropNewTables(migrationBuilder);
        }

    }

}
