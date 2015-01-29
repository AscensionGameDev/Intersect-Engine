using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectServer
{
    public class NPCBase
    {
        //Core info
		public string myName = "";
        public string mySprite = "";
		public int myNpcNum = 0;

        //Vitals & Stats
        public int[] maxVital = new int[2];
        public int[] vital = new int[2];
        public int[] stat = new int[3];

        //Standard Info
		public long deleted = 0;


		public NPCBase(int npcNum)
		{
			if (npcNum == -1) {
				return;
			}
			myNpcNum = npcNum;
			
		}

        public void Load(byte[] packet)
        {
            ByteBuffer bf;
            bf = new ByteBuffer();
            bf.WriteBytes(packet);
            //Load NPC Stufffff

            deleted = bf.ReadLong();
        }
    }
}
