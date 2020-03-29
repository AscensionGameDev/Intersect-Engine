using System;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class Friend
    {

        public Friend()
        {
        }

        public Friend(Player me, Player friend)
        {
            Owner = me;
            Target = friend;
        }

        [JsonProperty(nameof(Owner))]

        // Note: Do not change to OwnerId or it collides with the hidden
        // one that Entity Framework expects/creates under the covers.
        private Guid JsonOwnerId => Owner?.Id ?? Guid.Empty;

        [JsonProperty(nameof(Target))]

        // Note: Do not change to TargetId or it collides with the hidden
        // one that Entity Framework expects/creates under the covers.
        private Guid JsonTargetId => Target?.Id ?? Guid.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        [JsonIgnore]
        public virtual Player Owner { get; private set; }

        [JsonIgnore]
        public virtual Player Target { get; private set; }

    }

}
