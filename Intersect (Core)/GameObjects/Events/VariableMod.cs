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

        public Enums.VariableMod ModType { get; set; } = Enums.VariableMod.Set;

        public long Value { get; set; }

        public long HighValue { get; set; }

        [JsonProperty("DupVariableId")]
        public Guid DuplicateVariableId { get; set; }

    }

    public partial class BooleanVariableMod : VariableMod
    {

        public bool Value { get; set; }

        public VariableType DupVariableType { get; set; } = VariableType.PlayerVariable;

        [JsonProperty("DupVariableId")]
        public Guid DuplicateVariableId { get; set; }

    }

    public partial class StringVariableMod : VariableMod
    {

        public Enums.VariableMod ModType { get; set; } = Enums.VariableMod.Set;

        public string Value { get; set; }

        public string Replace { get; set; }

    }

}
