using System;
using System.ComponentModel.DataAnnotations.Schema;
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
