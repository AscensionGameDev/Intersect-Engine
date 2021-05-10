using System;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{

    public class GuildBankSlot : Item, ISlot
    {

        public GuildBankSlot()
        {
        }

        public GuildBankSlot(int slot)
        {
            Slot = slot;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
        public Guid Id { get; private set; }

        [JsonIgnore]
        public Guid GuildId { get; private set; }

        [JsonIgnore]
        public virtual Guild Guild { get; private set; }

        public int Slot { get; private set; }

    }

}
