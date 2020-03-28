using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Models;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class ServerVariableBase : DatabaseObject<ServerVariableBase>, IFolderable
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

        public VariableDataTypes Type { get; set; } = VariableDataTypes.Boolean;

        [NotMapped]
        [JsonIgnore]
        [NotNull]
        public VariableValue Value { get; set; } = new VariableValue();

        [NotMapped]
        [JsonProperty("Value")]
        public dynamic ValueData { get => Value.Value; set => Value.Value = value; }

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
        public string Folder { get; set; } = "";

    }

}
