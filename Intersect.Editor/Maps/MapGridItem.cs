using System;

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
