using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Editor
{
    public static class EditorLoop
    {
        public static void startLoop(frmMain myForm)
        {
            long animationTimer = Environment.TickCount;
            long waterfallTimer = Environment.TickCount;
            // drawing loop
            while (myForm.Visible) // loop while the window is open
            {
                if (Globals.GameMaps[Globals.currentMap] != null)
                {
                    if (myForm.Text != "Intersect Editor - Map# " + Globals.currentMap + " " + Globals.GameMaps[Globals.currentMap].myName + " Revision: " + Globals.GameMaps[Globals.currentMap].revision + " CurX: " + Globals.curTileX + " CurY: " + Globals.curTileY)
                    {
                        myForm.Text = "Intersect Editor - Map# " + Globals.currentMap + " " + Globals.GameMaps[Globals.currentMap].myName + " Revision: " + Globals.GameMaps[Globals.currentMap].revision + " CurX: " + Globals.curTileX + " CurY: " + Globals.curTileY;
                    }
                }
                if (waterfallTimer < Environment.TickCount)
                {
                    Globals.waterfallFrame++;
                    if (Globals.waterfallFrame == 3) { Globals.waterfallFrame = 0; }
                    waterfallTimer = Environment.TickCount + 500;
                }
                if (animationTimer < Environment.TickCount)
                {
                    Globals.autotileFrame++;
                    if (Globals.autotileFrame == 3) { Globals.autotileFrame = 0; }
                    animationTimer = Environment.TickCount + 600;
                }
                GFX.Render();
                System.Windows.Forms.Application.DoEvents(); // handle form events
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
