using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Pets;

/// <summary>
/// Describes a companion pet that follows its owner around and picks up loot for them.
/// Pets are regular game objects: they live in the game database, are created from the editor
/// and are given to players through the "Change Pet" event command.
/// </summary>
public partial class PetDescriptor : DatabaseObject<PetDescriptor>, IFolderable
{
    [JsonConstructor]
    public PetDescriptor(Guid id) : base(id)
    {
        Name = "New Pet";
    }

    //EF wants NO PARAMETERS!!!!!
    public PetDescriptor()
    {
        Name = "New Pet";
    }

    /// <summary>
    /// Entity sprite (from resources/entities) used to draw the pet.
    /// </summary>
    public string Sprite { get; set; } = string.Empty;

    /// <summary>
    /// Movement speed, in the same units as an entity's Speed stat.
    /// </summary>
    public int Speed { get; set; } = 30;

    /// <summary>
    /// How far (in tiles, measured from the owner) the pet will go to fetch an item.
    /// </summary>
    public int PickupRange { get; set; } = 6;

    /// <summary>
    /// When true the pet also picks items back up that its owner dropped on purpose.
    /// </summary>
    public bool PickupOwnerDrops { get; set; }

    /// <summary>
    /// When true the pet also picks up items nobody owns (map spawns, expired loot of other players).
    /// </summary>
    public bool PickupUnownedItems { get; set; }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;
}
