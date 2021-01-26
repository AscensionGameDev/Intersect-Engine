using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ErrorMessagePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ErrorMessagePacket()
        {
        }

        public ErrorMessagePacket(string header, string error)
        {
            Header = header;
            Error = error;
        }

        [Key(0)]
        public string Header { get; set; }

        [Key(1)]
        public string Error { get; set; }

    }

}
