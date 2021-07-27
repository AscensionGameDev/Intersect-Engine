using Intersect.Client.Framework.Spells;
using System;

namespace Intersect.Client.Spells
{

    public class Spell : ISpell
    {

        public Guid SpellId { get; set; }

        public ISpell Clone()
        {
            var newSpell = new Spell() {
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
