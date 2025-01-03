using System.Collections.Immutable;
using Intersect.Enums;

namespace Intersect.Utilities;
public static class VariableModUtils
{

    public static readonly ImmutableArray<VariableModType> SetMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.Set,
            VariableModType.DupPlayerVar,
            VariableModType.DupGlobalVar,
            VariableModType.DupGuildVar,
            VariableModType.DuplicateUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> AddMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.Add,
            VariableModType.AddPlayerVar,
            VariableModType.AddGlobalVar,
            VariableModType.AddGuildVar,
            VariableModType.AddUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> SubMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.Subtract,
            VariableModType.SubtractPlayerVar,
            VariableModType.SubtractGlobalVar,
            VariableModType.SubtractGuildVar,
            VariableModType.SubtractUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> MultMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.Multiply,
            VariableModType.MultiplyPlayerVar,
            VariableModType.MultiplyGlobalVar,
            VariableModType.MultiplyGuildVar,
            VariableModType.MultiplyUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> DivideMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.Divide,
            VariableModType.DividePlayerVar,
            VariableModType.DivideGlobalVar,
            VariableModType.DivideGuildVar,
            VariableModType.DivideUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> LShiftMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.LeftShift,
            VariableModType.LeftShiftPlayerVar,
            VariableModType.LeftShiftGlobalVar,
            VariableModType.LeftShiftGuildVar,
            VariableModType.LeftShiftUserVariable,
        });

    public static readonly ImmutableArray<VariableModType> RShiftMods = ImmutableArray.Create(new VariableModType[] {
            VariableModType.RightShift,
            VariableModType.RightShiftPlayerVar,
            VariableModType.RightShiftGlobalVar,
            VariableModType.RightShiftGuildVar,
            VariableModType.RightShiftUserVariable,
        });

}
