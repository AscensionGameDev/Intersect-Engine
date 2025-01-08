using Newtonsoft.Json;

namespace Intersect.Server.Entities;


public partial struct Label(string label, Color color)
{
    [JsonProperty(nameof(Label))]
    public readonly string Text = label;

    public readonly Color Color = color;
}
