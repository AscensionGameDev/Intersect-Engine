using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Events;

public partial class BooleanVariableMod : VariableMod
{
    public bool Value { get; set; }

    public VariableType DupVariableType { get; set; } = VariableType.PlayerVariable;

    [JsonProperty("DupVariableId")]
    public Guid DuplicateVariableId { get; set; }
}