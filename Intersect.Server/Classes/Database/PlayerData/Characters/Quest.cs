using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Entities;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Characters
{
    public class Quest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public Guid CharacterId { get; private set; }
        [JsonIgnore] public virtual Player Character { get; private set; }
        public Guid QuestId { get; private set; }
        public Guid TaskId { get; set; }
        public int TaskProgress { get; set; }
        public bool Completed { get; set; }

        public Quest()
        {
            
        }

        public Quest(Guid id)
        {
            QuestId = id;
        }
    }
}
