using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Server.Entities;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{
    public class Variable : IPlayerOwned
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid PlayerId { get; private set; }
        [JsonIgnore] public virtual Player Player { get; private set; }
        public Guid VariableId { get; private set; }

        [NotMapped]
        public VariableValue Value { get; set; } = new VariableValue();

        [Column("Value")]
        [JsonIgnore]
        public string DBValue
        {
            get => Value.JsonValue;
            private set => Value.JsonValue = value;
        }

        public Variable()
        {
            
        }

        public Variable(Guid id)
        {
            VariableId = id;
        }
    }
}
