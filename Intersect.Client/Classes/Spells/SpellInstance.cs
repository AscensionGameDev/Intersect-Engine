using System;

namespace Intersect.Client.Spells
{
    public class SpellInstance
    {
        public long SpellCd;
        public Guid SpellId;

        public SpellInstance Clone()
        {
            SpellInstance newSpell = new SpellInstance()
            {
                SpellId = SpellId,
                SpellCd = SpellCd
            };
            return newSpell;
        }

        public void Load(ByteBuffer bf)
        {
            SpellId = bf.ReadGuid();
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(SpellId);
            return bf.ToArray();
        }
    }
}