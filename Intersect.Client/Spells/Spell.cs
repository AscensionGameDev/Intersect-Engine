using System;

namespace Intersect.Client.Spells
{

    public partial class Spell
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
