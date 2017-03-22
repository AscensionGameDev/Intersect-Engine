using Intersect_Library;

namespace Intersect_Client.Classes.Spells
{
    public class SpellInstance
    {
        public int SpellNum = -1;
        public long SpellCD = 0;

        public SpellInstance Clone()
        {
            SpellInstance newSpell = new SpellInstance()
            {
                SpellNum = SpellNum,
                SpellCD = SpellCD
            };
            return newSpell;
        }
        public void Load(ByteBuffer bf)
        {
            SpellNum = bf.ReadInteger();
        }
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(SpellNum);
            return bf.ToArray();
        }
    }
}
