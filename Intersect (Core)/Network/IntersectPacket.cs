using System;
using System.Collections.Generic;

using Intersect.Collections;
using MessagePack;

namespace Intersect.Network
{
    [MessagePackObject]
    public abstract class IntersectPacket : IPacket
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
                if (mCachedData == null)
                    mCachedData = MessagePacker.Instance.Serialize(this) ?? throw new Exception("Failed to serialize packet.");

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

}
