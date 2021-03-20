using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Server.Entities;

using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Players
{

    public class Variable : IPlayerOwned
    {

        public Variable() : this(Guid.Empty) { }

        public Variable(Guid id)
        {
            VariableId = id;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
        public Guid Id { get; protected set; }

        public Guid VariableId { get; protected set; }

        [NotMapped]
        public string VariableName => PlayerVariableBase.GetName(VariableId);

        [NotMapped]
        [JsonIgnore]
        public VariableValue Value { get; set; } = new VariableValue();

        [NotMapped]
        [JsonProperty("Value")]
        public dynamic ValueData => Value.Value;

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

        [JsonIgnore]
        public Guid PlayerId { get; protected set; }

        [JsonIgnore]
        public virtual Player Player { get; protected set; }

    }

}
