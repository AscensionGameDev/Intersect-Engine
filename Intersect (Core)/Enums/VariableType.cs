namespace Intersect.Enums
{
    // Should properly separate static value, player & global vars into a separate enum.
    // But technical debt :/
    // Crying as I add in Guild variables.. don't hate me :(
    
    public enum VariableType
    {

        PlayerVariable = 0,

        ServerVariable,

        GuildVariable,

        UserVariable,

    }
}
