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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Library.Localization;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Forms
{
    public partial class frmWarpSelection : Form
    {
        private bool _result = false;
        private int _currentMap = -1;
        private int _currentX;
        private int _currentY;
        private List<int> _restrictMaps = null;
        private bool _tileSelection = true;


        public frmWarpSelection()
        {
            InitializeComponent();
            InitLocalization();
            mapTreeList1.UpdateMapList(_currentMap);
            pnlMap.Width = Options.TileWidth*Options.MapWidth;
            pnlMap.Height = Options.TileHeight*Options.MapHeight;
            pnlMap.BackColor = Color.Black;
            mapTreeList1.SetSelect(new TreeViewEventHandler(NodeDoubleClick));
        }

        public void InitForm(bool tileSelection = true, List<int> restrictMaps = null)
        {
            mapTreeList1.UpdateMapList(_currentMap,restrictMaps);
            _restrictMaps = restrictMaps;
            if (!tileSelection)
            {
                _tileSelection = false;
                this.Text = Strings.Get("warpselection", "mapselectiontitle");
            }
        }

        private void InitLocalization()
        {
            this.Text = Strings.Get("warpselection", "title");
            chkChronological.Text = Strings.Get("warpselection", "chronological");
            btnOk.Text = Strings.Get("warpselection", "okay");
            btnCancel.Text = Strings.Get("warpselection", "cancel");
            grpMapList.Text = Strings.Get("warpselection", "maplist");
            grpMapPreview.Text = Strings.Get("warpselection", "mappreview");
        }

        private void NodeDoubleClick(object sender, TreeViewEventArgs e)
        {
                if (e.Node.Tag.GetType() == typeof(MapListMap))
                {
                    SelectTile(((MapListMap) e.Node.Tag).MapNum, _currentX, _currentY);
                }
        }

        public void SelectTile(int mapNum, int x, int y)
        {
            if (_currentMap != mapNum || x != _currentX || y != _currentY)
            {
                _currentMap = mapNum;
                _currentX = x;
                _currentY = y;
                mapTreeList1.UpdateMapList(mapNum, _restrictMaps);
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            if (_currentMap > -1)
            {
                if (MapInstance.GetMap(_currentMap) != null)
                {
                    if (Database.LoadMapCacheLegacy(_currentMap, MapInstance.GetMap(_currentMap).Revision) != null)
                    {
                        Bitmap newBitmap = new Bitmap(pnlMap.Width,pnlMap.Height);
                        Image img = Database.LoadMapCacheLegacy(_currentMap, MapInstance.GetMap(_currentMap).Revision);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap);
                        g.DrawImage(img, new Rectangle(0, 0, pnlMap.Width, pnlMap.Height),
                            new Rectangle(0, 0, pnlMap.Width, pnlMap.Height), GraphicsUnit.Pixel);
                        if (_tileSelection)
                        {
                            g.DrawRectangle(new Pen(Color.White, 2f),
                                new Rectangle(_currentX*Options.TileWidth, _currentY*Options.TileHeight,
                                    Options.TileWidth,
                                    Options.TileHeight));
                        }
                        g.Dispose();
                        pnlMap.BackgroundImage = newBitmap;
                        img.Dispose();
                        tmrMapCheck.Enabled = false;
                    }
                    else
                    {
                        tmrMapCheck.Enabled = true;
                    }
                }
                else
                {
                    Globals.MapsToFetch = new List<int>() {_currentMap};
                    PacketSender.SendNeedMap(_currentMap);
                    pnlMap.BackgroundImage = null;
                    //Use a timer to check when we have the map.
                    tmrMapCheck.Enabled = true;
                }
            }
        }

        private void chkChronological_CheckedChanged(object sender, EventArgs e)
        {
            mapTreeList1.Chronological = chkChronological.Checked;
            mapTreeList1.UpdateMapList(_currentMap, _restrictMaps);
        }

        private void frmWarpSelection_Load(object sender, EventArgs e)
        {
            mapTreeList1.BeginInvoke(mapTreeList1.MapListDelegate, new object[] { _currentMap, _restrictMaps });
            UpdatePreview();
        }

        private void tmrMapCheck_Tick(object sender, EventArgs e)
        {
            if (_currentMap > -1)
            {
                if (MapInstance.GetMap(_currentMap) != null)
                {
                    if (Database.LoadMapCacheLegacy(_currentMap,MapInstance.GetMap(_currentMap).Revision) != null)
                    {
                        UpdatePreview();
                        tmrMapCheck.Enabled = false;
                    }
                    else
                    {
                        MapInstance oldMap = Globals.CurrentMap;
                        Globals.CurrentMap = MapInstance.GetMap(_currentMap);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            var screenshotTexture = EditorGraphics.ScreenShotMap();
                            screenshotTexture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                            Database.SaveMapCache(_currentMap, MapInstance.GetMap(_currentMap).Revision, ms.ToArray());
                        }
                        Globals.CurrentMap = oldMap;
                    }
                }
            }
            else
            {
                tmrMapCheck.Enabled = false;
            }
        }

        private void pnlMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= pnlMap.Width || e.Y >= pnlMap.Height) { return; }
            if (e.X < 0 || e.Y < 0) { return; }
            _currentX = (int)Math.Floor((double)(e.X) / Options.TileWidth);
            _currentY = (int)Math.Floor((double)(e.Y) / Options.TileHeight);
            UpdatePreview();
        }

        private void pnlMap_DoubleClick(object sender, EventArgs e)
        {
            btnOk_Click(null, null);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_currentMap != -1) _result = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool GetResult()
        {
            return _result;
        }

        public int GetMap()
        {
            return _currentMap;
        }

        public int GetX()
        {
            return _currentX;
        }

        public int GetY()
        {
            return _currentY;
        }
    }
}
