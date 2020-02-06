using Intersect.Collections;

using System;
using System.Collections.Generic;

namespace Intersect.Network
{
    public interface IPacket : IDisposable
    {
        byte[] Data { get; }

        bool IsValid { get; }

        Dictionary<string, SanitizedValue<object>> Sanitize();

    }
}