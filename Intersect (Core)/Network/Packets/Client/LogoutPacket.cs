using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class LogoutPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public LogoutPacket()
        {
        }

        public LogoutPacket(bool returnToCharSelect, bool returnToMainMenu)
        {
            ReturningToCharSelect = returnToCharSelect;
            ReturningToMainMenu = returnToMainMenu;
        }

        [Key(0)]
        public bool ReturningToCharSelect { get; set; }

        [Key(1)]
        public bool ReturningToMainMenu { get; set; }

    }

}
