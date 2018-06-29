using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects
{
    public class CraftingTableBase : DatabaseObject<CraftingTableBase>
    {
        public List<int> Crafts = new List<int>();

        [JsonConstructor]
        public CraftingTableBase(int index) : base(index)
        {
            Name = "New Table";
        }

        public override byte[] BinaryData => Data();

        public override void Load(byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(data);
            Name = bf.ReadString();
            var count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                Crafts.Add(bf.ReadInteger());
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteString(Name);
            bf.WriteInteger(Crafts.Count);
            for (int i = 0; i < Crafts.Count; i++)
            {
                bf.WriteInteger(Crafts[i]);
            }
            return bf.ToArray();
        }
    }
}
