using Intersect.Collections;
using Intersect.Framework.Reflection;
#if DIAGNOSTIC

#endif
using MessagePack;

namespace Intersect.Network;

[MessagePackObject]
public abstract partial class IntersectPacket : IPacket
{
    [IgnoreMember]
    private byte[] mCachedData = null;

    [IgnoreMember]
    private byte[] mCachedCompresedData = null;

    /// <inheritdoc />
    public virtual void Dispose()
    {
    }

    /// <inheritdoc />
    [IgnoreMember]
    public virtual byte[] Data
    {
        get
        {
            mCachedData ??= MessagePacker.Instance.Serialize(this) ??
                                   throw new Exception($"Failed to serialize {this.GetFullishName()}");

#if DIAGNOSTIC
            ApplicationContext.Context.Value?.Logger.LogDebug($"{GetType().FullName}({mCachedData.Length})={Convert.ToHexString(mCachedData)}");
#endif
            return mCachedData;
        }
    }

    public virtual void ClearCachedData()
    {
        mCachedData = null;
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
