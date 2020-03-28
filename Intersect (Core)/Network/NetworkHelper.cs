namespace Intersect.Network
{

    public static class NetworkHelper
    {

        public static bool IsValidPort(ulong port)
        {
            return 0 < port && port <= ushort.MaxValue;
        }

    }

}
