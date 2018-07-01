using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Server.Classes.Spells;

namespace Intersect.Server.Classes.Database
{
    public class Spell
    {
        public Guid SpellId { get; set; }
        public long SpellCd { get; set; }
        

        public static Spell None => new Spell(Guid.Empty);

        public Spell()
        {
        }

        public Spell(Guid spellId)
        {
            SpellId = spellId;
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
            SpellCd = spell.SpellCd;
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
