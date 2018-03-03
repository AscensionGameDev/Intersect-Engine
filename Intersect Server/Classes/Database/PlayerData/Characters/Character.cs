using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class Character : EntityBase
    {
        //Account
        public virtual User Account { get; private set; }

        //Character Info
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

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

        public static Character GetCharacter(PlayerContext context, Guid id)
        {
            return GetCharacter(context,p => p.Id == id);
        }

        public static Character GetCharacter(PlayerContext context, string name)
        {
            return GetCharacter(context,p => p.Name.ToLower() == name.ToLower());
        }

        public static Character GetCharacter(PlayerContext context,  System.Linq.Expressions.Expression<Func<Character, bool>> predicate)
        {
            var character = context.Characters.Where(predicate)
                .Include(p => p.Bank)
                .Include(p => p.Friends)
                .Include(p => p.Hotbar)
                .Include(p => p.Quests)
                .Include(p => p.Switches)
                .Include(p => p.Variables)
                .Include(p => p.Items)
                .Include(p => p.Spells)
                .SingleOrDefault();
            if (character != null)
            {
                character.FixLists();
                character.Items = character.Items.OrderBy(p => p.Slot).ToList();
                character.Bank = character.Bank.OrderBy(p => p.Slot).ToList();
                character.Spells = character.Spells.OrderBy(p => p.Slot).ToList();
                character.Hotbar = character.Hotbar.OrderBy(p => p.Slot).ToList();
            }
            return character;
        }

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
    }
}
