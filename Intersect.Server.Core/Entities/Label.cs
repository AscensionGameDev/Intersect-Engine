using Newtonsoft.Json;

namespace Intersect.Server.Entities;


public partial struct Label(string label, Color color)
{
    [JsonProperty(nameof(Label))]
    public string Text = label;

    public Color Color = color;
}
