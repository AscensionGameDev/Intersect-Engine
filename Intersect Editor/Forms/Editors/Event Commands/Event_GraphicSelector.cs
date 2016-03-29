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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.General;
using EditorGraphics = Intersect_Editor.Classes.EditorGraphics;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    public partial class Event_GraphicSelector : UserControl
    {

        private EventGraphic _editingGraphic;
        private FrmEvent _eventEditor;
        private EventGraphic _tmpGraphic = new EventGraphic();
        private Event_MoveRouteDesigner _routeDesigner = null;
        private bool _newRouteAction = false;

        private int _spriteWidth = 0;
        private int _spriteHeight = 0;

        private bool _mouseDown;
        private bool _loading = false;

        public Event_GraphicSelector(EventGraphic editingGraphic, FrmEvent eventEditor, Event_MoveRouteDesigner moveRouteDesigner = null, bool newMoveRouteAction = false)
        {
            InitializeComponent();
            _editingGraphic = editingGraphic;
            _tmpGraphic.CopyFrom(_editingGraphic);
            _eventEditor = eventEditor;
            _loading = true;
            cmbGraphicType.SelectedIndex = _editingGraphic.Type;
            UpdateGraphicList();
            if (cmbGraphic.Items.IndexOf(_editingGraphic.Filename) > -1)
            {
                cmbGraphic.SelectedIndex = cmbGraphic.Items.IndexOf(_editingGraphic.Filename);
            }
            UpdatePreview();
            _routeDesigner = moveRouteDesigner;
            _newRouteAction = newMoveRouteAction;
            _loading = false;
        }

        private void GraphicTypeUpdated()
        {
            _tmpGraphic.Filename = "";
            _tmpGraphic.Type = 0;
            _tmpGraphic.X = 0;
            _tmpGraphic.Y = 0;
            _tmpGraphic.Width = -1;
            _tmpGraphic.Height = -1;
            if (cmbGraphicType.SelectedIndex == 0) //No Graphic
            {
                cmbGraphic.Hide();
                lblGraphic.Hide();
                UpdatePreview();
            }
            else if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                _tmpGraphic.Type = 1;
                cmbGraphic.Show();
                lblGraphic.Show();
                cmbGraphic.Items.Clear();
                foreach (var filename in EditorGraphics.EntityFileNames)
                {
                    cmbGraphic.Items.Add(filename);
                }
                if (cmbGraphic.Items.Count > 0) cmbGraphic.SelectedIndex = 0;
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                _tmpGraphic.Type = 2;
                _tmpGraphic.Width = 0;
                _tmpGraphic.Height = 0;
                lblGraphic.Show();
                cmbGraphic.Show();
                cmbGraphic.Items.Clear();
                foreach (var filename in Globals.Tilesets)
                {
                    if (File.Exists("Resources/Tilesets/" + filename))
                    {
                        cmbGraphic.Items.Add(filename);
                    }
                }
                if (cmbGraphic.Items.Count > 0) cmbGraphic.SelectedIndex = 0;
            }
        }

        private void UpdateGraphicList()
        {
            cmbGraphic.Items.Clear();
            if (cmbGraphicType.SelectedIndex == 0) //No Graphic
            {
                cmbGraphic.Hide();
                lblGraphic.Hide();
            }
            else if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                cmbGraphic.Show();
                lblGraphic.Show();
                foreach (var filename in EditorGraphics.EntityFileNames)
                {
                    cmbGraphic.Items.Add(filename);
                }
                if (cmbGraphic.Items.Count > 0) cmbGraphic.SelectedIndex = 0;
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                lblGraphic.Show();
                cmbGraphic.Show();
                foreach (var filename in Globals.Tilesets)
                {
                    if (File.Exists("Resources/Tilesets/" + filename))
                    {
                        cmbGraphic.Items.Add(filename);
                    }
                }
                if (cmbGraphic.Items.Count > 0) cmbGraphic.SelectedIndex = 0;
            }
        }

        private void UpdatePreview()
        {
            System.Drawing.Graphics graphics;
            Bitmap sourceBitmap = null;
            Bitmap destBitmap = null;
            if (cmbGraphicType.SelectedIndex == 1) //Sprite
            {
                sourceBitmap = new Bitmap("Resources/Entities/" + cmbGraphic.Text);
                _spriteWidth = sourceBitmap.Width/4;
                _spriteHeight = sourceBitmap.Height/4;
            }
            else if (cmbGraphicType.SelectedIndex == 2) //Tileset
            {
                sourceBitmap = new Bitmap("Resources/Tilesets/" + cmbGraphic.Text);
            }
            if (sourceBitmap != null)
            {
                pnlGraphic.Show();
                destBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
                pnlGraphic.Width = sourceBitmap.Width;
                pnlGraphic.Height = sourceBitmap.Height;
                graphics = System.Drawing.Graphics.FromImage(destBitmap);
                graphics.Clear(Color.Black);
                graphics.DrawImage(sourceBitmap, new System.Drawing.Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height));
                if (cmbGraphicType.SelectedIndex == 1)
                {
                    graphics.DrawRectangle(new Pen(Color.White, 2f),
                        new Rectangle(_tmpGraphic.X*sourceBitmap.Width/4, _tmpGraphic.Y*sourceBitmap.Height/4,
                            sourceBitmap.Width/4, sourceBitmap.Height/4));
                }
                else if (cmbGraphicType.SelectedIndex == 2)
                {
                    var selX = _tmpGraphic.X;
                    var selY = _tmpGraphic.Y;
                    var selW = _tmpGraphic.Width;
                    var selH = _tmpGraphic.Height;
                    if (selW < 0)
                    {
                        selX -= Math.Abs(selW);
                        selW = Math.Abs(selW);
                    }
                    if (selH < 0)
                    {
                        selY -= Math.Abs(selH);
                        selH = Math.Abs(selH);
                    }
                    graphics.DrawRectangle(new Pen(Color.White,2f), 
                        new Rectangle(selX * Options.TileWidth, selY * Options.TileHeight,
                           Options.TileWidth + (selW * Options.TileWidth), Options.TileHeight + (selH * Options.TileHeight)));
                }
                sourceBitmap.Dispose();
                graphics.Dispose();
                pnlGraphic.BackgroundImage = destBitmap;
            }
            else
            {
                pnlGraphic.Hide();
            }

        }

        private void cmbGraphicType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading) GraphicTypeUpdated();
        }

        private void cmbGraphic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tmpGraphic.Filename = cmbGraphic.Text;
            UpdatePreview();
        }

        private void pnlGraphic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height) { return; }
            if (cmbGraphicType.SelectedIndex == 1)
            {
                _tmpGraphic.X = (int)Math.Floor((double)e.X / _spriteWidth);
                _tmpGraphic.Y = (int)Math.Floor((double)e.Y / _spriteHeight);
            }
            else
            {
                _mouseDown = true;
                _tmpGraphic.X = (int)Math.Floor((double)e.X / Options.TileWidth);
                _tmpGraphic.Y = (int)Math.Floor((double)e.Y / Options.TileHeight);
            }
            _tmpGraphic.Width = 0;
            _tmpGraphic.Height = 0;
            if (_tmpGraphic.X < 0) { _tmpGraphic.X = 0; }
            if (_tmpGraphic.Y < 0) { _tmpGraphic.Y = 0; }
            UpdatePreview();
        }

        private void pnlGraphic_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height) { return; }
            if (cmbGraphicType.SelectedIndex != 2) return;
            var selX = _tmpGraphic.X;
            var selY = _tmpGraphic.Y;
            var selW = _tmpGraphic.Width;
            var selH = _tmpGraphic.Height;
            if (selW < 0)
            {
                selX -= Math.Abs(selW);
                selW = Math.Abs(selW);
            }
            if (selH < 0)
            {
                selY -= Math.Abs(selH);
                selH = Math.Abs(selH);
            }
            _tmpGraphic.X = selX;
            _tmpGraphic.Y = selY;
            _tmpGraphic.Width = selW;
            _tmpGraphic.Height = selH;
            _mouseDown = false;
            UpdatePreview();
        }

        private void pnlGraphic_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > pnlGraphic.Width || e.Y > pnlGraphic.Height) { return; }
            if (cmbGraphicType.SelectedIndex != 2) return;
            if (_mouseDown)
            {
                var tmpX = (int)Math.Floor((double)e.X / Options.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Options.TileHeight);
                _tmpGraphic.Width = tmpX - _tmpGraphic.X;
                _tmpGraphic.Height = tmpY - _tmpGraphic.Y;
            }
            UpdatePreview();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _editingGraphic.CopyFrom(_tmpGraphic);
            _eventEditor.CloseGraphicSelector(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_routeDesigner != null && _newRouteAction) _routeDesigner.RemoveLastAction();
            _eventEditor.CloseGraphicSelector(this);
        }
    }
}
