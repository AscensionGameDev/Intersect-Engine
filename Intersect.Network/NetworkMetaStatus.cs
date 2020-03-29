namespace Intersect.Network
{

    public enum NetworkMetaStatus
    {

        Unknown = 0,

        ConnectionEstablished = 1,

        HandshakeRequested = 2,

        HandshakeReceived = 4,

        HandshakeCompleted = 8,

        Connected = 16

    }

}
