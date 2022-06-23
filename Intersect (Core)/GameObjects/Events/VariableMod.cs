using System;

using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{

    public partial class VariableMod
    {

    }

    public partial class IntegerVariableMod : VariableMod
    {

        public VariableMods ModType { get; set; } = VariableMods.Set;

        public long Value { get; set; }

        public long HighValue { get; set; }

        [JsonProperty("DupVariableId")]
        public Guid DuplicateVariableId { get; set; }

    }

    public partial class BooleanVariableMod : VariableMod
    {

        public bool Value { get; set; }

        public VariableTypes DupVariableType { get; set; } = VariableTypes.PlayerVariable;

        [JsonProperty("DupVariableId")]
        public Guid DuplicateVariableId { get; set; }

    }

    public partial class StringVariableMod : VariableMod
    {

        public VariableMods ModType { get; set; } = VariableMods.Set;

        public string Value { get; set; }

        public string Replace { get; set; }

    }

}
