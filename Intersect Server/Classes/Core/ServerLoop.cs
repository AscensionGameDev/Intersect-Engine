/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Threading;

namespace Intersect_Server.Classes
{
    public static class ServerLoop
    {
        public static void RunServerLoop(Network nb)
        {
            long timeUpdateTick = Environment.TickCount + 1000;
            while (true)
            {
                nb.RunServer();
                for (int i = 0; i < Globals.GameMaps.Length; i++)
                {
                    if (Globals.GameMaps[i] != null)
                    {
                        if (Globals.GameMaps[i].Active)
                        {
                            Globals.GameMaps[i].Update();
                        }
                    }
                }

                for (int i = 0; i < Globals.Clients.Count; i++)
                {
                    if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                    {
                        Globals.Clients[i].Entity.Update();
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
