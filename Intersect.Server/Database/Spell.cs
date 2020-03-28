using System;

using Newtonsoft.Json;

namespace Intersect.Server.Database
{

    public class Spell
    {

        public Spell()
        {
        }

        public Spell(Guid spellId)
        {
            SpellId = spellId;
        }

        public Guid SpellId { get; set; }

        //SpellCD NO LONGER USED
        //CAN'T REMOVE VIA EF UNTIL SQLITE ALLOWS ALTER TABLE DROP COLUMN
        //DON"T REMEMBER THIS VARIABLE ELSE EF WILL FAIL TO SAVE NEW PLAYERS
        [JsonIgnore]
        public long SpellCd { get; set; }

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
