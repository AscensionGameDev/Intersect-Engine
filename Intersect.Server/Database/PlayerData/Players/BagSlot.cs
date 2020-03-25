﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players
{
    public class BagSlot : Item, ISlot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), JsonIgnore]
        public Guid Id { get; private set; }
        [JsonIgnore] public Guid ParentBagId { get; private set; }
        [JsonIgnore] public virtual Bag ParentBag { get; private set; }
        public int Slot { get; private set; }

        public BagSlot()
        {

        }

        public BagSlot(int slot)
        {
            Slot = slot;
        }
    }
}
