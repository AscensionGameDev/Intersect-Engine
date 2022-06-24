using Intersect.Logging;

namespace Intersect.Server.Database.PlayerData.Migrations
{
    public partial class GuildBankMaxSlotMigration
    {
        public static void Run(PlayerContext context)
        {
            CapGuildBankSize(context);
        }

        public static void CapGuildBankSize(PlayerContext context)
        {
            Log.Info("Checking to see if there are any guilds exceeding the configured max bank size...");

            // Go through each and every quest to check if all the tasks have valid events.
            foreach (var guild in context.Guilds)
            {
                if (guild.BankSlotsCount > Options.Instance.Bank.MaxSlots)
                {
                    Log.Info($"Too many bank slots ({guild.BankSlotsCount}) for guild {guild.Name}. Setting to {Options.Instance.Bank.MaxSlots}.");
                    guild.BankSlotsCount = Options.Instance.Bank.MaxSlots;
                }
            }

            // Track our changes and save them or the work we've just done is lost.
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }
    }
}
