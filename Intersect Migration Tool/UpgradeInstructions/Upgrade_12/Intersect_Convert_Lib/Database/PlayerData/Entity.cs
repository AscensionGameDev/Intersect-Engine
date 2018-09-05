using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Migration.Localization;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Database.PlayerData
{
    public class EntityInstance
    {
        [Column(Order = 1)]
        public string Name { get; set; }
        public Guid MapId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Dir { get; set; }
        public string Sprite { get; set; }
        public string Face { get; set; }
        public int Level { get; set; }

        [Column("Vitals")]
        public string VitalsJson
        {
            get => DatabaseUtils.SaveIntArray(_vital, (int)Enums.Vitals.VitalCount);
            set => _vital = DatabaseUtils.LoadIntArray(value, (int)Enums.Vitals.VitalCount);
        }
        [NotMapped]
        public int[] _vital { get; set; } = new int[(int)Enums.Vitals.VitalCount];
        [NotMapped]
        private int[] _maxVital = new int[(int)Vitals.VitalCount];


        [Column("Stats")]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(BaseStat, (int)Enums.Stats.StatCount);
            set => BaseStat = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] BaseStat { get; set; } = new int[(int)Enums.Stats.StatCount];

        //Inventory
        public virtual List<InventorySlot> Items { get; set; } = new List<InventorySlot>();

        //Spells
        public virtual List<SpellSlot> Spells { get; set; } = new List<SpellSlot>();


        //Instance Values
        private Guid _id;

        public virtual Guid GetId()
        {
            return _id;
        }
    }
}