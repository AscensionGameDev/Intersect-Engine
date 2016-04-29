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
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Point = System.Drawing.Point;
using System.Drawing;
using System.IO;
using System.Threading;
using Hjg.Pngcs;
using Intersect_Library;
using Intersect_Library.GameObjects.Maps.MapList;
using Color = System.Drawing.Color;


namespace Intersect_Editor.Forms
{
    public partial class frmGridView : Form
    {
        private int gridWidth = 5;
        private int gridHeight = 5;
        private MapGridItem[,] myGrid;
        private bool showNames = true;
        private bool showPreviews = false;
        private int defaultSize;
        private int currentSize;
        private int currentCellX;
        private int currentCellY;
        private int contextMap = -1;
        public frmGridView()
        {
            InitializeComponent();
            cmbZoom.SelectedIndex = 0;
        }

        public void InitGrid(ByteBuffer bf)
        {
            int size = 0;
            List<int> gridMaps = new List<int>();
            gridWidth = (int)bf.ReadLong();
            gridHeight = (int)bf.ReadLong();
            myGrid = new MapGridItem[gridWidth, gridHeight];
            mapGridView.AutoSize = true;
            mapGridView.Columns.Clear();
            mapGridView.Rows.Clear();
            for (int x = -1; x <= gridWidth; x++)
            {
                mapGridView.Columns.Add(@"", @"");
            }
            mapGridView.Rows.Add(gridHeight + 2);
            for (int x = -1; x <= gridWidth; x++)
            {
                for (int y = -1; y <= gridHeight; y++)
                {
                    if (y == -1 || y == gridHeight || x == -1 || x == gridWidth)
                    {
                        mapGridView.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Black;
                    }
                    else
                    {
                        int num = bf.ReadInteger();
                        if (num == -1)
                        {
                            myGrid[x, y] = new MapGridItem(num);
                            mapGridView.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            myGrid[x, y] = new MapGridItem(num, bf.ReadString(), bf.ReadInteger());
                            gridMaps.Add(myGrid[x, y].mapnum);
                            mapGridView.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Green;
                            mapGridView.Rows[y + 1].Cells[x + 1].ValueType = typeof(string);
                            mapGridView.Rows[y + 1].Cells[x + 1].Value = myGrid[x, y].mapnum + ". " + myGrid[x, y].name;
                            if (mapGridView.Rows[y + 1].Cells[x + 1].Size.Width > size) size = mapGridView.Rows[y + 1].Cells[x + 1].Size.Width;
                        }
                    }
                }
            }
            linkMapToolStripMenuItem.DropDownItems.Clear();
            List<ToolStripButton> linkBtns = new List<ToolStripButton>();
            //Get a list of maps -- if they are not in this grid.
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                if (!gridMaps.Contains(MapList.GetOrderedMaps()[i].MapNum))
                {
                    var item = new ToolStripButton(MapList.GetOrderedMaps()[i].MapNum + ". " + MapList.GetOrderedMaps()[i].Name);
                    item.Tag = MapList.GetOrderedMaps()[i].MapNum;
                    item.Click += LinkMapItem_Click;
                    linkBtns.Add(item);
                }
            }
            linkMapToolStripMenuItem.DropDownItems.AddRange(linkBtns.ToArray());
            defaultSize = size;
            currentSize = 0;
            cmbZoom_SelectedIndexChanged(null, null);
            mapGridView.Select();
        }

