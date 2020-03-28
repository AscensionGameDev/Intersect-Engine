using Intersect.Memory;

namespace Intersect.Network.Crypto
{

    internal interface IKeyEncoder
    {

        IBuffer Encode(IBuffer source, IBuffer destination);

        IBuffer Decode(IBuffer source, IBuffer destination);

    }

}
