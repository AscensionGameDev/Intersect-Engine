using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Intersect.Config
{
    public class PaperdollOptions
    {
        public List<string> Up = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Helmet",
            "Shield"
        };
        public List<string> Down = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Helmet",
            "Shield"
        };
        public List<string> Left = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Helmet",
            "Shield"
        };
        public List<string> Right = new List<string>()
        {
            "Helmet",
            "Armor",
            "Weapon",
            "Helmet",
            "Shield"
        };

        [JsonIgnore] public List<string>[] Directions;

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
                foreach (var item in direction)
                {
                    if (!equipment.Slots.Contains(item))
                    {
                        throw new Exception($"Config Error: Paperdoll item {item} does not exist in equipment slots!");
                    }
                }

            }
        }
    }
}
