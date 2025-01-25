using System.Buffers.Binary;
using System.Numerics;
using MessagePack;
using System.Security.Cryptography;
using Intersect.GameObjects.Maps;
using Intersect.Models;
using Newtonsoft.Json.Converters;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class MapPacket : IntersectPacket
{
    [IgnoreMember]
    private string? _checksum;

    [IgnoreMember]
    private string? _version;

    // Parameterless Constructor for MessagePack
    public MapPacket()
    {
    }

    public MapPacket(
        Guid mapId,
        bool deleted,
        string data = null,
        byte[] tileData = null,
        byte[] attributeData = null,
        int revision = -1,
        int gridX = -1,
        int gridY = -1,
        bool[]? borders = null
    )
    {
        MapId = mapId;
        Deleted = deleted;
        Data = data;
        TileData = tileData;
        AttributeData = attributeData;
        Revision = revision;
        GridX = gridX;
        GridY = gridY;
        CameraHolds = borders;
    }

    [Key(0)]
    public Guid MapId { get; set; }

    [Key(1)]
    public bool Deleted { get; set; }

    [Key(2)]
    public string Data { get; set; }

    [Key(3)]
    public byte[] TileData { get; set; }

    [Key(4)]
    public byte[] AttributeData { get; set; }

    [Key(5)]
    public int Revision { get; set; }

    [Key(6)]
    public int GridX { get; set; }

    [Key(7)]
    public int GridY { get; set; }

    [Key(8)]
    public bool[]? CameraHolds { get; set; }

    [IgnoreMember]
    public string? CacheChecksum
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_checksum))
            {
                return _checksum;
            }

            _checksum = ObjectCacheKey<MapBase>.ComputeChecksum(base.Data);
            return _checksum;
        }
    }

    [IgnoreMember]
    public string? CacheVersion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_version))
            {
                return _version;
            }

            _version = ComputeCacheVersion(MapId, Revision, GridX, GridY, CameraHolds);
            return _version;
        }
    }

    public static string ComputeCacheVersion(Guid id, int revision, int gridX, int gridY, bool[]? cameraHolds)
    {
        var hashInputData = id.ToByteArray()
            .Concat(BitConverter.GetBytes(revision))
            .Concat(BitConverter.GetBytes(gridX))
            .Concat(BitConverter.GetBytes(gridY))
            .Concat(cameraHolds?.SelectMany(BitConverter.GetBytes) ?? [])
            .ToArray();
        var versionData = SHA256.HashData(hashInputData);
        var version = Convert.ToBase64String(versionData);
        return version;
    }
}