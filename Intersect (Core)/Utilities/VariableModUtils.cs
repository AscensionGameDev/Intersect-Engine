using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;

namespace Intersect.Utilities;
public static class VariableModUtils
{

    public static List<VariableMod> SetMods = new List<VariableMod>() {
            VariableMod.Set,
            VariableMod.DupPlayerVar,
            VariableMod.DupGlobalVar,
            VariableMod.DupGuildVar,
            VariableMod.DuplicateUserVariable,
        };

    public static List<VariableMod> AddMods = new List<VariableMod>() {
            VariableMod.Add,
            VariableMod.AddPlayerVar,
            VariableMod.AddGlobalVar,
            VariableMod.AddGuildVar,
            VariableMod.AddUserVariable,
        };

    public static List<VariableMod> SubMods = new List<VariableMod>() {
            VariableMod.Subtract,
            VariableMod.SubtractPlayerVar,
            VariableMod.SubtractGlobalVar,
            VariableMod.SubtractGuildVar,
            VariableMod.SubtractUserVariable,
        };

    public static List<VariableMod> MultMods = new List<VariableMod>() {
            VariableMod.Multiply,
            VariableMod.MultiplyPlayerVar,
            VariableMod.MultiplyGlobalVar,
            VariableMod.MultiplyGuildVar,
            VariableMod.MultiplyUserVariable,
        };

    public static List<VariableMod> DivideMods = new List<VariableMod>() {
            VariableMod.Divide,
            VariableMod.DividePlayerVar,
            VariableMod.DivideGlobalVar,
            VariableMod.DivideGuildVar,
            VariableMod.DivideUserVariable,
        };

    public static List<VariableMod> LShiftMods = new List<VariableMod>() {
            VariableMod.LeftShift,
            VariableMod.LeftShiftPlayerVar,
            VariableMod.LeftShiftGlobalVar,
            VariableMod.LeftShiftGuildVar,
            VariableMod.LeftShiftUserVariable,
        };

    public static List<VariableMod> RShiftMods = new List<VariableMod>() {
            VariableMod.RightShift,
            VariableMod.RightShiftPlayerVar,
            VariableMod.RightShiftGlobalVar,
            VariableMod.RightShiftGuildVar,
            VariableMod.RightShiftUserVariable,
        };

}
