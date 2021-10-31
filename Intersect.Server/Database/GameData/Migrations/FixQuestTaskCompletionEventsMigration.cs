using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Logging;

namespace Intersect.Server.Database.GameData.Migrations
{
    public class FixQuestTaskCompletionEventsMigration
    {

        public static void Run(GameContext context)
        {
            FixQuestTaskCompletionEvents(context);
        }

        public static void FixQuestTaskCompletionEvents(GameContext context)
        {
            Log.Info("Checking for broken Quest Task Completion Events..");

            // Go through each and every quest to check if all the tasks have valid events.
            foreach (var quest in context.Quests)
            {
                foreach(var task in quest.Tasks)
                {
                    // Is the completion event non-existent? If so, try and fix it!
                    if (task.CompletionEvent == null)
                    {
                        Log.Warn($"Quest {quest.Name} ({quest.Name}) has task {task.Id} with broken CompletionEvent, attempting to fix..");

                        // Can we find the event detached somewhere?
                        var foundEvent = EventBase.Get(task.Id);
                        if (foundEvent != null)
                        {
                            // We can! Link it up again!
                            task.CompletionEvent = foundEvent;
                            Log.Info($"Fixed quest {quest.Name} ({quest.Name}) task {task.Id}, linked up old event {foundEvent.Id}.");
                        }
                        else
                        {
                            // Somehow this event doesn't exist.. Recreate it!
                            var evtb = (EventBase)DbInterface.AddGameObject(GameObjectType.Event, task.Id);
                            task.CompletionEvent = evtb;
                            Log.Info($"Fixed quest {quest.Name} ({quest.Name}) task {task.Id}, created new event {evtb.Id}.");
                        }
                    }
                }
            }

            // Track our changes and save them or the work we've just done is lost.
            context.ChangeTracker.DetectChanges();
            context.SaveChanges();
        }

    }
}
