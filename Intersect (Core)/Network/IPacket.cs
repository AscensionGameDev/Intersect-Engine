using System;
using System.Collections.Generic;

using Intersect.Collections;

namespace Intersect.Network
{

    public interface IPacket : IDisposable
    {

        byte[] Data { get; }

        bool IsValid { get; }

        long ReceiveTime { get; set; }

        long ProcessTime { get; set; }

        Dictionary<string, SanitizedValue<object>> Sanitize();

    }

}
