using Intersect.Client.Framework.Entities;
using Newtonsoft.Json;

namespace Intersect.Client.Entities
{

    public struct Label : ILabel
    {

        [JsonProperty("Label")] public string Text { get; set; }

        public Color Color { get; set; }

        public Label(string label, Color color)
        {
            Text = label;
            Color = color;
        }

    }

}
