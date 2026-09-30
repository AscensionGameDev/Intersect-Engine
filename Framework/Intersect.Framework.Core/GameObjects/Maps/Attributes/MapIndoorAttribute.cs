namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

/// <summary>
/// Marks a tile as the inside of a building. The roof layers are hidden while the local player stands on it,
/// and nobody can step between an Indoor tile and an outside tile except through an <see cref="MapIndoorDoorAttribute"/> tile.
/// </summary>
public partial class MapIndoorAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Indoor;

    /// <summary>
    /// True when a single step from <paramref name="from"/> to <paramref name="to"/> crosses the wall of a
    /// building (Indoor tile on one side, a tile that is neither Indoor nor a door on the other).
    /// </summary>
    public static bool BlocksMove(MapAttribute? from, MapAttribute? to)
    {
        var fromType = from?.Type;
        var toType = to?.Type;

        static bool IsHouseTile(MapAttributeType? type) =>
            type is MapAttributeType.Indoor or MapAttributeType.IndoorDoor;

        if (fromType == MapAttributeType.Indoor && !IsHouseTile(toType))
        {
            return true;
        }

        return toType == MapAttributeType.Indoor && !IsHouseTile(fromType);
    }
}
