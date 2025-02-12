using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Menu;

public record struct CharacterSelectionPreviewMetadata(
    Guid Id,
    string Name,
    string Sprite,
    string Face,
    int Level,
    string Class,
    EquipmentFragment[] Equipment
);