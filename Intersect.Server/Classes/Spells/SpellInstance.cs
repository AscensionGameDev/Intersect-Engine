namespace Intersect.Server.Spells
{
    public class SpellInstance
    {
        public long SpellCd;
        public int SpellNum = -1;

        public SpellInstance()
        {
        }

        public SpellInstance(int num)
        {
            SpellNum = num;
        }

        public SpellInstance Clone()
        {
            SpellInstance newSpell = new SpellInstance()
            {
                SpellNum = SpellNum,
                SpellCd = SpellCd
            };
            return newSpell;
        }
    }
}