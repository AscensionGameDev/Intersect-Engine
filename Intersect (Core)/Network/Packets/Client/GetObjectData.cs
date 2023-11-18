using Intersect.GameObjects.Maps;
using Intersect.Models;
using MessagePack;

namespace Intersect.Network.Packets.Client;

[GenericPacketTypeArguments(typeof(MapBase))]
[MessagePackObject]
public partial class GetObjectData<TGameObject> : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public GetObjectData()
    {
    }

    public GetObjectData(params ObjectCacheKey<TGameObject>[] cacheKeys)
    {
        CacheKeys = new List<ObjectCacheKey<TGameObject>>(cacheKeys);
    }

    [Key(0)] public List<ObjectCacheKey<TGameObject>> CacheKeys { get; set; }
}