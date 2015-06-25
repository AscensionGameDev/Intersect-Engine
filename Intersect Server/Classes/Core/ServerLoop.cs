using System;
using System.Threading;

namespace Intersect_Server.Classes
{
    class ServerLoop
    {
        public ServerLoop(Network nb)
        {
            long timeUpdateTick = Environment.TickCount + 1000;
            while (true)
            {
                nb.RunServer();
                foreach (var map in Globals.GameMaps)
                {
                    if (map.Active)
                    {
                        map.Update();
                    }
                }

                foreach (var player in Globals.Clients)
                {
                    if (player != null && player.Entity != null)
                    {
                        player.Entity.Update();
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
