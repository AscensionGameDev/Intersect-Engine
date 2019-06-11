using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Editor.Classes.Maps
{
    public class MapSaveState
    {
        public string Metadata { get; set; }
        public byte[] Tiles { get; set; }
        public byte[] Attributes { get; set; }

        public MapSaveState(string metadata, byte[] tiles, byte[] attributes)
        {
            Metadata = metadata;
            Tiles = tiles;
            Attributes = attributes;
        }

        public bool Matches(MapSaveState otherState)
        {
            return Metadata != otherState.Metadata || !Tiles.SequenceEqual(otherState.Tiles) || !Attributes.SequenceEqual(otherState.Attributes);
        }
    }
}
