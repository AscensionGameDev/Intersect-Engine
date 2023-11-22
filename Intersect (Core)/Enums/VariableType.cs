using Intersect.Attributes;

namespace Intersect.Enums
{
    // Should properly separate static value, player & global vars into a separate enum.
    // But technical debt :/
    // Crying as I add in Guild variables.. don't hate me :(

    public enum VariableType
    {
        [RelatedTable(GameObjectType.PlayerVariable)]
        PlayerVariable = 0,

        [RelatedTable(GameObjectType.ServerVariable)]
        ServerVariable,

        [RelatedTable(GameObjectType.GuildVariable)]
        GuildVariable,

        [RelatedTable(GameObjectType.UserVariable)]
        UserVariable,
    }
}
