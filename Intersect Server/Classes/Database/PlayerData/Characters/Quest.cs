using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Quest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
        public Guid QuestId { get; set; }
        public Guid TaskId { get; set; }
        public int TaskProgress { get; set; }
        public int Completed { get; set; }
    }
}
