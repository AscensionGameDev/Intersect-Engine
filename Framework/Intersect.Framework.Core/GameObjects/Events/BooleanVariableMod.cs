using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Events;

public partial class BooleanVariableMod : VariableMod
{
    public bool Value { get; set; }

    public VariableType DupVariableType { get; set; } = VariableType.PlayerVariable;

    [JsonProperty("DupVariableId")]
    public Guid DuplicateVariableId { get; set; }
}