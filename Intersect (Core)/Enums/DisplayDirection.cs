using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Intersect.Enums
{
    /// <summary>
    /// Defines the way the bar will be rendered
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DisplayDirection
    {
        /// <summary>
        /// The bar will render from start (empty life) to end (full life)
        /// </summary>
        StartToEnd = 0,
        /// <summary>
        /// The bar will render from end (empty life) to start (full life)
        /// </summary>
        EndToStart,
        /// <summary>
        /// The bar will render from top (empty life) to bottom (full life)
        /// </summary>
        TopToBottom,
        /// <summary>
        /// The bar will render from bottom (empty life) to top (full life)
        /// </summary>
        BottomToTop,
    }
}
