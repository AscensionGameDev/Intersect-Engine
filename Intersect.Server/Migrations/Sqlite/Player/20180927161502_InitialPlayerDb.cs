using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{

    public partial class InitialPlayerDb : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bags", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SlotCount = table.Column<int>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Bags", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Users", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Power = table.Column<string>(nullable: true),
                    PasswordResetCode = table.Column<string>(nullable: true),
                    PasswordResetTime = table.Column<DateTime>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Bag_Items", columns: table => new
                {
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    ParentBagId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Bag_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bag_Items_Bags_BagId", column: x => x.BagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_Bag_Items_Bags_ParentBagId", column: x => x.ParentBagId, principalTable: "Bags",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Bans", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    Banner = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_Users_PlayerId", column: x => x.PlayerId, principalTable: "Users",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

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
                name: "Mutes", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    Muter = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Mutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mutes_Users_PlayerId", column: x => x.PlayerId, principalTable: "Users",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
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

            migrationBuilder.CreateIndex(name: "IX_Bag_Items_BagId", table: "Bag_Items", column: "BagId");

            migrationBuilder.CreateIndex(name: "IX_Bag_Items_ParentBagId", table: "Bag_Items", column: "ParentBagId");

            migrationBuilder.CreateIndex(name: "IX_Bans_PlayerId", table: "Bans", column: "PlayerId");

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

            migrationBuilder.CreateIndex(name: "IX_Mutes_PlayerId", table: "Mutes", column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Bag_Items");

            migrationBuilder.DropTable(name: "Bans");

            migrationBuilder.DropTable(name: "Character_Bank");

            migrationBuilder.DropTable(name: "Character_Friends");

            migrationBuilder.DropTable(name: "Character_Hotbar");

            migrationBuilder.DropTable(name: "Character_Items");

            migrationBuilder.DropTable(name: "Character_Quests");

            migrationBuilder.DropTable(name: "Character_Spells");

            migrationBuilder.DropTable(name: "Character_Switches");

            migrationBuilder.DropTable(name: "Character_Variables");

            migrationBuilder.DropTable(name: "Mutes");

            migrationBuilder.DropTable(name: "Bags");

            migrationBuilder.DropTable(name: "Characters");

            migrationBuilder.DropTable(name: "Users");
        }

    }

}
