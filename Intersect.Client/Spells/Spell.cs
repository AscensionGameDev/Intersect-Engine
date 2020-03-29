using System;

namespace Intersect.Client.Spells
{

    public class Spell
    {

        public Guid SpellId;

        public Spell Clone()
        {
            var newSpell = new Spell()
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
