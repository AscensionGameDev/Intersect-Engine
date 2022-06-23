using Newtonsoft.Json;

namespace Intersect.Client.Framework.Entities
{

    public partial class Label
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
