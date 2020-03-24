using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Client.Entities
{
    public struct Label
    {
        public string Text;
        public Color Color;

        public Label(string label, Color color)
        {
            Text = label;
            Color = color;
        }
    }
}
