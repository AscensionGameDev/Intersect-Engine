using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Intersect.Config
{

    public class PaperdollOptions
    {

        [JsonIgnore] public List<string>[] Directions;

        public List<string> Down = new List<string>()
        {
            "Player",
            "Armor",
            "Helmet",
            "Weapon",
            "Shield",
            "Boots"
        };

        public List<string> Left = new List<string>()
        {
            "Player",
            "Armor",
            "Helmet",
            "Weapon",
            "Shield",
            "Boots"
        };

        public List<string> Right = new List<string>()
        {
            "Player",
            "Armor",
            "Helmet",
            "Weapon",
            "Shield",
            "Boots"
        };

        public List<string> Up = new List<string>()
        {
            "Player",
            "Armor",
            "Helmet",
            "Weapon",
            "Shield",
            "Boots"
        };

        public PaperdollOptions()
        {
            Directions = new List<string>[]
            {
                Up,
                Down,
                Left,
                Right
            };
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Up.Clear();
            Down.Clear();
            Left.Clear();
            Right.Clear();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Up = new List<string>(Up.Distinct());
            Down = new List<string>(Down.Distinct());
            Left = new List<string>(Left.Distinct());
            Right = new List<string>(Right.Distinct());
            Directions = new List<string>[]
            {
                Up,
                Down,
                Left,
                Right
            };
        }

        public void Validate(EquipmentOptions equipment)
        {
            foreach (var direction in Directions)
            {
                var hasPlayer = false;
                foreach (var item in direction)
                {
                    if (item == "Player")
                    {
                        hasPlayer = true;
                    }

                    if (!equipment.Slots.Contains(item) && item != "Player")
                    {
                        throw new Exception($"Config Error: Paperdoll item {item} does not exist in equipment slots!");
                    }
                }

                if (!hasPlayer)
                {
                    throw new Exception($"Config Error: Paperdoll direction {direction} does not have Player listed!");
                }
            }
        }

    }

}
