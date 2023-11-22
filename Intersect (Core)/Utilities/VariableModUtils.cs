using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Utilities;
public static class VariableModUtils
{

    public static ImmutableArray<VariableMod> SetMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Set,
            VariableMod.DupPlayerVar,
            VariableMod.DupGlobalVar,
            VariableMod.DupGuildVar,
            VariableMod.DuplicateUserVariable,
        });

    public static ImmutableArray<VariableMod> AddMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Add,
            VariableMod.AddPlayerVar,
            VariableMod.AddGlobalVar,
            VariableMod.AddGuildVar,
            VariableMod.AddUserVariable,
        });

    public static ImmutableArray<VariableMod> SubMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Subtract,
            VariableMod.SubtractPlayerVar,
            VariableMod.SubtractGlobalVar,
            VariableMod.SubtractGuildVar,
            VariableMod.SubtractUserVariable,
        });

    public static ImmutableArray<VariableMod> MultMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Multiply,
            VariableMod.MultiplyPlayerVar,
            VariableMod.MultiplyGlobalVar,
            VariableMod.MultiplyGuildVar,
            VariableMod.MultiplyUserVariable,
        });

    public static ImmutableArray<VariableMod> DivideMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.Divide,
            VariableMod.DividePlayerVar,
            VariableMod.DivideGlobalVar,
            VariableMod.DivideGuildVar,
            VariableMod.DivideUserVariable,
        });

    public static ImmutableArray<VariableMod> LShiftMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.LeftShift,
            VariableMod.LeftShiftPlayerVar,
            VariableMod.LeftShiftGlobalVar,
            VariableMod.LeftShiftGuildVar,
            VariableMod.LeftShiftUserVariable,
        });

    public static ImmutableArray<VariableMod> RShiftMods = ImmutableArray.Create(new VariableMod[] {
            VariableMod.RightShift,
            VariableMod.RightShiftPlayerVar,
            VariableMod.RightShiftGlobalVar,
            VariableMod.RightShiftGuildVar,
            VariableMod.RightShiftUserVariable,
        });

}
