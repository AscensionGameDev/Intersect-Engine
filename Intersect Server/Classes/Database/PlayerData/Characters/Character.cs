using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Character : EntityBase
    {
        //Account
        public User Account { get; set; }

        //Character Info
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //Name, X, Y, Dir, Etc all in the base Entity Class
        public Guid Class { get; set; }
        public int ClassIndex { get; set; }
        public int Gender { get; set; }
        public long Exp { get; set; }

        public int StatPoints { get; set; }

        [Column("Equipment")]
        public string EquipmentJson
        {
            get => DatabaseUtils.SaveIntArray(Equipment, Options.EquipmentSlots.Count);
            set => Equipment = DatabaseUtils.LoadIntArray(value, Options.EquipmentSlots.Count);
        }
        [NotMapped]
        public int[] Equipment { get; set; } = new int[Options.EquipmentSlots.Count];


        public DateTime? LastOnline { get; set; }

        //Bank
        public List<BankItem> Bank { get; set; } = new List<BankItem>();

        //Friends
        public List<Friend> Friends { get; set; } = new List<Friend>();

        //HotBar
        public List<Hotbar> Hotbar { get; set; } = new List<Hotbar>();

        //Quests
        public List<Quest> Quests { get; set; } = new List<Quest>();

        //Switches
        public List<Switch> Switches { get; set; } = new List<Switch>();

        //Variables
        public List<Variable> Variables { get; set; } = new List<Variable>();
    }
}
