using Intersect.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Classes.Database.PlayerData.Characters
{
    public class BankItem : Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
        public int Slot { get; set; }

        public BankItem()
        {

        }

        public new static BankItem None => new BankItem(-1, 0);

        public BankItem(int itemNum, int itemVal) : base(itemNum, itemVal, null)
        {

        }

        public BankItem(Item item) : base(item.ItemNum,item.ItemVal,item.Bag)
        {
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                StatBoost[i] = item.StatBoost[i];
            }
        }

        public new BankItem Clone()
        {
            return new BankItem(this);
        }
    }
}
