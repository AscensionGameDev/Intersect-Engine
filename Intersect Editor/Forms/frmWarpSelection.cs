using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Intersect;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Color = System.Drawing.Color;

namespace Intersect.Editor.Forms
{
    public partial class frmWarpSelection : Form
    {
        private int _currentMap = -1;
        private int _currentX;
        private int _currentY;
        private int _drawnMap = -1;
        private Image _mapImage = null;
        private List<int> _restrictMaps = null;
        private bool _result = false;
        private bool _tileSelection = true;

        public frmWarpSelection()
        {
            InitializeComponent();
            InitLocalization();
            mapTreeList1.UpdateMapList(_currentMap);
            pnlMap.Width = Options.TileWidth * Options.MapWidth;
            pnlMap.Height = Options.TileHeight * Options.MapHeight;
            pnlMap.BackColor = System.Drawing.Color.Black;
            mapTreeList1.SetSelect(NodeDoubleClick);

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, pnlMap,
                new object[] {true});
        }

        public void InitForm(bool tileSelection = true, List<int> restrictMaps = null)
        {
            mapTreeList1.UpdateMapList(_currentMap, restrictMaps);
            _restrictMaps = restrictMaps;
            if (!tileSelection)
            {
                _tileSelection = false;
                Text = Strings.Get("warpselection", "mapselectiontitle");
            }
        }

        private void InitLocalization()
        {
            Text = Strings.Get("warpselection", "title");
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
            btnRefreshPreview.Enabled = _currentMap > -1;
        }

        private void UpdatePreview()
        {
            if (_currentMap > -1)
            {
                if (_currentMap != _drawnMap)
                {
                    var img = Database.LoadMapCacheLegacy(_currentMap, -1);
                    if (img != null)
                    {
                        _mapImage = img;
                    }
                    else
                    {
                        if (MapInstance.Lookup.Get<MapInstance>(_currentMap) != null) MapInstance.Lookup.Get<MapInstance>(_currentMap).Delete();
                        Globals.MapsToFetch = new List<int>() {_currentMap};
                        if (!Globals.MapsToScreenshot.Contains(_currentMap)) Globals.MapsToScreenshot.Add(_currentMap);
                        PacketSender.SendNeedMap(_currentMap);
                        pnlMap.BackgroundImage = null;
                        //Use a timer to check when we have the map.
                        tmrMapCheck.Enabled = true;
                        return;
                    }
                }
                Bitmap newBitmap = new Bitmap(pnlMap.Width, pnlMap.Height);
                Graphics g = Graphics.FromImage(newBitmap);
                g.DrawImage(_mapImage, new Rectangle(0, 0, pnlMap.Width, pnlMap.Height),
                    new Rectangle(0, 0, pnlMap.Width, pnlMap.Height), GraphicsUnit.Pixel);
                if (_tileSelection)
                {
                    g.DrawRectangle(new Pen(System.Drawing.Color.White, 2f),
                        new Rectangle(_currentX * Options.TileWidth, _currentY * Options.TileHeight,
                            Options.TileWidth,
                            Options.TileHeight));
                }
                g.Dispose();
                pnlMap.BackgroundImage = newBitmap;
                tmrMapCheck.Enabled = false;
                _drawnMap = _currentMap;
                return;
            }
            else
            {
                pnlMap.BackgroundImage = null;
            }
        }

        private void chkChronological_CheckedChanged(object sender, EventArgs e)
        {
            mapTreeList1.Chronological = chkChronological.Checked;
            mapTreeList1.UpdateMapList(_currentMap, _restrictMaps);
        }

        private void frmWarpSelection_Load(object sender, EventArgs e)
        {
            mapTreeList1.BeginInvoke(mapTreeList1.MapListDelegate, _currentMap, _restrictMaps);
            UpdatePreview();
        }

        private void tmrMapCheck_Tick(object sender, EventArgs e)
        {
            if (_currentMap > -1)
            {
                if (Database.LoadMapCacheLegacy(_currentMap, -1) != null)
                {
                    UpdatePreview();
                    tmrMapCheck.Enabled = false;
                }
            }
            else
            {
                tmrMapCheck.Enabled = false;
            }
        }

        private void pnlMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X >= pnlMap.Width || e.Y >= pnlMap.Height)
            {
                return;
            }
            if (e.X < 0 || e.Y < 0)
            {
                return;
            }
            _currentX = (int) Math.Floor((double) (e.X) / Options.TileWidth);
            _currentY = (int) Math.Floor((double) (e.Y) / Options.TileHeight);
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

        private void btnRefreshPreview_Click(object sender, EventArgs e)
        {
            if (_currentMap > -1)
            {
                _drawnMap = -1;
                Database.ClearMapCache(_currentMap);
                UpdatePreview();
            }
        }
    }
}