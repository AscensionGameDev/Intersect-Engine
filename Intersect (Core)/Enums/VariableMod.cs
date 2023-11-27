using Intersect.Attributes;

namespace Intersect.Enums
{
    public enum VariableMod
    {
        Set = 0,

        Add,

        Subtract,

        Random,

        SystemTime,

        [RelatedVariableType(VariableType.PlayerVariable)]
        DupPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        DupGlobalVar,

        [RelatedVariableType(VariableType.PlayerVariable)]
        AddPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        AddGlobalVar,

        [RelatedVariableType(VariableType.PlayerVariable)]
        SubtractPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        SubtractGlobalVar,

        Replace,

        Multiply,

        [RelatedVariableType(VariableType.PlayerVariable)]
        MultiplyPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        MultiplyGlobalVar,

        Divide,

        [RelatedVariableType(VariableType.PlayerVariable)]
        DividePlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        DivideGlobalVar,

        LeftShift,

        [RelatedVariableType(VariableType.PlayerVariable)]
        LeftShiftPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        LeftShiftGlobalVar,

        RightShift,

        [RelatedVariableType(VariableType.PlayerVariable)]
        RightShiftPlayerVar,

        [RelatedVariableType(VariableType.ServerVariable)]
        RightShiftGlobalVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        DupGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        AddGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        SubtractGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        MultiplyGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        DivideGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        LeftShiftGuildVar,

        [RelatedVariableType(VariableType.GuildVariable)]
        RightShiftGuildVar,

        [RelatedVariableType(VariableType.UserVariable)]
        DuplicateUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        AddUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        SubtractUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        MultiplyUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        DivideUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        LeftShiftUserVariable,

        [RelatedVariableType(VariableType.UserVariable)]
        RightShiftUserVariable,
    }
}
