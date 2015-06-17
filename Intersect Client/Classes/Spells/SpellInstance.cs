using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes
{
    public class SpellInstance
    {
        public int SpellNum = -1;
        public long SpellCD = 0;

        public SpellInstance Clone()
        {
            SpellInstance newSpell = new SpellInstance();
            newSpell.SpellNum = SpellNum;
            newSpell.SpellCD = SpellCD;
            return newSpell;
        }
        public void Load(ByteBuffer bf)
        {
            SpellNum = bf.ReadInteger();
            SpellCD = bf.ReadLong();
        }
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(SpellNum);
            bf.WriteLong(SpellCD);
            return bf.ToArray();
        }
    }
}
