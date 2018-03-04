using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Classes.Entities;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Quest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        public virtual Player Character { get; private set; }
        public int QuestId { get; private set; }
        public int TaskId { get; set; }
        public int TaskProgress { get; set; }
        public int Completed { get; set; }

        public Quest()
        {
            
        }

        public Quest(int id)
        {
            QuestId = id;
        }
    }
}
