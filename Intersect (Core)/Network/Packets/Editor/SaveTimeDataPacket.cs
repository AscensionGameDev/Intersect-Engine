using MessagePack;

namespace Intersect.Network.Packets.Editor
{
    [MessagePackObject]
    public class SaveTimeDataPacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public SaveTimeDataPacket()
        {
        }

        public SaveTimeDataPacket(string timeJson)
        {
            TimeJson = timeJson;
        }

        [Key(0)]
        public string TimeJson { get; set; }

    }

}
