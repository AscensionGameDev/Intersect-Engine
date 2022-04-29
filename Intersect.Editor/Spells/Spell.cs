using System;

namespace Intersect.Editor.Spells
{

    public class Spell
    {

        public Guid Id { get; set; }

        public Spell Clone()
        {
            var newSpell = new Spell() {
                Id = Id
            };

            return newSpell;
        }

        public void Load(Guid spellId)
        {
            Id = spellId;
        }

    }

}
