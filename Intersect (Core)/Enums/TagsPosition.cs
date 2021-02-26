using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Enums
{
    /// <summary> Enum used for switching Tags Position. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TagPosition
    {
        /// <summary> Positions the tag above the name label. </summary>
        Above,
        /// <summary> Positions the tag under the name label. </summary>
        Under,
        /// <summary> Positions the tag as prefix (left) of the name label. </summary>
        Prefix,
        /// <summary> Positions the tag as suffix (right) of the name label. </summary>
        Suffix
    }
}
