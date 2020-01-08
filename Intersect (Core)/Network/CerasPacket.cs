using System;
using System.Collections.Generic;

using Intersect.Collections;

using JetBrains.Annotations;

namespace Intersect.Network
{
    public abstract class CerasPacket : IPacket
    {
        [NotNull] private static readonly Ceras sCerasInstance = new Ceras(true);

        protected CerasPacket()
        {
        }


        /// <inheritdoc />
        public virtual void Dispose() => throw new NotImplementedException();

        /// <inheritdoc />
        public byte[] Data => sCerasInstance.Serialize(this);

        /// <inheritdoc />
        public virtual Dictionary<string, SanitizedValue<object>> Sanitize() => null;

    }
}
