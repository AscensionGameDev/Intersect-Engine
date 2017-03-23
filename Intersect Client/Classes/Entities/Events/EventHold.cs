namespace Intersect_Client.Classes.Entities
{
    public class EventHold
    {
        public int EventIndex;
        public int MapNum;

        public EventHold(int mapNum, int eventIndex)
        {
            MapNum = mapNum;
            EventIndex = eventIndex;
        }
    }
}