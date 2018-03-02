using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Variable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
        public Guid VariableId { get; set; }
        public int Value { get; set; }
    }
}
