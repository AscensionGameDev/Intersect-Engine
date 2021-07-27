using System;

namespace Intersect.Client.Framework.Spells
{
    public interface ISpell
    {
        Guid SpellId { get; set; }

        ISpell Clone();
        void Load(Guid spellId);
    }
}