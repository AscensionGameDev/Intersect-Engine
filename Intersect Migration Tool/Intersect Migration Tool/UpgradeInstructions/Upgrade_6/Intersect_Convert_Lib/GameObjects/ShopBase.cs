using System;
using System.Collections.Generic;
using System.Linq;
using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects
{
    public class ShopBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "shops";
        public new const GameObject OBJECT_TYPE = GameObject.Shop;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        public List<ShopItem> BuyingItems = new List<ShopItem>();

        //Buying List
        public bool BuyingWhitelist = true;
        public int DefaultCurrency = 0;

        public string Name = "New Shop";

        //Selling List
        public List<ShopItem> SellingItems = new List<ShopItem>();

        public ShopBase(int id) : base(id)
        {
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            DefaultCurrency = myBuffer.ReadInteger();
            SellingItems.Clear();
            BuyingItems.Clear();
            var sellingCount = myBuffer.ReadInteger();
            for (int i = 0; i < sellingCount; i++)
            {
                SellingItems.Add(new ShopItem(myBuffer));
            }
            BuyingWhitelist = Convert.ToBoolean(myBuffer.ReadByte());
            var buyingCount = myBuffer.ReadInteger();
            for (int i = 0; i < buyingCount; i++)
            {
                BuyingItems.Add(new ShopItem(myBuffer));
            }
            myBuffer.Dispose();
        }

        public byte[] ShopData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(DefaultCurrency);
            myBuffer.WriteInteger(SellingItems.Count);
            for (int i = 0; i < SellingItems.Count; i++)
            {
                myBuffer.WriteBytes(SellingItems[i].Data());
            }
            myBuffer.WriteByte(Convert.ToByte(BuyingWhitelist));
            myBuffer.WriteInteger(BuyingItems.Count);
            for (int i = 0; i < BuyingItems.Count; i++)
            {
                myBuffer.WriteBytes(BuyingItems[i].Data());
            }

            return myBuffer.ToArray();
        }

        public static ShopBase GetShop(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (ShopBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ShopBase) Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return ShopData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, ShopBase> GetObjects()
        {
            Dictionary<int, ShopBase> objects = Objects.ToDictionary(k => k.Key, v => (ShopBase) v.Value);
            return objects;
        }
    }

    public class ShopItem
    {
        public int CostItemNum;
        public int CostItemVal;
        public int ItemNum;

        public ShopItem(ByteBuffer myBuffer)
        {
            ItemNum = myBuffer.ReadInteger();
            CostItemNum = myBuffer.ReadInteger();
            CostItemVal = myBuffer.ReadInteger();
        }

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