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
using System.Windows.Forms;
using Intersect_Editor.Forms;

namespace Intersect_Editor.Classes
{
    public static class EditorLoop
    {
        public static void StartLoop()
        {
            Globals.MainForm.Visible = true;
            FrmMain myForm = Globals.MainForm;
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
                //Check Editors
                if (Globals.ResourceEditor != null && Globals.ResourceEditor.IsDisposed == false)
                {
                    Globals.ResourceEditor.Render();
                }
                Graphics.Render();
                Network.Update();
                Application.DoEvents(); // handle form events
                Thread.Sleep(10);
            }
        }
    }
}
