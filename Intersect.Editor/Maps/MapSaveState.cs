using System.Linq;

namespace Intersect.Editor.Classes.Maps
{

    public class MapSaveState
    {

        public MapSaveState(string metadata, byte[] tiles, byte[] attributes, string eventData)
        {
            Metadata = metadata;
            Tiles = tiles;
            Attributes = attributes;
            EventData = eventData;
        }

        public string Metadata { get; set; }

        public byte[] Tiles { get; set; }

        public byte[] Attributes { get; set; }

        public string EventData { get; set; }

        public bool Matches(MapSaveState otherState)
        {
            return Metadata == otherState.Metadata &&
                   Tiles.SequenceEqual(otherState.Tiles) &&
                   Attributes.SequenceEqual(otherState.Attributes) &&
                   EventData.SequenceEqual(otherState.EventData);
        }

    }

}
