using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class NpcEntityPacket : EntityPacket
    {
        //Parameterless Constructor for MessagePack
        public NpcEntityPacket()
        {
        }


        [Key(25)]
        public int Aggression { get; set; }

    }

}
