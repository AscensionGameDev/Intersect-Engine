using System.Collections.Generic;
using System.Runtime.Serialization;
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
            Directions = new List<string>[]
            {
                Up,
                Down,
                Left,
                Right
            };
        }
    }
}
