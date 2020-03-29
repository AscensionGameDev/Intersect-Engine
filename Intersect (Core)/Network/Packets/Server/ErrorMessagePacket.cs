namespace Intersect.Network.Packets.Server
{

    public class ErrorMessagePacket : CerasPacket
    {

        public ErrorMessagePacket(string header, string error)
        {
            Header = header;
            Error = error;
        }

        public string Header { get; set; }

        public string Error { get; set; }

    }

}
