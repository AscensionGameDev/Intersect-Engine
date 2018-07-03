using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Classes.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Variable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Player Character { get; private set; }
        public Guid VariableId { get; private set; }
        public int Value { get; set; }

        public Variable()
        {
            
        }

        public Variable(Guid id)
        {
            VariableId = id;
        }
    }
}
