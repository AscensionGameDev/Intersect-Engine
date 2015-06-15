using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes.NPCs
{
    public class NpcStruct
    {
        //Core info
        public string Name = "";
        public string Sprite = "";

        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];
        public int Experience = 0;

        //Basic Info
        public int SpawnDuration = 0;
        public byte Behavior = 0;
        public int SightRange = 0;

        //Drops
        public List<NPCDrop> Drops = new List<NPCDrop>();


        public NpcStruct()
        {
            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Drops.Add(new NPCDrop());
            }

        }

        public void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Sprite = myBuffer.ReadString();
            for (int i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                MaxVital[i] = myBuffer.ReadInteger();
            }
            for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                Stat[i] = myBuffer.ReadInteger();
            }
            Experience = myBuffer.ReadInteger();
            SpawnDuration = myBuffer.ReadInteger();
            Behavior = myBuffer.ReadByte();
            SightRange = myBuffer.ReadInteger();
            for (int i = 0; i < Constants.MaxNpcDrops; i++)
            {
                Drops[i].ItemNum = myBuffer.ReadInteger();
                Drops[i].Amount = myBuffer.ReadInteger();
                Drops[i].Chance = myBuffer.ReadInteger();
            }

            myBuffer.Dispose();
        }
    }

    public class NPCDrop
    {
        public int ItemNum;
        public int Amount;
        public int Chance;

    }
}
