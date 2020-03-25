using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Intersect.Server.Entities
{
    public struct Label
    {
        [JsonProperty("Label")]
        public string Text;
        public Color Color;

        public Label(string label, Color color)
        {
            Text = label;
            Color = color;
        }
    }
}
