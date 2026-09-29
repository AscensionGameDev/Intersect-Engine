namespace Intersect.Client.Interface.Game.Furniture;

/// <summary>
/// Holds the currently "armed" furniture catalog selection - the piece of furniture the player has picked
/// from <see cref="FurnitureWindow"/> and is about to place with their next click on the guild hall map.
/// Purely client-side UI state, cleared once the placement click is sent (or cancelled).
/// </summary>
public static class FurniturePlacementState
{
    public static Guid? ArmedFurnitureDescriptorId { get; private set; }

    public static bool IsArmed => ArmedFurnitureDescriptorId.HasValue;

    public static void Arm(Guid furnitureDescriptorId)
    {
        ArmedFurnitureDescriptorId = furnitureDescriptorId;
    }

    public static void Clear()
    {
        ArmedFurnitureDescriptorId = null;
    }
}
