namespace Intersect.Network
{
    public enum PacketGroups : byte
    {
        Ping = 0,
        Utility,
        Authentication,
        Administration,
        ObjectData,
        Chat,
        Entity,
        Character,
        Movement,
        Combat,
        Projectile,
        Item,
        Spell,
        Shop,
        Bank,
        Crafting,
        Party,
        Quest,
        Trade,
        Bag,
        Friend
    }
}