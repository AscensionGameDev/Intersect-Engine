using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Intersect.Migration.Localization;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Crafting;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events.Commands;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Utilities;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using JetBrains.Annotations;
using Switch = Intersect.Server.Classes.Database.PlayerData.Characters.Switch;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Database.PlayerData
{
    [Table("Characters")]
    public class Player : EntityInstance
    {
        //Online Players List
        private static Dictionary<Guid, Player> OnlinePlayers = new Dictionary<Guid, Player>();
        public static Player Find(Guid id) => OnlinePlayers.ContainsKey(id) ? OnlinePlayers[id] : null;
        public static int OnlineCount => OnlinePlayers.Count;

        //Account
        public virtual User Account { get; private set; }

        //Character Info
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new Guid Id { get; private set; }

        //Name, X, Y, Dir, Etc all in the base Entity Class
        public Guid ClassId { get; set; }
        public Gender Gender { get; set; }
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
        public virtual List<BankSlot> Bank { get; set; } = new List<BankSlot>();

        //Friends
        public virtual List<Friend> Friends { get; set; } = new List<Friend>();

        //HotBar
        public virtual List<HotbarSlot> Hotbar { get; set; } = new List<HotbarSlot>();

        //Quests
        public virtual List<Quest> Quests { get; set; } = new List<Quest>();

        //Switches
        public virtual List<Switch> Switches { get; set; } = new List<Switch>();

        //Variables
        public virtual List<Variable> Variables { get; set; } = new List<Variable>();

        public void FixLists()
        {
            if (Spells.Count < Options.MaxPlayerSkills)
            {
                for (int i = Spells.Count; i < Options.MaxPlayerSkills; i++)
                {
                    Spells.Add(new SpellSlot(i));
                }
            }

            if (Items.Count < Options.MaxInvItems)
            {
                for (int i = Items.Count; i < Options.MaxInvItems; i++)
                {
                    Items.Add(new InventorySlot(i));
                }
            }

            if (Bank.Count < Options.MaxBankSlots)
            {
                for (int i = Bank.Count; i < Options.MaxBankSlots; i++)
                {
                    Bank.Add(new BankSlot(i));
                }
            }

            if (Hotbar.Count < Options.MaxHotbar)
            {
                for (var i = Hotbar.Count; i < Options.MaxHotbar; i++)
                {
                    Hotbar.Add(new HotbarSlot(i));
                }
            }
        }


        public Player()
        {

        }

        public override Guid GetId()
        {
            return Id;
        }

        public void Online()
        {
            OnlinePlayers.Add(Id, this);
        }

        ~Player()
        {
            OnlinePlayers.Remove(Id);
        }
    }
}
