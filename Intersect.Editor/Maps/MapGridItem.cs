using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Maps
{
    public class MapGridItem
    {
        public MapGridItem(Guid id, string name = "", int revision = 0)
        {
            MapId = id;
            this.Name = name;
            this.Revision = revision;
        }

        public string Name { get; set; }
        public int Revision { get; set; }
        public Guid MapId { get; set; }
        public Texture2D Tex { get; set; }
    }
}
