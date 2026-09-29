using Newtonsoft.Json;
using Intersect.Models;

namespace Intersect.Framework.Core.GameObjects.Achievements;

/// <summary>
/// Describes an achievement ("haut fait") a player can unlock, with optional rewards. Achievements are regular
/// game objects: they live in the game database, are created from the editor and are granted to players through
/// the "Unlock Achievement" event command (or automatically when clearing a dungeon that lists one as its reward).
/// </summary>
public partial class AchievementDescriptor : DatabaseObject<AchievementDescriptor>, IFolderable
{
    [JsonConstructor]
    public AchievementDescriptor(Guid id) : base(id)
    {
        Name = "New Achievement";
    }

    //EF wants NO PARAMETERS!!!!!
    public AchievementDescriptor()
    {
        Name = "New Achievement";
    }

    /// <summary>Shown under the achievement's name once unlocked (or always, if not <see cref="Hidden"/>).</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Icon (from resources/items, reusing item-icon sized art) shown next to the achievement.</summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// When true, the achievement's name and description are replaced with "???" until the player unlocks it,
    /// like a secret achievement.
    /// </summary>
    public bool Hidden { get; set; }

    public int RewardExperience { get; set; }

    public Guid RewardItemId { get; set; } = Guid.Empty;

    public int RewardItemQuantity { get; set; } = 1;

    public Guid RewardTitleId { get; set; } = Guid.Empty;

    public Guid RewardMountId { get; set; } = Guid.Empty;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
