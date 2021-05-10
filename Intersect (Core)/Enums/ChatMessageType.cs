namespace Intersect.Enums
{
    /// <summary>
    /// Defines all different types for chat messages so we can filter them on the client for display purposes.
    /// </summary>
    public enum ChatMessageType
    {
        // Skipping numbers in the below list on purpose for future additions.
        // Chat Channels
        Local = 0,

        Party,

        Global,

        PM,

        Admin,

        Guild,

        // Player Messages
        Experience = 20,

        Loot,

        Inventory,

        Bank,

        Combat,

        Quest,

        Crafting,

        Trading,

        Friend,

        Spells,

        // System Messages
        Notice = 40,

        Error,

    }
}
