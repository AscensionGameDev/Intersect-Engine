using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Logging;
using Intersect.Server.Entities;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.Database.PlayerData.Players
{
    public class Variable : IPlayerOwned
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        public Guid PlayerId { get; protected set; }

        [JsonIgnore] public virtual Player Player { get; protected set; }

        public Guid VariableId { get; protected set; }

        [NotMapped]
        [NotNull]
        public VariableValue Value { get; set; } = new VariableValue();

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

        public Variable() : this(Guid.Empty) { }

        public Variable(Guid id)
        {
            VariableId = id;
        }
    }
}
