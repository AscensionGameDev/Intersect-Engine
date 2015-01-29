using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersectServer
{
    class ServerLoop
    {
        public ServerLoop(NetworkBase nb)
        {
            int slot;
            int mapUpdateCount;
            long timeUpdateTick = Environment.TickCount + 1000;
            
            for (int i = 0; i < 5; i++)
            {
                slot = GlobalVariables.findOpenEntity();
                GlobalVariables.entities[slot] = new NPC(slot, GlobalVariables.GameNpcs[0]);
                GlobalVariables.entities[slot].currentMap = 0;
                GlobalVariables.entities[slot].currentX = 2 + i * 2;
                GlobalVariables.entities[slot].currentY = 4;
                GlobalVariables.entities[slot].dir = 1;
            }
            while (true)
            {
                nb.runServer();
                mapUpdateCount = 0;
                for (int i = 0; i < GlobalVariables.GameMaps.Length; i++)
                {
                    if (GlobalVariables.GameMaps[i].active)
                    {
                        GlobalVariables.GameMaps[i].Update();
                        mapUpdateCount++;
                    }
                }
                if (timeUpdateTick < Environment.TickCount)
                {
                    GlobalVariables.GameTime++;
                    if (GlobalVariables.GameTime >= 2400) { GlobalVariables.GameTime = 0; }
                    timeUpdateTick = Environment.TickCount + 1000;
                }

                System.Threading.Thread.Sleep(10);

            }
        }
    }
}
