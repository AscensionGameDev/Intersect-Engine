using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class ServerVariableBase : VariableDescriptor<ServerVariableBase>, IVariableBase
    {
        [JsonConstructor]
        public ServerVariableBase(Guid id) : base(id)
        {
            Name = "New Global Variable";
        }

        public ServerVariableBase()
        {
            Name = "New Global Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        // TODO(0.8): Rename this to DataType
        public VariableDataType Type { get; set; } = VariableDataType.Boolean;

        [NotMapped]
        [JsonIgnore]
        public VariableValue Value { get; set; } = new VariableValue();

        [NotMapped]
        [JsonProperty("Value")]
        public dynamic ValueData
        {
            get => Value.Value;
            set => Value.Value = value;
        }

        [Column(nameof(Value))]
        [JsonIgnore]
        public string Json
        {
            get => Value.Json.ToString(Formatting.None);
            private set
            {
                if (VariableValue.TryParse(value, out var json))
                {
                    Value.Json = json;
                }
            }
        }

        /// <inheritdoc />
        public string Folder { get; set; } = string.Empty;
    }
}
