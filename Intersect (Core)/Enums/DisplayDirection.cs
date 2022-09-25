using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Intersect.Enums
{
    /// <summary>
    /// Defines the direction from which bars are filled up.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DisplayDirection
    {
        /// <summary>
        /// <para>The bar will be filled from:</para> start (left: empty life) to end (right: full life).
        /// </summary>
        StartToEnd = 0,

        /// <summary>
        /// <para>The bar will be filled from:</para> end (right: empty life) to start (left: full life).
        /// </summary>
        EndToStart,

        /// <summary>
        /// <para>The bar will be filled from:</para> top (empty life) to bottom (full life).
        /// </summary>
        TopToBottom,

        /// <summary>
        /// <para>The bar will be filled from:</para> bottom (empty life) to top (full life).
        /// </summary>
        BottomToTop,
    }
}
