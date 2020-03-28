namespace Intersect.Network.Packets.Editor
{

    public class SaveTimeDataPacket : EditorPacket
    {

        public SaveTimeDataPacket(string timeJson)
        {
            TimeJson = timeJson;
        }

        public string TimeJson { get; set; }

    }

}
