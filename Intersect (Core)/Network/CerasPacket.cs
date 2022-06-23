using System;
using System.Collections.Generic;

using Intersect.Collections;

namespace Intersect.Network
{

    public abstract partial class CerasPacket : IPacket
    {
        private static Ceras sCerasInstance { get; set; }

        private static Ceras Ceras => (sCerasInstance = (sCerasInstance ?? new Ceras(true)));

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <inheritdoc />
        public virtual byte[] Data => Ceras.Serialize(this) ?? throw new Exception("Failed to serialize packet.");

        public virtual bool IsValid => true;

        public abstract long ReceiveTime { get; set; }

        public abstract long ProcessTime { get; set; }

        /// <inheritdoc />
        public virtual Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            return null;
        }

    }

}
