using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ShopBase : DatabaseObject<ShopBase>
    {
        public bool BuyingWhitelist { get; set; } = true;
        public int DefaultCurrency { get; set; }
        
        [Column("BuyingItems")]
        [JsonIgnore]
        public string JsonBuyingItems
        {
            get => JsonConvert.SerializeObject(BuyingItems);
            set => BuyingItems = JsonConvert.DeserializeObject<List<ShopItem>>(value);
        }
        [NotMapped]
        public List<ShopItem> BuyingItems = new List<ShopItem>();

        [Column("SellingItems")]
        [JsonIgnore]
        public string JsonSellingItems
        {
            get => JsonConvert.SerializeObject(SellingItems);
            set => SellingItems = JsonConvert.DeserializeObject<List<ShopItem>>(value);
        }
        [NotMapped]
        public List<ShopItem> SellingItems = new List<ShopItem>();

        [JsonConstructor]
        public ShopBase(int index) : base(index)
        {
            Name = "New Shop";
        }

        //EF is so damn picky about its parameters
        public ShopBase()
        {
            Name = "New Shop";
        }

        public static ShopBase Get(int index)
        {
            return ShopBase.Lookup.Get<ShopBase>(index);
        }
    }

    public class ShopItem
    {
        public int CostItemNum;
        public int CostItemVal;
        public int ItemNum;

        [NotMapped]
        public ItemBase Item => ItemBase.Lookup.Get<ItemBase>(ItemNum);

        public ShopItem(ByteBuffer myBuffer)
        {
            ItemNum = myBuffer.ReadInteger();
            CostItemNum = myBuffer.ReadInteger();
            CostItemVal = myBuffer.ReadInteger();
        }

        [JsonConstructor]
        public ShopItem(int itemNum, int costItemNum, int costVal)
        {
            ItemNum = itemNum;
            CostItemNum = costItemNum;
            CostItemVal = costVal;
        }

        public byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(ItemNum);
            myBuffer.WriteInteger(CostItemNum);
            myBuffer.WriteInteger(CostItemVal);
            return myBuffer.ToArray();
        }
    }
}