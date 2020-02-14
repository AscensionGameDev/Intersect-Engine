using Newtonsoft.Json;
using System;

namespace Intersect.Server.Database
{
    public class Spell
    {
        public Guid SpellId { get; set; }
        

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
                SpellId = SpellId
            };
            return newSpell;
        }

        public virtual void Set(Spell spell)
        {
            SpellId = spell.SpellId;
        }
    }
}
