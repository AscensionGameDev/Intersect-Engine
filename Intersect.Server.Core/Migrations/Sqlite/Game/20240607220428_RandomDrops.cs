using Intersect.GameObjects;
using Intersect.Server.Database;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class RandomDrops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            using (var context = DbInterface.CreateGameContext(false))
            {
                foreach (var npc in context.Npcs)
                {
                    for (var i = 0; i < npc.Drops.Count; i++)
                    {
                        var drop = npc.Drops[i];
                        npc.Drops[i] = new Drop()
                        {
                            Chance = drop.Chance,
                            ItemId = drop.ItemId,
                            Quantity = drop.Quantity,
                            MinQuantity = 1
                        };
                    }

                    context.Npcs.Update(npc);
                }

                foreach (var resource in context.Resources)
                {
                    for (var i = 0; i < resource.Drops.Count; i++)
                    {
                        var drop = resource.Drops[i];
                        resource.Drops[i] = new Drop()
                        {
                            Chance = drop.Chance,
                            ItemId = drop.ItemId,
                            Quantity = drop.Quantity,
                            MinQuantity = 1
                        };
                    }

                    context.Resources.Update(resource);
                }

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
