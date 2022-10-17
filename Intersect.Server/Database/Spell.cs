using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects;

namespace Intersect.Server.Database
{

    public partial class Spell
    {

        public Spell()
        {
        }

        public Spell(Guid spellId)
        {
            SpellId = spellId;
        }

        public Guid SpellId { get; set; }

        [NotMapped]
        public string SpellName => SpellBase.GetName(SpellId);

        public static Spell None => new Spell(Guid.Empty);

        public Spell Clone()
        {
            var newSpell = new Spell()
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
