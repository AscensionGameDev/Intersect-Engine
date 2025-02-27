using Intersect.Enums;

namespace Intersect.GameObjects.Events;

public partial class StringVariableMod : VariableMod
{
    public VariableModType ModType { get; set; } = VariableModType.Set;

    public string Value { get; set; }

    public string Replace { get; set; }
}