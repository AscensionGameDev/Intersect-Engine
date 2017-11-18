using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intersect.Config
{
    public class EquipmentOptions
    {
        public int WeaponSlot = 2;
        public int ShieldSlot = 3;
        public List<string> Slots = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Helmet",
            "Shield"
        };

        public PaperdollOptions Paperdoll = new PaperdollOptions();

        public List<string> ToolTypes = new List<string>()
        {
            "Axe",
            "Pickaxe",
            "Shovel",
            "Fishing Rod"
        };

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Slots.Clear();
            ToolTypes.Clear();
        }

    public bool Validate()
        {
            //TODO Validate loaded input
            return true;
        }
    }
}
