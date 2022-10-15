using Intersect.Logging;
using System;

namespace Intersect.Server.Database.GameData.Migrations
{
    public class BonusEffectsToListMigration
    {
        public static void Run(GameContext context)
        {
            MigrateItemBonusEffectsToList(context);
        }

        public static void MigrateItemBonusEffectsToList(GameContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Log.Info("Updating items to use new bonus effects list instead of lone value");

            // Go through each and every quest to check if all the tasks have valid events.
            foreach (var item in context.Items)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (item.Effect == default || item.Effect.Type == Enums.EffectType.None)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    continue;
                }

#pragma warning disable CS0618 // Type or member is obsolete
                item.Effects.Add(item.Effect);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            // Track our changes and save them or the work we've just done is lost.
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }
    }
}
