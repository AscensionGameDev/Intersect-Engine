using System;
using System.Collections.Generic;

using Intersect.Collections;

using JetBrains.Annotations;

namespace Intersect.Network
{

    public abstract class CerasPacket : IPacket
    {
        [NotNull] private static readonly Ceras sCerasInstance = new Ceras(true);

        /// <inheritdoc />
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual byte[] Data => sCerasInstance.Serialize(this);

        public virtual bool IsValid => true;

        /// <inheritdoc />
        public virtual Dictionary<string, SanitizedValue<object>> Sanitize()
        {
            return null;
        }

    }

}
