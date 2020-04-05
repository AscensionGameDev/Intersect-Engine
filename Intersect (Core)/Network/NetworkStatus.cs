namespace Intersect.Network
{
    public enum NetworkStatus
    {

        Unknown = 0,

        Connecting,

        Online,

        Offline,

        Failed,

        VersionMismatch,

        ServerFull,
        
        HandshakeFailure,

        Quitting,

    }
}
