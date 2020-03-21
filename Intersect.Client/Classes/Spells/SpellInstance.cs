using System;

namespace Intersect.Client.Spells
{
    public class SpellInstance
    {
        public Guid SpellId;

        public SpellInstance Clone()
        {
            SpellInstance newSpell = new SpellInstance()
            {
                SpellId = SpellId
            };
            return newSpell;
        }

        public void Load(Guid spellId)
        {
            SpellId = spellId;
        }
    }
}