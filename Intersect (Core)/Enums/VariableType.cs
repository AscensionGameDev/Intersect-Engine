using System.ComponentModel;
using static Intersect.Attributes.Attributes;

namespace Intersect.Enums
{
    // Should properly separate static value, player & global vars into a separate enum.
    // But technical debt :/
    // Crying as I add in Guild variables.. don't hate me :(

    public enum VariableType
    {
        [Description("Player Variable")]
        [RelatedTable(GameObjectType.PlayerVariable)]
        PlayerVariable = 0,

        [Description("Server Variable")]
        [RelatedTable(GameObjectType.ServerVariable)]
        ServerVariable,

        [Description("Guild Variable")]
        [RelatedTable(GameObjectType.GuildVariable)]
        GuildVariable,

        [Description("Instance Variable")]
        [RelatedTable(GameObjectType.UserVariable)]
        UserVariable,
    }
}
