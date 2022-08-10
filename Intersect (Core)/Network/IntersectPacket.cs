using Intersect.Collections;

using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public abstract partial class IntersectPacket : IPacket
{
    [IgnoreMember]
    private byte[]? mCachedData = default;

    [IgnoreMember]
    private byte[]? mCachedCompresedData = default;

    /// <inheritdoc />
    public virtual void Dispose() { }

    /// <inheritdoc />
    [IgnoreMember]
    public virtual byte[] Data =>
        mCachedData
            ??= MessagePacker.Instance.Serialize(this)
            ?? throw new InvalidOperationException("Failed to serialize packet.");

    [IgnoreMember]
    public virtual bool IsValid => true;

    [IgnoreMember]
    public virtual long ReceiveTime { get; set; }

    [IgnoreMember]
    public virtual long ProcessTime { get; set; }

    public virtual void ClearCachedData() => mCachedData = default;

    /// <inheritdoc />
    public virtual Dictionary<string, SanitizedValue<object>>? Sanitize() => default;
}
