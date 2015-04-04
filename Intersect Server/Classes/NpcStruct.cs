namespace Intersect_Server.Classes
{
    public class NpcStruct
    {
        //Core info
		public string MyName = "";
        public string MySprite = "";
		public int MyNpcNum;

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Vital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];

        //Standard Info
		public long Deleted;


		public NpcStruct(int npcNum)
		{
			if (npcNum == -1) {
				return;
			}
			MyNpcNum = npcNum;
			
		}

        public void Load(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            //Load NPC Stufffff

            Deleted = bf.ReadLong();
        }
    }
}
