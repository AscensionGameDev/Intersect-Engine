using Intersect.Server.Database.Migration;
using Intersect.Server.Database.PlayerData;

using System.Linq;

namespace Intersect.Server.Database.GameData.Migrations
{
    internal class AddInitialDataMigrationHistory<TContext> : DataMigration<TContext>
        where TContext : IntersectDbContext<TContext>
    {
    }

    [DataMigration("20201112213700_AddInitialDataMigrationHistory")]
    [RequiresMigration("20201112030046_AddDataMigrationHistory", MigrationType.Schema)]
    internal class AddInitialDataMigrationHistoryGameContext : AddInitialDataMigrationHistory<GameContext>
    {
        public override bool Up(GameContext context)
        {
            if (context.__DataMigrationsHistory.Any())
            {
                return false;
            }

            context.__DataMigrationsHistory.Add(
                DataMigrationMetadata.GetMigrationMetadataFor(typeof(Beta6)).CreateHistory<GameContext>()
            );

            return context.SaveChanges() > 0;
        }
    }

    [DataMigration("20201112213700_AddInitialDataMigrationHistory")]
    [RequiresMigration("20201112030147_AddDataMigrationHistory", MigrationType.Schema)]
    internal class AddInitialDataMigrationHistoryPlayerContext : AddInitialDataMigrationHistory<PlayerContext>
    {
        public override bool Up(PlayerContext context) => true;
    }
}
