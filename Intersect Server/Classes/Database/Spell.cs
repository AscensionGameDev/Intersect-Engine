using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Server.Classes.Spells;

namespace Intersect.Server.Classes.Database
{
    public class Spell
    {
        public int SpellId { get; set; }
        public long SpellCd { get; set; }
        

        public static Spell None => new Spell(-1);

        public Spell()
        {
        }

        public Spell(int spellIndex)
        {
            SpellId = spellIndex;
        }

        public Spell Clone()
        {
            Spell newSpell = new Spell()
            {
                SpellId = SpellId,
                SpellCd = SpellCd
            };
            return newSpell;
        }

        public virtual void Set(Spell spell)
        {
            SpellId = spell.SpellId;
            SpellCd = spell.SpellId;
        }

        public void Load(ByteBuffer bf)
        {
            SpellId = bf.ReadInteger();
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(SpellId);
            return bf.ToArray();
        }
    }
}
