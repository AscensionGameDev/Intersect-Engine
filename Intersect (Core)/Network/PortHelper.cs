namespace Intersect.Network
{

    public static partial class PortHelper
    {

        public static bool IsValidPort(ulong port)
        {
            return 0 < port && port <= ushort.MaxValue;
        }

    }

}
