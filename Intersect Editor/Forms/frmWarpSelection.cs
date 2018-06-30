using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Editor.Classes.Localization;

namespace Intersect.Editor.Forms
{
    public partial class FrmWarpSelection : Form
    {
        private int mCurrentMap = -1;
        private int mCurrentX;
        private int mCurrentY;
        private int mDrawnMap = -1;
        private Image mMapImage;
        private List<int> mRestrictMaps;
        private bool mResult;
        private bool mTileSelection = true;

        public FrmWarpSelection()
        {
            InitializeComponent();
            InitLocalization();
            mapTreeList1.UpdateMapList(mCurrentMap);
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
            mapTreeList1.UpdateMapList(mCurrentMap, restrictMaps);
            mRestrictMaps = restrictMaps;
            if (!tileSelection)
            {
                mTileSelection = false;
                Text = Strings.WarpSelection.mapselectiontitle;
            }
        }

        private void InitLocalization()
        {
            Text = Strings.WarpSelection.title;
            chkChronological.Text = Strings.WarpSelection.chronological;
            btnOk.Text = Strings.WarpSelection.okay;
            btnCancel.Text = Strings.WarpSelection.cancel;
            grpMapList.Text = Strings.WarpSelection.maplist;
            grpMapPreview.Text = Strings.WarpSelection.mappreview;
        }

        private void NodeDoubleClick(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(MapListMap))
            {
                SelectTile(((MapListMap) e.Node.Tag).MapNum, mCurrentX, mCurrentY);
            }
        }

        public void SelectTile(int mapNum, int x, int y)
        {
            if (mCurrentMap != mapNum || x != mCurrentX || y != mCurrentY)
            {
                mCurrentMap = mapNum;
                mCurrentX = x;
                mCurrentY = y;
                mapTreeList1.UpdateMapList(mapNum, mRestrictMaps);
                UpdatePreview();
            }
            btnRefreshPreview.Enabled = mCurrentMap > -1;
        }

        private void UpdatePreview()
        {
            if (mCurrentMap > -1)
            {
                if (mCurrentMap != mDrawnMap)
                {
                    var img = Database.LoadMapCacheLegacy(mCurrentMap, -1);
                    if (img != null)
                    {
                        mMapImage = img;
                    }
                    else
                    {
                        if (MapInstance.Lookup.Get<MapInstance>(mCurrentMap) != null)
                            MapInstance.Lookup.Get<MapInstance>(mCurrentMap).Delete();
                        Globals.MapsToFetch = new List<int>() {mCurrentMap};
                        if (!Globals.MapsToScreenshot.Contains(mCurrentMap)) Globals.MapsToScreenshot.Add(mCurrentMap);
                        PacketSender.SendNeedMap(mCurrentMap);
                        pnlMap.BackgroundImage = null;
                        //Use a timer to check when we have the map.
                        tmrMapCheck.Enabled = true;
                        return;
                    }
                }
                Bitmap newBitmap = new Bitmap(pnlMap.Width, pnlMap.Height);
                Graphics g = Graphics.FromImage(newBitmap);
                g.DrawImage(mMapImage, new Rectangle(0, 0, pnlMap.Width, pnlMap.Height),
                    new Rectangle(0, 0, pnlMap.Width, pnlMap.Height), GraphicsUnit.Pixel);
                if (mTileSelection)
                {
                    g.DrawRectangle(new Pen(System.Drawing.Color.White, 2f),
                        new Rectangle(mCurrentX * Options.TileWidth, mCurrentY * Options.TileHeight,
                            Options.TileWidth,
                            Options.TileHeight));
                }
                g.Dispose();
                pnlMap.BackgroundImage = newBitmap;
                tmrMapCheck.Enabled = false;
                mDrawnMap = mCurrentMap;
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
            mapTreeList1.UpdateMapList(mCurrentMap, mRestrictMaps);
        }

        private void frmWarpSelection_Load(object sender, EventArgs e)
        {
            mapTreeList1.BeginInvoke(mapTreeList1.MapListDelegate, mCurrentMap, mRestrictMaps);
            UpdatePreview();
        }

        private void tmrMapCheck_Tick(object sender, EventArgs e)
        {
            if (mCurrentMap > -1)
            {
                var img = Database.LoadMapCacheLegacy(mCurrentMap, -1);
                if (img != null)
                {
                    UpdatePreview();
                    tmrMapCheck.Enabled = false;
                    img.Dispose();
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
            mCurrentX = (int) Math.Floor((double) (e.X) / Options.TileWidth);
            mCurrentY = (int) Math.Floor((double) (e.Y) / Options.TileHeight);
            UpdatePreview();
        }

        private void pnlMap_DoubleClick(object sender, EventArgs e)
        {
            btnOk_Click(null, null);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (mCurrentMap != -1) mResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool GetResult()
        {
            return mResult;
        }

        public int GetMap()
        {
            return mCurrentMap;
        }

        public int GetX()
        {
            return mCurrentX;
        }

        public int GetY()
        {
            return mCurrentY;
        }

        private void btnRefreshPreview_Click(object sender, EventArgs e)
        {
            if (mCurrentMap > -1)
            {
                mDrawnMap = -1;
                Database.ClearMapCache(mCurrentMap);
                UpdatePreview();
            }
        }
    }
}