namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Unlocks an achievement for the player, granting its rewards (if any) and announcing it in chat.
/// </summary>
public partial class UnlockAchievementCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.UnlockAchievement;

    public Guid AchievementId { get; set; }
}
