using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class Quest : IPlayerOwned
    {

        public Quest()
        {
        }

        public Quest(Guid id)
        {
            QuestId = id;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public Guid Id { get; private set; }

        [JsonIgnore]
        public Guid QuestId { get; private set; }

        public Guid TaskId { get; set; }

        public int TaskProgress { get; set; }

        public bool Completed { get; set; }

        [JsonIgnore]
        public Guid PlayerId { get; private set; }

        [JsonIgnore]
        public virtual Player Player { get; private set; }

        public string Data()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}
