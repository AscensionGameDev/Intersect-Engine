using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.Server.Classes.Database.PlayerData.Characters;

namespace Intersect.Server.Classes.Database
{
    public class EntityBase
    {
        public string Name { get; set; }
        public Guid Map { get; set; }
        //Going to be obsolete soon.
        public int MapIndex { get; set; }
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
            get => DatabaseUtils.SaveIntArray(Vital, (int)Enums.Vitals.VitalCount);
            set => Vital = DatabaseUtils.LoadIntArray(value, (int)Enums.Vitals.VitalCount);
        }
        [NotMapped]
        public int[] Vital { get; set; } = new int[(int)Enums.Vitals.VitalCount];


        [Column("MaxVitals")]
        public string MaxVitalsJson
        {
            get => DatabaseUtils.SaveIntArray(MaxVital, (int)Enums.Vitals.VitalCount);
            set => MaxVital = DatabaseUtils.LoadIntArray(value, (int)Enums.Vitals.VitalCount);
        }
        [NotMapped]
        public int[] MaxVital { get; set; } = new int[(int)Enums.Vitals.VitalCount];


        [Column("Stats")]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(Stat, (int)Enums.Stats.StatCount);
            set => Stat = DatabaseUtils.LoadIntArray(value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] Stat { get; set; } = new int[(int)Enums.Stats.StatCount];

        //Inventory
        public virtual List<InventorySlot> Items { get; set; } = new List<InventorySlot>();

        //Spells
        public virtual List<SpellSlot> Spells { get; set; } = new List<SpellSlot>();
    }
}
