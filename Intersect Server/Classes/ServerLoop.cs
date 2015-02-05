using System;
using System.Threading;

namespace Intersect_Server.Classes
{
    class ServerLoop
    {
        public ServerLoop(Network nb)
        {
            long timeUpdateTick = Environment.TickCount + 1000;
            
            for (var i = 0; i < 5; i++)
            {
                var slot = Globals.FindOpenEntity();
                Globals.Entities[slot] = new Npc(slot, Globals.GameNpcs[0])
                {
                    CurrentMap = 0,
                    CurrentX = 2 + i*2,
                    CurrentY = 4,
                    Dir = 1
                };
            }
            while (true)
            {
                nb.RunServer();
                foreach (var t in Globals.GameMaps)
                {
                    if (t.Active)
                    {
                        t.Update();
                    }
                }
                if (timeUpdateTick < Environment.TickCount)
                {
                    Globals.GameTime++;
                    if (Globals.GameTime >= 2400) { Globals.GameTime = 0; }
                    timeUpdateTick = Environment.TickCount + 1000;
                }

                Thread.Sleep(10);

            }
        }
    }
}
