using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Entities;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class Variable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        [JsonIgnore] public virtual Player Character { get; private set; }
        public Guid VariableId { get; private set; }
        public long Value { get; set; }

        public Variable()
        {
            
        }

        public Variable(Guid id)
        {
            VariableId = id;
        }
    }
}
