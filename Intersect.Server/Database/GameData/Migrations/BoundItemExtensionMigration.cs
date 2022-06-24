namespace Intersect.Server.Database.GameData.Migrations
{
    public partial class BoundItemExtensionMigration
    {

        public static void Run(GameContext context)
        {
            MigrateBoundItems(context);
        }

        public static void MigrateBoundItems(GameContext context)
        {
            // Go through all of our items to reconfigure the new item binding properties.
            foreach(var item in context.Items)
            {

                // Determine whether or not the Bound property(now CanDrop) was true, if so set all the binding options to not allow item drops.
                if (item.CanDrop == true)
                {
                    item.CanDrop = false;
                    item.CanTrade = false;
                    item.CanSell = false;
                    item.CanBank = false;
                    item.CanBag = false;
                    item.CanGuildBank = false;

                    item.DropChanceOnDeath = 0;
                }
                // If it wasn't bound before, unbind all the new options!
                else
                {
                    item.CanDrop = true;
                    item.CanTrade = true;
                    item.CanSell = true;
                    item.CanBank = true;
                    item.CanBag = true;
                    item.CanGuildBank = true;

                    // Set item drop chance to the default configuration option.
                    item.DropChanceOnDeath = Options.ItemDropChance;
                }
            }

            // Track our changes and save them or the work we've just done is lost.
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }

    }
}
