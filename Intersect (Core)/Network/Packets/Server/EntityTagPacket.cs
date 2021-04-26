using System;
using Intersect.Enums;
using Newtonsoft.Json;

namespace Intersect.Network.Packets.Server
{
    [MessagePack.MessagePackObject]
    public class EntityTagPacket : IntersectPacket
    {
        [MessagePack.Key(0)] [JsonProperty("Tag")]
        public string FileName;

        [MessagePack.Key(1)] public TagPosition TagPos;

        public EntityTagPacket(string fileName, TagPosition tagPos)
        {
            FileName = fileName;
            TagPos = tagPos;
        }
    }
}