        private void frmGridView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void mapGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex > 0 && e.ColumnIndex - 1 < gridWidth && e.RowIndex - 1 < gridHeight)
            {
                if (!e.Handled && myGrid[e.ColumnIndex - 1, e.RowIndex - 1].mapnum > -1 && ((e.PaintParts & DataGridViewPaintParts.Background) != DataGridViewPaintParts.None))
                {
                    e.Handled = true;
                    if (File.Exists(myGrid[e.ColumnIndex - 1, e.RowIndex - 1].imagepath) && showPreviews)
                    {
                        Bitmap tmpBitmap = new Bitmap(myGrid[e.ColumnIndex - 1, e.RowIndex - 1].imagepath);
                        e.Graphics.DrawImage(tmpBitmap, e.CellBounds);
                        tmpBitmap.Dispose();
                    }
                    else
                    {
                        if (myGrid[e.ColumnIndex - 1, e.RowIndex - 1].mapnum == Globals.CurrentMap)
                        {
                            e.Graphics.FillRectangle(Brushes.OrangeRed, e.CellBounds);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.Green, e.CellBounds);
                        }
                    }
                }
                e.Graphics.DrawRectangle(Pens.LightGray, new Rectangle(e.CellBounds.X - 1, e.CellBounds.Y - 1, e.CellBounds.Width, e.CellBounds.Height));
                if (showNames) e.PaintContent(e.CellBounds);
            }
        }

        private void btnToggleNames_Click(object sender, EventArgs e)
        {
            showNames = !showNames;
            mapGridView.Refresh();
        }

        private void btnFetchPreview_Click(object sender, EventArgs e)
        {
            //Get a list of maps without images.
            List<int> maps = new List<int>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (myGrid[x, y].mapnum > -1 && !File.Exists(myGrid[x, y].imagepath))
                    {
                        maps.Add(myGrid[x, y].mapnum);
                    }
                }
            }
            if (maps.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to fetch previews for each map? This could take several minutes based on the number of maps in this grid!", "Fetch Preview?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.FetchingMapPreviews = true;
                    Globals.PreviewProgressForm = new frmProgress();
                    Globals.PreviewProgressForm.SetTitle("Fetching Map Previews");
                    Globals.PreviewProgressForm.SetProgress("Fetching Maps: 0/" + maps.Count, 0, false);
                    Globals.FetchCount = maps.Count;
                    Globals.MapsToFetch = maps;
                    for (int i = 0; i < maps.Count; i++)
                    {
                        PacketSender.SendNeedMap(maps[i]);
                    }
                    Globals.PreviewProgressForm.ShowDialog();
                    showPreviews = true;
                    mapGridView.Refresh();
                }
            }
        }

        private void btnTogglePreviews_Click(object sender, EventArgs e)
        {
            showPreviews = !showPreviews;
            mapGridView.Refresh();
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Globals.InEditor) return;
            int size = 0;
            DataGridViewCellStyle style = mapGridView.DefaultCellStyle.Clone();
            mapGridView.AutoSize = true;
            if (cmbZoom.SelectedIndex == 0)
            {
                mapGridView.AutoSize = false;
                mapGridView.Width = this.Width;
                mapGridView.Height = this.Height - toolStrip1.Height;
                size = Math.Min((int)(Math.Floor(mapGridView.Height / (mapGridView.RowCount * ((float)(Options.TileHeight * Options.MapHeight) / (float)(Options.TileWidth * Options.MapWidth))))), mapGridView.Width / mapGridView.ColumnCount);
                style.Font = new Font(style.Font.FontFamily, 8.25f, FontStyle.Bold);
            }
            else if (cmbZoom.SelectedIndex == 1)
            {
                size = (Options.MapWidth * Options.TileWidth) / 8;
                style.Font = new Font(style.Font.FontFamily, 24f, FontStyle.Bold);
            }
            else if (cmbZoom.SelectedIndex == 2)
            {
                size = (Options.MapWidth * Options.TileWidth) / 4;
                style.Font = new Font(style.Font.FontFamily, 24f, FontStyle.Bold);
            }
            else if (cmbZoom.SelectedIndex == 3)
            {
                size = (Options.MapWidth * Options.TileWidth) / 2;
                style.Font = new Font(style.Font.FontFamily, 36f, FontStyle.Bold);
            }
            else if (cmbZoom.SelectedIndex == 4)
            {
                size = (Options.MapWidth * Options.TileWidth) * 3 / 4;
                style.Font = new Font(style.Font.FontFamily, 48f, FontStyle.Bold);
            }
            else
            {
                size = Options.MapWidth * Options.TileWidth;
                style.Font = new Font(style.Font.FontFamily, 60f, FontStyle.Bold);
            }
            if (size != currentSize)
            {
                mapGridView.DefaultCellStyle = style;
                currentSize = size;
                for (int x = -1; x <= gridWidth; x++)
                {
                    mapGridView.Columns[x + 1].Width = size;
                }
                for (int y = -1; y <= gridHeight; y++)
                {
                    mapGridView.Rows[y + 1].Height = (int)((float)size * ((float)(Options.TileHeight * Options.MapHeight) / (float)(Options.TileWidth * Options.MapWidth)));
                }
                mapGridView.Refresh();
            }
        }

        private void mapGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex > 0 && e.ColumnIndex - 1 < gridWidth && e.RowIndex - 1 < gridHeight)
            {
                if (myGrid[e.ColumnIndex - 1, e.RowIndex - 1].mapnum > -1)
                {
                    Globals.MainForm.EnterMap(myGrid[e.ColumnIndex - 1, e.RowIndex - 1].mapnum);
                }
            }
        }

        private void mapGridView_SelectionChanged(object sender, EventArgs e)
        {
            mapGridView.ClearSelection();
        }

        private void mapGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            currentCellX = e.ColumnIndex;
            currentCellY = e.RowIndex;
        }

        private void mapGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (mapGridView.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
                {
                    currentCellX = mapGridView.HitTest(e.X, e.Y).ColumnIndex;
                    currentCellY = mapGridView.HitTest(e.X, e.Y).RowIndex;
                }
                else
                {
                    contextMap = -1;
                }
                if (currentCellX >= 0 && currentCellY >= 0)
                {
                    if (currentCellX >= 0 && currentCellY >= 0 && currentCellX - 1 <= gridWidth &&
                        currentCellY - 1 <= gridHeight)
                    {
                        if (currentCellX == 0 || currentCellY == 0 || currentCellX - 1 == gridWidth ||
                            currentCellY - 1 == gridHeight || myGrid[currentCellX - 1, currentCellY - 1].mapnum <= -1)
                        {
                            int adjacentMap = -1;
                            //Check Left
                            if (currentCellX > 1 && currentCellY != 0 && currentCellY - 1 < gridHeight)
                            {
                                if (myGrid[currentCellX - 2, currentCellY - 1].mapnum > -1)
                                    adjacentMap = myGrid[currentCellX - 2, currentCellY - 1].mapnum;
                            }
                            //Check Right
                            if (currentCellX < gridWidth && currentCellY != 0 && currentCellY - 1 < gridHeight)
                            {
                                if (myGrid[currentCellX, currentCellY - 1].mapnum > -1)
                                    adjacentMap = myGrid[currentCellX, currentCellY - 1].mapnum;
                            }
                            //Check Up
                            if (currentCellX != 0 && currentCellY > 1 && currentCellX - 1 < gridWidth)
                            {
                                if (myGrid[currentCellX - 1, currentCellY - 2].mapnum > -1)
                                    adjacentMap = myGrid[currentCellX - 1, currentCellY - 2].mapnum;
                            }
                            //Check Down
                            if (currentCellX != 0 && currentCellY < gridHeight && currentCellX - 1 < gridWidth)
                            {
                                if (myGrid[currentCellX - 1, currentCellY].mapnum > -1)
                                    adjacentMap = myGrid[currentCellX - 1, currentCellY].mapnum;
                            }
                            if (adjacentMap > -1)
                            {
                                mapMenuStrip.Show(mapGridView, new Point(e.X, e.Y));
                                unlinkMapToolStripMenuItem.Visible = false;
                                linkMapToolStripMenuItem.Visible = true;
                            }
                        }
                        else
                        {
                            contextMap = myGrid[currentCellX - 1, currentCellY - 1].mapnum;
                            mapMenuStrip.Show(mapGridView, new Point(e.X, e.Y));
                            unlinkMapToolStripMenuItem.Visible = true;
                            linkMapToolStripMenuItem.Visible = false;
                        }
                    }
                }
            }
        }

        private void LinkMapItem_Click(object sender, EventArgs e)
        {
            //Make sure the selected tile is adjacent to a map
            int linkMap = (int)((ToolStripItem)sender).Tag;
            int adjacentMap = -1;
            //Check Left
            if (currentCellX > 1 && currentCellY != 0 && currentCellY - 1 < gridHeight)
            {
                if (myGrid[currentCellX - 2, currentCellY - 1].mapnum > -1)
                    adjacentMap = myGrid[currentCellX - 2, currentCellY - 1].mapnum;
            }
            //Check Right
            if (currentCellX < gridWidth && currentCellY != 0 && currentCellY - 1 < gridHeight)
            {
                if (myGrid[currentCellX, currentCellY - 1].mapnum > -1)
                    adjacentMap = myGrid[currentCellX, currentCellY - 1].mapnum;
            }
            //Check Up
            if (currentCellX != 0 && currentCellY > 1 && currentCellX - 1 < gridWidth)
            {
                if (myGrid[currentCellX - 1, currentCellY - 2].mapnum > -1)
                    adjacentMap = myGrid[currentCellX - 1, currentCellY - 2].mapnum;
            }
            //Check Down
            if (currentCellX != 0 && currentCellY < gridHeight && currentCellX - 1 < gridWidth)
            {
                if (myGrid[currentCellX - 1, currentCellY].mapnum > -1)
                    adjacentMap = myGrid[currentCellX - 1, currentCellY].mapnum;
            }
            if (adjacentMap != -1)
            {
                PacketSender.SendLinkMap(adjacentMap, linkMap, currentCellX - 1, currentCellY - 1);
            }
        }


        private void unlinkMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMap > -1)
            {
                if (MessageBox.Show("Are you sure you want to unlink map " + contextMap + "?", "Unlink Map", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    PacketSender.SendUnlinkMap(contextMap);
            }
        }

        void ScreenshotWorld(string filename)
        {
            int rowSize = (Options.TileHeight * Options.MapHeight);
            int colSize = (Options.MapWidth * Options.TileWidth);
            int cols = colSize * (mapGridView.ColumnCount - 2);
            int rows = rowSize * (mapGridView.RowCount - 2);
            Bitmap tmpBitmap = new Bitmap(colSize, rowSize);
            string loadedBitmap = "";
            Graphics g = Graphics.FromImage(tmpBitmap);
            var png = new PngWriter(new FileStream(filename, FileMode.OpenOrCreate), new ImageInfo(cols, rows, 16, true));

            //Generate one row at a time.
            for (int y = 0; y < rows; y++)
            {
                int gridRow = (int)Math.Floor(y / (double)rowSize);
                byte[] row = new byte[png.ImgInfo.Cols * 4];
                for (int x = 1; x < mapGridView.ColumnCount - 1; x++)
                {
                    int gridCol = x;

                    MapGridItem item = myGrid[gridCol - 1, gridRow];
                    if (item.mapnum >= 0)
                    {
                        if (item.imagepath != "" && File.Exists(item.imagepath))
                        {
                            if (item.pngReader == null)
                                item.pngReader = FileHelper.CreatePngReader(item.imagepath);
                            ImageLine line = item.pngReader.ReadRow(y - (gridRow * rowSize));

                            //Get the pixel color we need
                            for (int x1 = (x - 1) * colSize; x1 < (x - 1) * colSize + colSize; x1++)
                            {
                                Color clr = Color.FromArgb(ImageLineHelper.GetPixelToARGB8(line, x1 - (x - 1) * colSize));
                                row[x1 * 4] = clr.R;
                                row[x1 * 4 + 1] = clr.G;
                                row[x1 * 4 + 2] = clr.B;
                                row[x1 * 4 + 3] = clr.A;
                            }
                        }
                        else
                        {
                            for (int x1 = (x - 1) * colSize; x1 < (x - 1) * colSize + colSize; x1++)
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
                        for (int x1 = (x - 1) * colSize; x1 < (x - 1) * colSize + colSize; x1++)
                        {
                            row[x1 * 4] = Color.Gray.R;
                            row[x1 * 4 + 1] = Color.Gray.G;
                            row[x1 * 4 + 2] = Color.Gray.B;
                            row[x1 * 4 + 3] = Color.Gray.A;
                        }
                    }
                }
                png.WriteRowByte(row, y);
                Globals.PreviewProgressForm.SetProgress("Saving Row: " + y + "/" + rows, (int)((y / (float)rows) * 100), false);
                Application.DoEvents();
            }
            png.End();
            Globals.PreviewProgressForm.NotifyClose();
        }

        private void frmGridView_ResizeEnd(object sender, EventArgs e)
        {
            if (cmbZoom.SelectedIndex == 0) cmbZoom_SelectedIndexChanged(null, null);
        }

        private void btnScreenshotWorld_Click(object sender, EventArgs e)
        {
            //Get a list of maps without images.
            List<int> maps = new List<int>();
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (myGrid[x, y].mapnum > -1 && !File.Exists(myGrid[x, y].imagepath))
                    {
                        maps.Add(myGrid[x, y].mapnum);
                    }
                }
            }
            if (maps.Count > 0)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to fetch previews for each map? This is required before a screenshot can be taken!",
                        "Fetch Preview?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.FetchingMapPreviews = true;
                    Globals.PreviewProgressForm = new frmProgress();
                    Globals.PreviewProgressForm.SetTitle("Fetching Map Previews");
                    Globals.PreviewProgressForm.SetProgress("Fetching Maps: 0/" + maps.Count, 0, false);
                    Globals.FetchCount = maps.Count;
                    Globals.MapsToFetch = maps;
                    for (int i = 0; i < maps.Count; i++)
                    {
                        PacketSender.SendNeedMap(maps[i]);
                    }
                    Globals.PreviewProgressForm.ShowDialog();
                }
                else
                {
                    return;
                }
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            fileDialog.Title = "Save a screenshot of the world";
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "")
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to save a screenshot of your world to a file? This could take several minutes!",
                        "Save Screenshot?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.PreviewProgressForm = new frmProgress();
                    Globals.PreviewProgressForm.SetTitle("Saving Screenshot");
                    Thread screenShotThread = new Thread(() => ScreenshotWorld(fileDialog.FileName));
                    screenShotThread.Start();
                    Globals.PreviewProgressForm.ShowDialog();
                }
            }
        }
    }

    public class MapGridItem
    {
        public string name { get; set; }
        public int revision { get; set; }
        public int mapnum { get; set; }
        public string imagepath { get; set; }
        public PngReader pngReader { get; set; }
        public MapGridItem(int num, string name = "", int revision = 0)
        {
            this.mapnum = num;
            this.name = name;
            this.revision = revision;
            this.imagepath = "resources/mapcache/" + mapnum + "_" + revision + ".png";
        }
    }
}
