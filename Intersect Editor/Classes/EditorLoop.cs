using System;
using System.Threading;
using System.Windows.Forms;
using Intersect_Editor.Forms;

namespace Intersect_Editor.Classes
{
    public static class EditorLoop
    {
        public static void StartLoop(FrmMain myForm)
        {
            long animationTimer = Environment.TickCount;
            long waterfallTimer = Environment.TickCount;
            // drawing loop
            while (myForm.Visible) // loop while the window is open
            {
                if (Globals.GameMaps[Globals.CurrentMap] != null)
                {
                    if (myForm.Text != @"Intersect Editor - Map# " + Globals.CurrentMap + @" " + Globals.GameMaps[Globals.CurrentMap].MyName + @" Revision: " + Globals.GameMaps[Globals.CurrentMap].Revision + @" CurX: " + Globals.CurTileX + @" CurY: " + Globals.CurTileY)
                    {
                        myForm.Text = @"Intersect Editor - Map# " + Globals.CurrentMap + @" " + Globals.GameMaps[Globals.CurrentMap].MyName + @" Revision: " + Globals.GameMaps[Globals.CurrentMap].Revision + @" CurX: " + Globals.CurTileX + @" CurY: " + Globals.CurTileY;
                    }
                }
                if (waterfallTimer < Environment.TickCount)
                {
                    Globals.WaterfallFrame++;
                    if (Globals.WaterfallFrame == 3) { Globals.WaterfallFrame = 0; }
                    waterfallTimer = Environment.TickCount + 500;
                }
                if (animationTimer < Environment.TickCount)
                {
                    Globals.AutotileFrame++;
                    if (Globals.AutotileFrame == 3) { Globals.AutotileFrame = 0; }
                    animationTimer = Environment.TickCount + 600;
                }
                Graphics.Render();
                Application.DoEvents(); // handle form events
                Thread.Sleep(10);
            }
        }
    }
}
