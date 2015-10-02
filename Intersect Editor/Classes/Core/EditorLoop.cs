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
using System.Threading;
using System.Windows.Forms;
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
            Globals.MainForm.EnterMap(Globals.CurrentMap);
            frmMain myForm = Globals.MainForm;
            long animationTimer = Environment.TickCount;
            long waterfallTimer = Environment.TickCount;
            // drawing loop
            while (myForm.Visible) // loop while the window is open
            {
                if (Globals.CurrentMap > -1)
                {
                    if (Globals.GameMaps[Globals.CurrentMap] != null)
                    {
                        myForm.toolStripLabelCoords.Text = @" CurX: " + Globals.CurTileX + @" CurY: " + Globals.CurTileY;
                        myForm.toolStripLabelRevision.Text = @"Revision: " + Globals.GameMaps[Globals.CurrentMap].Revision;
                        if (myForm.Text != @"Intersect Editor - Map# " + Globals.CurrentMap + @" " + Globals.GameMaps[Globals.CurrentMap].MyName)
                        {
                            myForm.Text = @"Intersect Editor - Map# " + Globals.CurrentMap + @" " + Globals.GameMaps[Globals.CurrentMap].MyName;
                        }
                    }
                }

                //Process the Undo/Redo Buttons
                if (Globals.MapEditorWindow.MapUndoStates.Count > 0)
                {
                    myForm.toolStripBtnUndo.Enabled = true;
                }
                else
                {
                    myForm.toolStripBtnUndo.Enabled = false;
                }
                if (Globals.MapEditorWindow.MapRedoStates.Count > 0)
                {
                    myForm.toolStripBtnRedo.Enabled = true;
                }
                else
                {
                    myForm.toolStripBtnRedo.Enabled = false;
                }

                //Process the Fill/Erase Buttons
                if (Globals.CurrentLayer <= Constants.LayerCount)
                {
                    myForm.toolStripBtnFill.Enabled = true;
                    myForm.fillToolStripMenuItem.Enabled = true;
                    myForm.toolStripBtnErase.Enabled = true;
                    myForm.eraseLayerToolStripMenuItem.Enabled = true;
                }
                else
                {
                    myForm.toolStripBtnFill.Enabled = false;
                    myForm.fillToolStripMenuItem.Enabled = false;
                    myForm.toolStripBtnErase.Enabled = false;
                    myForm.eraseLayerToolStripMenuItem.Enabled = false;
                }

                //Process the Tool Buttons
                myForm.toolStripBtnPen.Enabled = false;
                myForm.toolStripBtnSelect.Enabled = true;
                myForm.toolStripBtnRect.Enabled = false;
                switch (Globals.CurrentLayer)
                {
                    case Constants.LayerCount: //Attributes
                        myForm.toolStripBtnPen.Enabled = true;
                        myForm.toolStripBtnRect.Enabled = true;
                        break;
                    case Constants.LayerCount + 1: //Lights
                        Globals.CurrentTool = (int)Enums.EdittingTool.Selection;
                        break;
                    case Constants.LayerCount + 2: //Events
                        Globals.CurrentTool = (int)Enums.EdittingTool.Selection;
                        break;
                    case Constants.LayerCount + 3: //NPCS
                        Globals.CurrentTool = (int) Enums.EdittingTool.Selection;
                        break;
                    default:
                        myForm.toolStripBtnPen.Enabled = true;
                        myForm.toolStripBtnRect.Enabled = true;
                        break;
                }
                
                switch (Globals.CurrentTool)
                {
                    case (int)Enums.EdittingTool.Pen:
                        if (!myForm.toolStripBtnPen.Checked) { myForm.toolStripBtnPen.Checked = true; }
                        if (myForm.toolStripBtnSelect.Checked) { myForm.toolStripBtnSelect.Checked = false; }
                        if (myForm.toolStripBtnRect.Checked) { myForm.toolStripBtnRect.Checked = false; }
                        break;
                    case (int)Enums.EdittingTool.Selection:
                        if (myForm.toolStripBtnPen.Checked) { myForm.toolStripBtnPen.Checked = false; }
                        if (!myForm.toolStripBtnSelect.Checked) { myForm.toolStripBtnSelect.Checked = true; }
                        if (myForm.toolStripBtnRect.Checked) { myForm.toolStripBtnRect.Checked = false; }
                        break;
                    case (int)Enums.EdittingTool.Rectangle:
                        if (myForm.toolStripBtnPen.Checked) { myForm.toolStripBtnPen.Checked = false; }
                        if (myForm.toolStripBtnSelect.Checked) { myForm.toolStripBtnSelect.Checked = false; }
                        if (!myForm.toolStripBtnRect.Checked) { myForm.toolStripBtnRect.Checked = true; }
                        break;
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
