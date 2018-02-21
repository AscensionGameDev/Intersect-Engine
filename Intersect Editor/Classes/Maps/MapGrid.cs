using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DarkUI.Forms;
using Hjg.Pngcs;
using Intersect.Editor.Classes.General;
using Intersect.Editor.Forms;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Classes.Maps
{
    public class MapGrid
    {
        private MapGridItem mContextMap;
        private ContextMenuStrip mContextMenu;

        private int mCurrentCellX = -1;
        private int mCurrentCellY = -1;

        private ToolStripMenuItem mDropDownLinkItem;
        private ToolStripMenuItem mDropDownUnlinkItem;
        private ToolStripMenuItem mRecacheMapItem;
        public Rectangle ContentRect;
        private bool mCreateTextures;
        private List<Texture2D> mFreeTextures = new List<Texture2D>();
        public MapGridItem[,] Grid;
        private List<int> mGridMaps = null;
        public int GridHeight = 50;
        public int GridWidth = 50;

        private List<int> mLinkMaps = new List<int>();
        public bool Loaded;

        private float mMaxZoom = 1f;
        private float mMinZoom;
        public bool ShowLines = true;
        private bool mSizeChanged = true;

        private object mTexLock = new object();
        private List<Texture2D> mTextures = new List<Texture2D>();
        public int TileHeight;
        public int TileWidth;
        private List<MapGridItem> mToLoad = new List<MapGridItem>();
        public Rectangle ViewRect;
        private Thread mWorkerThread;

        public float Zoom = 1;

        public MapGrid(ToolStripMenuItem dropDownItem, ToolStripMenuItem dropDownUnlink, ToolStripMenuItem recacheItem,
            ContextMenuStrip contextMenu)
        {
            mWorkerThread = new Thread(AsyncLoadingThread);
            mWorkerThread.Start();
            mDropDownLinkItem = dropDownItem;
            mDropDownLinkItem.Click += LinkMapItem_Click;
            mContextMenu = contextMenu;
            mDropDownUnlinkItem = dropDownUnlink;
            mDropDownUnlinkItem.Click += UnlinkMapItem_Click;
            mRecacheMapItem = recacheItem;
            mRecacheMapItem.Click += _recacheMapItem_Click;
        }

        private void AsyncLoadingThread()
        {
            while (!Globals.ClosingEditor)
            {
                lock (mTexLock)
                {
                    if (mToLoad.Count > 0 && mFreeTextures.Count > 0)
                    {
                        var itm = mToLoad[0];
                        mToLoad.RemoveAt(0);
                        var tex = mFreeTextures[0];
                        mFreeTextures.RemoveAt(0);

                        var texData = Database.LoadMapCache(itm.Mapnum, itm.Revision, TileWidth, TileHeight);
                        try
                        {
                            if (texData != null)
                            {
                                tex.SetData(texData);
                                itm.Tex = tex;
                            }
                            else
                            {
                                mFreeTextures.Add(tex);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Ignore errors that are likely caused by timing issues.
                            mFreeTextures.Add(tex);
                            mToLoad.Add(itm);
                        }
                    }
                }
            }
        }

        public Object GetMapGridLock()
        {
            return mTexLock;
        }

        public void Load(ByteBuffer bf)
        {
            Loaded = false;
            List<int> gridMaps = new List<int>();
            GridWidth = (int) bf.ReadLong();
            GridHeight = (int) bf.ReadLong();
            bf.ReadBoolean();
            lock (mTexLock)
            {
                UnloadTextures();
                Grid = new MapGridItem[GridWidth, GridHeight];
            }
            for (int x = -1; x <= GridWidth; x++)
            {
                for (int y = -1; y <= GridHeight; y++)
                {
                    if (y == -1 || y == GridHeight || x == -1 || x == GridWidth)
                    {
                    }
                    else
                    {
                        int num = bf.ReadInteger();
                        if (num == -1)
                        {
                            Grid[x, y] = new MapGridItem(num);
                        }
                        else
                        {
                            Grid[x, y] = new MapGridItem(num, bf.ReadString(), bf.ReadInteger());
                            gridMaps.Add(Grid[x, y].Mapnum);
                        }
                    }
                }
            }
            //Get a list of maps -- if they are not in this grid.
            mLinkMaps.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                if (!gridMaps.Contains(MapList.GetOrderedMaps()[i].MapNum))
                {
                    mLinkMaps.Add(MapList.GetOrderedMaps()[i].MapNum);
                }
            }
            mMaxZoom = 1f; //Real Size
            Zoom = mMinZoom;
            TileWidth = (int) (Options.TileWidth * Options.MapWidth * Zoom);
            TileHeight = (int) (Options.TileHeight * Options.MapHeight * Zoom);
            ContentRect = new Rectangle(ViewRect.Width / 2 - (TileWidth * (GridWidth + 2)) / 2,
                ViewRect.Height / 2 - (TileHeight * (GridHeight + 2)) / 2, TileWidth * (GridWidth + 2),
                TileHeight * (GridHeight + 2));
            mCreateTextures = true;
            Loaded = true;
            mGridMaps = gridMaps;
        }

        public void DoubleClick(int x, int y)
        {
            for (int x1 = 1; x1 < GridWidth + 1; x1++)
            {
                for (int y1 = 1; y1 < GridHeight + 1; y1++)
                {
                    if (new Rectangle(ContentRect.X + x1 * TileWidth, ContentRect.Y + y1 * TileHeight, TileWidth, TileHeight).Contains(new System.Drawing.Point(x, y)))
                    {
                        if (Grid[x1 - 1, y1 - 1].Mapnum > -1)
                        {
                            if (Globals.CurrentMap != null && Globals.CurrentMap.Changed() &&
                                DarkMessageBox.ShowInformation(Strings.Get("mapping", "savemapdialogue"),
                                    Strings.Get("mapping", "savemap"), DarkDialogButton.YesNo,
                                    Properties.Resources.Icon) ==
                                DialogResult.Yes)
                            {
                                SaveMap();
                            }
                            Globals.MainForm.EnterMap(Grid[x1 - 1, y1 - 1].Mapnum);
                            Globals.MapEditorWindow.Select();
                        }
                    }
                }
            }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int) EditingTool.Selection)
            {
                if (Globals.Dragging == true)
                {
                    //Place the change, we done!
                    Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                    Globals.MapEditorWindow.PlaceSelection();
                }
            }
            PacketSender.SendMap(Globals.CurrentMap);
        }

        public void ScreenshotWorld()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif",
                Title = Strings.Get("mapgrid", "savescreenshotdialogue")
            };
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "")
            {
                if (
                    DarkMessageBox.ShowWarning(Strings.Get("mapgrid", "savescreenshotconfirm"),
                        Strings.Get("mapgrid", "savescreenshottitle"), DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    FetchMissingPreviews(false);
                    Globals.PreviewProgressForm = new FrmProgress();
                    Globals.PreviewProgressForm.SetTitle(Strings.Get("mapgrid", "savingscreenshot"));
                    Thread screenShotThread = new Thread(() => ScreenshotWorld(fileDialog.FileName));
                    screenShotThread.Start();
                    Globals.PreviewProgressForm.ShowDialog();
                }
            }
        }

        void ScreenshotWorld(string filename)
        {
            int rowSize = (Options.TileHeight * Options.MapHeight);
            int colSize = (Options.MapWidth * Options.TileWidth);
            int cols = colSize * (GridWidth);
            int rows = rowSize * (GridHeight);
            Bitmap tmpBitmap = new Bitmap(colSize, rowSize);
            Graphics g = Graphics.FromImage(tmpBitmap);
            var png = new PngWriter(new FileStream(filename, FileMode.OpenOrCreate),
                new ImageInfo(cols, rows, 16, true));
            var pngReaderDict = new Dictionary<int, Bitmap>();
            var cacheRow = 0;
            //Generate one row at a time.
            for (int y = 0; y < rows; y++)
            {
                int gridRow = (int) Math.Floor(y / (double) rowSize);
                if (gridRow != cacheRow)
                {
                    foreach (var cache in pngReaderDict)
                    {
                        cache.Value.Dispose();
                    }
                    pngReaderDict.Clear();
                    cacheRow = gridRow;
                }
                byte[] row = new byte[png.ImgInfo.Cols * 4];
                for (int x = 0; x < GridWidth; x++)
                {
                    int gridCol = x;

                    MapGridItem item = Grid[gridCol, gridRow];
                    if (item.Mapnum >= 0)
                    {
                        Bitmap reader = null;
                        if (pngReaderDict.ContainsKey(item.Mapnum))
                        {
                            reader = pngReaderDict[item.Mapnum];
                        }
                        else
                        {
                            var data = Database.LoadMapCacheRaw(item.Mapnum, item.Revision);
                            if (data != null)
                            {
                                reader = new Bitmap(new MemoryStream(data));
                                pngReaderDict.Add(item.Mapnum, reader);
                            }
                        }

                        if (reader != null)
                        {
                            var rowNum = y - (gridRow * rowSize);
                            //Get the pixel color we need
                            for (int x1 = (x) * colSize; x1 < (x) * colSize + colSize; x1++)
                            {
                                System.Drawing.Color clr = reader.GetPixel(x1 - (x) * colSize, rowNum);
                                // Color.FromArgb(ImageLineHelper.GetPixelToARGB8(line, x1 - (x) * colSize));
                                row[x1 * 4] = clr.R;
                                row[x1 * 4 + 1] = clr.G;
                                row[x1 * 4 + 2] = clr.B;
                                row[x1 * 4 + 3] = clr.A;
                            }
                        }
                        else
                        {
                            for (int x1 = (x) * colSize; x1 < (x) * colSize + colSize; x1++)
                            {
                                row[x1 * 4] = 0;
                                row[x1 * 4 + 1] = 255;
                                row[x1 * 4 + 2] = 0;
                                row[x1 * 4 + 3] = 255;
                            }
                        }
                    }
                    else
                    {
                        for (int x1 = (x) * colSize; x1 < (x) * colSize + colSize; x1++)
                        {
                            row[x1 * 4] = System.Drawing.Color.Gray.R;
                            row[x1 * 4 + 1] = System.Drawing.Color.Gray.G;
                            row[x1 * 4 + 2] = System.Drawing.Color.Gray.B;
                            row[x1 * 4 + 3] = System.Drawing.Color.Gray.A;
                        }
                    }
                }
                png.WriteRowByte(row, y);
                Globals.PreviewProgressForm.SetProgress(Strings.Get("mapgrid", "savingrow", y, rows),
                    (int) ((y / (float) rows) * 100), false);
                Application.DoEvents();
            }
            png.End();
            Globals.PreviewProgressForm.NotifyClose();
        }

        public bool Contains(int mapNum)
        {
            if (Grid != null && Loaded)
            {
                return mGridMaps.Contains(mapNum);
            }
            return false;
        }

        public void ResetForm()
        {
            lock (mTexLock)
            {
                UnloadTextures();
                mCreateTextures = true;
            }
        }

        public void FetchMissingPreviews(bool clearAllFirst)
        {
            List<int> maps = new List<int>();
            if (clearAllFirst)
            {
                if (
                    DarkMessageBox.ShowWarning(Strings.Get("mapgrid", "clearandfetch"),
                        Strings.Get("mapgrid", "fetchcaption"), DarkDialogButton.YesNo, Properties.Resources.Icon) !=
                    DialogResult.Yes) return;
                if (
                    DarkMessageBox.ShowInformation(Strings.Get("mapgrid", "keepmapcache"),
                        Strings.Get("mapgrid", "mapcachecaption"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    Database.GridHideOverlay = EditorGraphics.HideOverlay;
                    Database.GridHideDarkness = EditorGraphics.HideDarkness;
                    Database.GridHideFog = EditorGraphics.HideFog;
                    Database.GridHideResources = EditorGraphics.HideResources;
                    if (EditorGraphics.LightColor != null)
                    {
                        Database.GridLightColor =
                            System.Drawing.Color.FromArgb(EditorGraphics.LightColor.A, EditorGraphics.LightColor.R,
                                EditorGraphics.LightColor.G, EditorGraphics.LightColor.B).ToArgb();
                    }
                    else
                    {
                        Database.GridLightColor = System.Drawing.Color.FromArgb(255, 255, 255, 255).ToArgb();
                    }
                }
                else
                {
                    Database.GridHideOverlay = true;
                    Database.GridHideDarkness = true;
                    Database.GridHideFog = true;
                    Database.GridHideResources = false;
                    Database.GridLightColor = System.Drawing.Color.White.ToArgb();
                }
                Database.SaveGridOptions();
                Database.ClearAllMapCache();
            }
            //Get a list of maps without images.
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (Grid[x, y].Mapnum > -1 && Database.LoadMapCacheLegacy(Grid[x, y].Mapnum,
                            Grid[x, y].Revision) == null)
                    {
                        maps.Add(Grid[x, y].Mapnum);
                    }
                }
            }
            if (maps.Count > 0)
            {
                if (clearAllFirst ||
                    DarkMessageBox.ShowWarning(Strings.Get("mapgrid", "justfetch"),
                        Strings.Get("mapgrid", "fetchcaption"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    Globals.FetchingMapPreviews = true;
                    Globals.PreviewProgressForm = new FrmProgress();
                    Globals.PreviewProgressForm.SetTitle(Strings.Get("mapgrid", "fetchingmaps"));
                    Globals.PreviewProgressForm.SetProgress(Strings.Get("mapgrid", "fetchingprogress", 0, maps.Count),
                        0,
                        false);
                    Globals.FetchCount = maps.Count;
                    Globals.MapsToFetch = maps;
                    for (int i = 0; i < maps.Count; i++)
                    {
                        PacketSender.SendNeedMap(maps[i]);
                    }
                    Globals.PreviewProgressForm.ShowDialog();
                }
            }
        }

        public void RightClickGrid(int x, int y, Panel mapGridView)
        {
            for (int x1 = 0; x1 < GridWidth + 2; x1++)
            {
                for (int y1 = 0; y1 < GridHeight + 2; y1++)
                {
                    if (new Rectangle(ContentRect.X + x1 * TileWidth, ContentRect.Y + y1 * TileHeight, TileWidth,
                            TileHeight)
                        .Contains(new System.Drawing.Point(x, y)))
                    {
                        mCurrentCellX = x1;
                        mCurrentCellY = y1;
                        if (mCurrentCellX >= 0 && mCurrentCellY >= 0)
                        {
                            if (mCurrentCellX >= 0 && mCurrentCellY >= 0 && mCurrentCellX - 1 <= GridWidth &&
                                mCurrentCellY - 1 <= GridHeight)
                            {
                                if (mCurrentCellX == 0 || mCurrentCellY == 0 || mCurrentCellX - 1 == GridWidth ||
                                    mCurrentCellY - 1 == GridHeight ||
                                    Grid[mCurrentCellX - 1, mCurrentCellY - 1].Mapnum <= -1)
                                {
                                    int adjacentMap = -1;
                                    //Check Left
                                    if (mCurrentCellX > 1 && mCurrentCellY != 0 && mCurrentCellY - 1 < GridHeight)
                                    {
                                        if (Grid[mCurrentCellX - 2, mCurrentCellY - 1].Mapnum > -1)
                                            adjacentMap = Grid[mCurrentCellX - 2, mCurrentCellY - 1].Mapnum;
                                    }
                                    //Check Right
                                    if (mCurrentCellX < GridWidth && mCurrentCellY != 0 &&
                                        mCurrentCellY - 1 < GridHeight)
                                    {
                                        if (Grid[mCurrentCellX, mCurrentCellY - 1].Mapnum > -1)
                                            adjacentMap = Grid[mCurrentCellX, mCurrentCellY - 1].Mapnum;
                                    }
                                    //Check Up
                                    if (mCurrentCellX != 0 && mCurrentCellY > 1 && mCurrentCellX - 1 < GridWidth)
                                    {
                                        if (Grid[mCurrentCellX - 1, mCurrentCellY - 2].Mapnum > -1)
                                            adjacentMap = Grid[mCurrentCellX - 1, mCurrentCellY - 2].Mapnum;
                                    }
                                    //Check Down
                                    if (mCurrentCellX != 0 && mCurrentCellY < GridHeight &&
                                        mCurrentCellX - 1 < GridWidth)
                                    {
                                        if (Grid[mCurrentCellX - 1, mCurrentCellY].Mapnum > -1)
                                            adjacentMap = Grid[mCurrentCellX - 1, mCurrentCellY].Mapnum;
                                    }
                                    if (adjacentMap > -1)
                                    {
                                        mContextMenu.Show(mapGridView, new System.Drawing.Point(x, y));
                                        mDropDownUnlinkItem.Visible = false;
                                        mDropDownLinkItem.Visible = true;
                                        mRecacheMapItem.Visible = false;
                                    }
                                }
                                else
                                {
                                    mContextMap = Grid[mCurrentCellX - 1, mCurrentCellY - 1];
                                    mContextMenu.Show(mapGridView, new System.Drawing.Point(x, y));
                                    mDropDownUnlinkItem.Visible = true;
                                    mRecacheMapItem.Visible = true;
                                    mDropDownLinkItem.Visible = false;
                                }
                            }
                        }
                        return;
                    }
                }
            }
            mCurrentCellX = -1;
            mCurrentCellY = -1;
        }

        private void UnlinkMapItem_Click(object sender, EventArgs e)
        {
            if (mContextMap != null && mContextMap.Mapnum > -1)
            {
                if (
                    DarkMessageBox.ShowWarning(Strings.Get("mapgrid", "unlinkprompt", mContextMap.Name),
                        Strings.Get("mapgrid", "unlinkcaption"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                    PacketSender.SendUnlinkMap(mContextMap.Mapnum);
            }
        }

        private void _recacheMapItem_Click(object sender, EventArgs e)
        {
            if (mContextMap != null && mContextMap.Mapnum > -1)
            {
                //Fetch and screenshot this singular map
                Database.SaveMapCache(mContextMap.Mapnum, mContextMap.Revision, null);
                if (MapInstance.Lookup.Get<MapInstance>(mContextMap.Mapnum) != null)
                    MapInstance.Lookup.Get<MapInstance>(mContextMap.Mapnum).Delete();
                Globals.MapsToFetch = new List<int>() {mContextMap.Mapnum};
                PacketSender.SendNeedMap(mContextMap.Mapnum);
            }
        }

        private void LinkMapItem_Click(object sender, EventArgs e)
        {
            FrmWarpSelection frmWarpSelection = new FrmWarpSelection();
            frmWarpSelection.InitForm(false, mLinkMaps);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                //Make sure the selected tile is adjacent to a map
                int linkMap = frmWarpSelection.GetMap();
                int adjacentMap = -1;
                //Check Left
                if (mCurrentCellX > 1 && mCurrentCellY != 0 && mCurrentCellY - 1 < GridHeight)
                {
                    if (Grid[mCurrentCellX - 2, mCurrentCellY - 1].Mapnum > -1)
                        adjacentMap = Grid[mCurrentCellX - 2, mCurrentCellY - 1].Mapnum;
                }
                //Check Right
                if (mCurrentCellX < GridWidth && mCurrentCellY != 0 && mCurrentCellY - 1 < GridHeight)
                {
                    if (Grid[mCurrentCellX, mCurrentCellY - 1].Mapnum > -1)
                        adjacentMap = Grid[mCurrentCellX, mCurrentCellY - 1].Mapnum;
                }
                //Check Up
                if (mCurrentCellX != 0 && mCurrentCellY > 1 && mCurrentCellX - 1 < GridWidth)
                {
                    if (Grid[mCurrentCellX - 1, mCurrentCellY - 2].Mapnum > -1)
                        adjacentMap = Grid[mCurrentCellX - 1, mCurrentCellY - 2].Mapnum;
                }
                //Check Down
                if (mCurrentCellX != 0 && mCurrentCellY < GridHeight && mCurrentCellX - 1 < GridWidth)
                {
                    if (Grid[mCurrentCellX - 1, mCurrentCellY].Mapnum > -1)
                        adjacentMap = Grid[mCurrentCellX - 1, mCurrentCellY].Mapnum;
                }
                if (adjacentMap != -1)
                {
                    PacketSender.SendLinkMap(adjacentMap, linkMap, mCurrentCellX - 1, mCurrentCellY - 1);
                }
            }
        }

        public void Update(Microsoft.Xna.Framework.Rectangle panelBounds)
        {
            mMinZoom =
                Math.Min(panelBounds.Width / (float) (Options.TileWidth * Options.MapWidth * (GridWidth + 2)),
                    panelBounds.Height / (float) (Options.TileHeight * Options.MapHeight * (GridHeight + 2))) / 2f;
            //Gotta calculate 
            if (Zoom < mMinZoom)
            {
                Zoom = mMinZoom * 2;
                TileWidth = (int) (Options.TileWidth * Options.MapWidth * Zoom);
                TileHeight = (int) (Options.TileHeight * Options.MapHeight * Zoom);
                ContentRect = new Rectangle(0, 0, TileWidth * (GridWidth + 2), TileHeight * (GridHeight + 2));
                lock (mTexLock)
                {
                    UnloadTextures();
                }
                mCreateTextures = true;
            }
            ViewRect = new Rectangle(panelBounds.Left, panelBounds.Top, panelBounds.Width, panelBounds.Height);
            if (ContentRect.X + TileWidth > ViewRect.Width) ContentRect.X = ViewRect.Width - TileWidth;
            if (ContentRect.X + ContentRect.Width < TileWidth) ContentRect.X = -ContentRect.Width + TileWidth;
            if (ContentRect.Y + TileHeight > ViewRect.Height) ContentRect.Y = ViewRect.Height - TileHeight;
            if (ContentRect.Y + ContentRect.Height < TileHeight) ContentRect.Y = -ContentRect.Height + TileHeight;
            if (mCreateTextures)
            {
                CreateTextures(panelBounds);
                mCreateTextures = false;
            }
            lock (GetMapGridLock())
            {
                if (Grid != null)
                {
                    lock (mTexLock)
                    {
                        for (int x = 0; x < GridWidth; x++)
                        {
                            for (int y = 0; y < GridHeight; y++)
                            {
                                //Figure out if this texture should be loaded
                                if (new Rectangle(ContentRect.X + (x + 1) * TileWidth,
                                        ContentRect.Y + (y + 1) * TileHeight,
                                        TileWidth,
                                        TileHeight).IntersectsWith(ViewRect) && Grid[x, y] != null)
                                {
                                    //if not loaded, add it to the queue
                                    if ((Grid[x, y].Tex == null || Grid[x, y].Tex.IsDisposed) &&
                                        Grid[x, y].Mapnum != -1 &&
                                        !mToLoad.Contains(Grid[x, y]))
                                    {
                                        mToLoad.Add(Grid[x, y]);
                                    }
                                }
                                else
                                {
                                    //If loaded, kick it to the curb
                                    if (mToLoad.Contains(Grid[x, y])) mToLoad.Remove(Grid[x, y]);
                                    if (Grid[x, y] != null && Grid[x, y].Tex != null)
                                    {
                                        mFreeTextures.Add(Grid[x, y].Tex);
                                        Grid[x, y].Tex = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public MapGridItem GetItemAt(int mouseX, int mouseY)
        {
            if (Loaded)
            {
                lock (mTexLock)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        for (int y = 0; y < GridHeight; y++)
                        {
                            //Figure out if this texture should be loaded
                            if (new Rectangle(ContentRect.X + (x + 1) * TileWidth, ContentRect.Y + (y + 1) * TileHeight,
                                    TileWidth,
                                    TileHeight).Contains(new System.Drawing.Point(mouseX, mouseY)) &&
                                Grid[x, y] != null)
                            {
                                return Grid[x, y];
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void CreateTextures(Microsoft.Xna.Framework.Rectangle panelBounds)
        {
            lock (mTexLock)
            {
                int hCount = (int) Math.Ceiling((float) panelBounds.Width / TileWidth) + 2;
                int wCount = (int) Math.Ceiling((float) panelBounds.Height / TileHeight) + 2;
                for (int i = 0; i < hCount * wCount && i < GridWidth * GridHeight; i++)
                {
                    mTextures.Add(new Texture2D(EditorGraphics.GetGraphicsDevice(), TileWidth, TileHeight));
                }
                mFreeTextures.AddRange(mTextures.ToArray());
            }
        }

        private void UnloadTextures()
        {
            lock (mTexLock)
            {
                for (int i = 0; i < mTextures.Count; i++)
                {
                    mTextures[i].Dispose();
                }
                mTextures.Clear();
                mFreeTextures.Clear();
                if (Grid != null && Loaded)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        for (int y = 0; y < GridHeight; y++)
                        {
                            Grid[x, y].Tex = null;
                        }
                    }
                }
            }
        }

        public void Move(int x, int y)
        {
            ContentRect.X -= x;
            ContentRect.Y -= y;
            if (ContentRect.X + TileWidth > ViewRect.Width) ContentRect.X = ViewRect.Width - TileWidth;
            if (ContentRect.X + ContentRect.Width < TileWidth) ContentRect.X = -ContentRect.Width + TileWidth;
            if (ContentRect.Y + TileHeight > ViewRect.Height) ContentRect.Y = ViewRect.Height - TileHeight;
            if (ContentRect.Y + ContentRect.Height < TileHeight) ContentRect.Y = -ContentRect.Height + TileHeight;
        }

        public void ZoomIn(int val, int mouseX, int mouseY)
        {
            int amt = val / 120;

            //Find the original tile we are hovering over
            var x1 = (double) Math.Min(ContentRect.Width, Math.Max(0, mouseX - ContentRect.X)) / (float) TileWidth;
            var y1 = (double) Math.Min(ContentRect.Height, Math.Max(0, mouseY - ContentRect.Y)) / (float) TileHeight;
            var prevZoom = Zoom;
            Zoom += .05f * amt;
            if (prevZoom != Zoom)
            {
                lock (mTexLock)
                {
                    UnloadTextures();
                }
                mCreateTextures = true;
            }
            if (Zoom < mMinZoom) Zoom = mMinZoom;
            if (Zoom > mMaxZoom) Zoom = mMaxZoom;
            TileWidth = (int) (Options.TileWidth * Options.MapWidth * Zoom);
            TileHeight = (int) (Options.TileHeight * Options.MapHeight * Zoom);
            //were gonna get the X/Y of where the content rect would need so that the grid location that the mouse is hovering over would be center of the viewing rect
            //Get the current location of the mouse over the current content rectangle

            var x2 = (int) (x1 * TileWidth);
            var y2 = (int) (y1 * TileHeight);

            ContentRect = new Rectangle(-x2 + mouseX, -y2 + mouseY, TileWidth * (GridWidth + 2),
                TileHeight * (GridHeight + 2));

            //Lets go the extra mile to make sure our selected map is 
        }
    }

    public class MapGridItem
    {
        public MapGridItem(int num, string name = "", int revision = 0)
        {
            Mapnum = num;
            this.Name = name;
            this.Revision = revision;
        }

        public string Name { get; set; }
        public int Revision { get; set; }
        public int Mapnum { get; set; }
        public Texture2D Tex { get; set; }
    }
}