using System.Collections.Immutable;
using Intersect.Enums;

namespace Intersect.Utilities;
public static class VariableModUtils
{

    public static readonly ImmutableArray<VariableMod> SetMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Set,
            VariableMod.DupPlayerVar,
            VariableMod.DupGlobalVar,
            VariableMod.DupGuildVar,
            VariableMod.DuplicateUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> AddMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Add,
            VariableMod.AddPlayerVar,
            VariableMod.AddGlobalVar,
            VariableMod.AddGuildVar,
            VariableMod.AddUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> SubMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Subtract,
            VariableMod.SubtractPlayerVar,
            VariableMod.SubtractGlobalVar,
            VariableMod.SubtractGuildVar,
            VariableMod.SubtractUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> MultMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Multiply,
            VariableMod.MultiplyPlayerVar,
            VariableMod.MultiplyGlobalVar,
            VariableMod.MultiplyGuildVar,
            VariableMod.MultiplyUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> DivideMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Divide,
            VariableMod.DividePlayerVar,
            VariableMod.DivideGlobalVar,
            VariableMod.DivideGuildVar,
            VariableMod.DivideUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> LShiftMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.LeftShift,
            VariableMod.LeftShiftPlayerVar,
            VariableMod.LeftShiftGlobalVar,
            VariableMod.LeftShiftGuildVar,
            VariableMod.LeftShiftUserVariable,
        });

    public static readonly ImmutableArray<VariableMod> RShiftMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.RightShift,
            VariableMod.RightShiftPlayerVar,
            VariableMod.RightShiftGlobalVar,
            VariableMod.RightShiftGuildVar,
            VariableMod.RightShiftUserVariable,
        });

}
