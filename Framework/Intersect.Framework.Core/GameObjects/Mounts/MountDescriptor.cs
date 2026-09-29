using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Mounts;

/// <summary>
/// Describes a mount a player can unlock and ride. Riding swaps the player's rendered sprite for the mount's
/// (the player's real sprite, equipment and animations are untouched, just hidden while mounted) and grants a
/// movement speed bonus. Mounts are regular game objects: they live in the game database and are created from
/// the editor, then handed to players through the "Unlock Mount" event command.
/// </summary>
public partial class MountDescriptor : DatabaseObject<MountDescriptor>, IFolderable
{
    [JsonConstructor]
    public MountDescriptor(Guid id) : base(id)
    {
        Name = "New Mount";
    }

    //EF wants NO PARAMETERS!!!!!
    public MountDescriptor()
    {
        Name = "New Mount";
    }

    /// <summary>
    /// Entity sprite (from resources/entities) used to draw the player while mounted.
    /// </summary>
    public string Sprite { get; set; } = string.Empty;

    /// <summary>
    /// Movement speed bonus while riding, as a percentage added on top of the player's normal move speed (e.g. 50 = +50% faster).
    /// </summary>
    public int SpeedBonus { get; set; } = 50;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
