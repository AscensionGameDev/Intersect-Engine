using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Events;

public partial class IntegerVariableMod : VariableMod
{
    public VariableModType ModType { get; set; } = VariableModType.Set;

    public long Value { get; set; }

    public long HighValue { get; set; }

    [JsonProperty("DupVariableId")]
    public Guid DuplicateVariableId { get; set; }
}