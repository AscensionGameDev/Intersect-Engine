namespace Intersect.Framework.Core.GameObjects.Maps.Attributes;

/// <summary>
/// The doorway of a building: the only tile type that connects Indoor tiles with the outside.
/// Roof layers are hidden while the local player stands on it.
/// </summary>
public partial class MapIndoorDoorAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.IndoorDoor;
}
