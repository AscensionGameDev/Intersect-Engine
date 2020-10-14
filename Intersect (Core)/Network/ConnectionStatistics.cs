namespace Intersect.Network
{
    public class ConnectionStatistics
    {
        public long Ping { get; set; }

        public long DroppedMessages { get; set; }

        public long ReceivedPackets { get; set; }

        public long ReceivedBytes { get; set; }

        public long ReceivedMessages { get; set; }

        public long ResentMessages { get; set; }

        public long SentPackets { get; set; }

        public long SentBytes { get; set; }

        public long SentMessages { get; set; }
    }
}
