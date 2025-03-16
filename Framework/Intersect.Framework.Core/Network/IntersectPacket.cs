using Intersect.Collections;
using Intersect.Framework.Reflection;
#if DIAGNOSTIC

#endif
using MessagePack;

namespace Intersect.Network;

public abstract partial class IntersectPacket : IPacket
{
    [IgnoreMember]
    private byte[]? _cachedData;

    [IgnoreMember]
    private byte[]? _cachedCompressedData = null;

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    [IgnoreMember]
    public virtual byte[] Data
    {
        get
        {
            _cachedData ??= MessagePacker.Instance.Serialize(this) ??
                                   throw new Exception($"Failed to serialize {this.GetFullishName()}");

#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"{GetType().FullName}({mCachedData.Length})={Convert.ToHexString(mCachedData)}");
#endif
            return _cachedData;
        }
    }

    public virtual void ClearCachedData()
    {
        _cachedData = null;
    }

    [IgnoreMember]
    public virtual bool IsValid => true;
    [IgnoreMember]
    public virtual long ReceiveTime { get; set; }
    [IgnoreMember]
    public virtual long ProcessTime { get; set; }

    /// <inheritdoc />
    public virtual Dictionary<string, SanitizedValue<object>> Sanitize()
    {
        return null;
    }

}
