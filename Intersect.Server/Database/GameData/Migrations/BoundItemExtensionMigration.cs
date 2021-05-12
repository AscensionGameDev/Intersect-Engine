namespace Intersect.Server.Database.GameData.Migrations
{
    public class BoundItemExtensionMigration
    {

        public static void Run(GameContext context)
        {
            MigrateBoundItems(context);
        }

        public static void MigrateBoundItems(GameContext context)
        {
            // Go through all of our items and determine whether or not the Bound property (now CanDrop) was true, if so set all the Cans to false because it's a bound item!
            // The user will have to set the granularity later.
            foreach(var item in context.Items)
            {
                if (item.CanDrop == true)
                {
                    item.CanDrop = false;
                    item.CanDropOnDeath = false;
                    item.CanTrade = false;
                    item.CanSell = false;
                    item.CanBank = false;
                    item.CanBag = false;
                }
            }

            // Track our changes and save them or the work we've just done is lost.
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }

    }
}
