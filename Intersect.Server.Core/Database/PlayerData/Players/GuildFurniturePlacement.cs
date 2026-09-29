using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Players;

/// <summary>
/// A single piece of furniture a guild has placed inside its guild hall instance. Purely
/// decorative for now - see <see cref="Intersect.Framework.Core.GameObjects.Furniture.FurnitureDescriptor"/>
/// for the catalog entry this references.
/// </summary>
public partial class GuildFurniturePlacement
{
    public GuildFurniturePlacement()
    {
    }

    public GuildFurniturePlacement(Guid furnitureDescriptorId, byte x, byte y, Direction direction)
    {
        FurnitureDescriptorId = furnitureDescriptorId;
        X = x;
        Y = y;
        Direction = direction;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
    public Guid Id { get; private set; }

    [JsonIgnore]
    public Guid GuildId { get; private set; }

    [JsonIgnore]
    [ForeignKey(nameof(GuildId))]
    public virtual Guild Guild { get; private set; }

    public Guid FurnitureDescriptorId { get; set; }

    public byte X { get; set; }

    public byte Y { get; set; }

    public Direction Direction { get; set; }
}
