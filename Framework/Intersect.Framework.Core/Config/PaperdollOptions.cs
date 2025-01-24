using System.Runtime.Serialization;
using Intersect.Framework.Annotations;
using Newtonsoft.Json;

namespace Intersect.Config;

public partial class PaperdollOptions
{
    [Ignore]
    [JsonIgnore]
    public List<string>[] Directions;

    public List<string> Down { get; set; } =
    [
        "Player",
        "Armor",
        "Helmet",
        "Weapon",
        "Shield",
        "Boots",
    ];

    public List<string> Left { get; set; } =
    [
        "Player",
        "Armor",
        "Helmet",
        "Weapon",
        "Shield",
        "Boots",
    ];

    public List<string> Right { get; set; } =
    [
        "Player",
        "Armor",
        "Helmet",
        "Weapon",
        "Shield",
        "Boots",
    ];

    public List<string> Up { get; set; } =
    [
        "Player",
        "Armor",
        "Helmet",
        "Weapon",
        "Shield",
        "Boots",
    ];

    public PaperdollOptions()
    {
        Directions =
        [
            Up,
            Down,
            Left,
            Right,
        ];
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
        Up = [..Up.Distinct()];
        Down = [..Down.Distinct()];
        Left = [..Left.Distinct()];
        Right = [..Right.Distinct()];
        Directions =
        [
            Up,
            Down,
            Left,
            Right,
        ];
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
