/*
    Intersect Game Engine (Editor)
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
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Forms;

namespace Intersect_Editor.Classes
{
    public static class EditorLoop
    {
        private static int _fps = 0;
        private static int _fpsCount = 0;
        private static long _fpsTime = 0;
        public static void StartLoop()
        {
            Globals.MainForm.Visible = true;
            Globals.MainForm.EnterMap(Globals.CurrentMap == null ? 0 : Globals.CurrentMap.GetId());
            frmMain myForm = Globals.MainForm;
            long animationTimer = Environment.TickCount;
            long waterfallTimer = Environment.TickCount;
            // drawing loop
            while (myForm.Visible) // loop while the window is open
            {
                myForm.Update();

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
                EditorGraphics.Render();
                GameContentManager.Update();
                Network.Update();
                Application.DoEvents(); // handle form events

                _fpsCount++;
                if (_fpsTime < Environment.TickCount)
                {
                    _fps = _fpsCount;
                    myForm.toolStripLabelFPS.Text = @"FPS: " + _fps;
                    _fpsCount = 0;
                    _fpsTime = Environment.TickCount + 1000;
                }
            }
        }
    }
}
