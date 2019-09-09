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

        public void Load(Guid spellId)
        {
            SpellId = spellId;
            SpellCd = 0;
        }
    }
}