using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Enums
{
    /// <summary> Used for switching the display mode of fullscreen textures. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DisplayModes
    {
        /// <summary> Default display mode of fullscreen textures. </summary>
        Default,
        
        /// <summary> Positions the sprite in the center of the game window. </summary>
        Center,

        /// <summary> Stretches the sprite to the game window size. </summary>
        Stretch,

        /// <summary> Fits the sprite to the game window height. </summary>
        FitHeight,

        /// <summary> Fits the sprite to the game window width. </summary>
        FitWidth
    }
}
