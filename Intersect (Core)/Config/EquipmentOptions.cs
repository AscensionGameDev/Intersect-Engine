using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Intersect.Config
{

    public class EquipmentOptions
    {

        public PaperdollOptions Paperdoll = new PaperdollOptions();

        public int ShieldSlot = 3;

        public List<string> Slots = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Shield",
            "Boots",
        };

        public List<string> ToolTypes = new List<string>()
        {
            "Axe",
            "Pickaxe",
            "Shovel",
            "Fishing Rod"
        };

        public int WeaponSlot = 2;

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Slots.Clear();
            ToolTypes.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            Slots = new List<string>(Slots.Distinct());
            ToolTypes = new List<string>(ToolTypes.Distinct());
            if (WeaponSlot < -1 || WeaponSlot > Slots.Count - 1)
            {
                throw new Exception("Config Error: (WeaponSlot) was out of bounds!");
            }

            if (ShieldSlot < -1 || ShieldSlot > Slots.Count - 1)
            {
                throw new Exception("Config Error: (ShieldSlot) was out of bounds!");
            }

            Paperdoll.Validate(this);
        }

    }

}
