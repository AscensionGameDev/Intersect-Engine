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
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using Point = System.Drawing.Point;
using System.Drawing;
using System.IO;

namespace Intersect_Editor.Forms
{
    public partial class frmGridView : Form
    {
        private int gridWidth = 5;
        private int gridHeight = 5;
        private MapGridItem[,] myGrid;
        public frmGridView()
        {
            InitializeComponent();
        }

        public void InitGrid(ByteBuffer bf)
        {
            int size = 0;
            gridWidth = (int)bf.ReadLong();
            gridHeight = (int)bf.ReadLong();
            myGrid = new MapGridItem[gridWidth, gridHeight];
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            for (int x = -1; x <= gridWidth; x++)
            {
                dataGridView1.Columns.Add(@"", @"");
            }
            dataGridView1.Rows.Add(gridHeight + 2);
            for (int x = -1; x <= gridWidth; x++)
            {
                for (int y = -1; y <= gridHeight; y++)
                {
                    if (y == -1 || y == gridHeight || x == -1 || x == gridWidth)
                    {
                        dataGridView1.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Black;
                    }
                    else
                    {
                        int num = bf.ReadInteger();
                        if (num == -1)
                        {
                            myGrid[x, y] = new MapGridItem(num);
                            dataGridView1.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Gray;
                        }
                        else
                        {
                            myGrid[x, y] = new MapGridItem(num,bf.ReadString(),bf.ReadInteger());
                            dataGridView1.Rows[y + 1].Cells[x + 1].Style.BackColor = Color.Green;
                            dataGridView1.Rows[y + 1].Cells[x + 1].ValueType = typeof(string);
                            dataGridView1.Rows[y + 1].Cells[x + 1].Value = myGrid[x,y].mapnum + ". " + myGrid[x,y].name;
                            if (dataGridView1.Rows[y + 1].Cells[x + 1].Size.Width > size) size = dataGridView1.Rows[y + 1].Cells[x + 1].Size.Width;
                        }
                    }
                }
            }
            for (int x = -1; x <= gridWidth; x++)
            {
                dataGridView1.Columns[x+1].Width = size;
            }
            for (int y = -1; y <= gridHeight; y++)
            {
                dataGridView1.Rows[y + 1].Height = (int)((float)size * ((float)(Globals.TileHeight * Globals.MapHeight)/(float)(Globals.TileWidth * Globals.MapWidth)));
            }
        }

        private void frmGridView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > 0 && e.ColumnIndex > 0 && e.ColumnIndex - 1 < gridWidth && e.RowIndex - 1 < gridHeight)
            {
                //e.Handled = true;
                //e.PaintContent(e.CellBounds);
                if ((e.PaintParts & DataGridViewPaintParts.Background) != DataGridViewPaintParts.None)
                {
                    //e.Graphics.DrawImage(Resources.Image1, e.CellBounds);
                }
                if (!e.Handled && myGrid[e.ColumnIndex - 1,e.RowIndex - 1].mapnum > -1)
                {
                    e.Handled = true;
                    if (File.Exists(myGrid[e.ColumnIndex - 1, e.RowIndex - 1].imagepath))
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
                    e.Graphics.DrawRectangle(Pens.LightGray, new Rectangle(e.CellBounds.X - 1,e.CellBounds.Y-1,e.CellBounds.Width,e.CellBounds.Height));
                    e.PaintContent(e.CellBounds);
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
        public MapGridItem(int num, string name = "", int revision = 0)
        {
            this.mapnum = num;
            this.name = name;
            this.revision = revision;
            this.imagepath = "resources/mapcache/" + mapnum + "_" + revision + ".png";
        }
    }
}
