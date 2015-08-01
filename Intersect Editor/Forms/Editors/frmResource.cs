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
using System.Windows.Forms;
using System.Drawing;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Image = SFML.Graphics.Image;
using SFML.System;
using Intersect_Editor.Classes;
using System.IO;

namespace Intersect_Editor.Classes
{
    public partial class frmResource : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        private ByteBuffer[] _resourcesBackup;
        private bool[] _changed;
        private int _editorIndex;

        private Bitmap _initialTileset;
        private Bitmap _endTileset;
        private Bitmap _initialBitmap;
        private Bitmap _endBitmap;

        public frmResource()
        {
            InitializeComponent();
        }

        private void frmResource_Load(object sender, EventArgs e)
        {
            _initialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            _endBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            lstResources.SelectedIndex = 0;
            cmbInitialSprite.Items.Clear();
            cmbEndSprite.Items.Clear();
            cmbInitialSprite.Items.Add("None");
            cmbEndSprite.Items.Add("None");
            for (int i = 0; i < Globals.Tilesets.Length; i++)
            {
                cmbInitialSprite.Items.Add(Globals.Tilesets[i]);
                cmbEndSprite.Items.Add(Globals.Tilesets[i]);
            }
            scrlDropItem.Maximum = Constants.MaxItems - 1;
            UpdateEditor();

        }

        public void InitEditor()
        {
            _resourcesBackup = new ByteBuffer[Constants.MaxResources];
            _changed = new bool[Constants.MaxResources];
            for (var i = 0; i < Constants.MaxResources; i++)
            {
                _resourcesBackup[i] = new ByteBuffer();
                _resourcesBackup[i].WriteBytes(Globals.GameResources[i].ResourceData());
                lstResources.Items.Add((i + 1) + ") " + Globals.GameResources[i].Name);
                _changed[i] = false;
            }
            cmbToolType.Items.Add("None");
            cmbToolType.Items.AddRange(Enums.ToolTypes.ToArray());
            UpdateInitialScrollBars();
            UpdateFinalScrollBars();
        }

        private void UpdateEditor()
        {
            _editorIndex = lstResources.SelectedIndex;
            txtName.Text = Globals.GameResources[_editorIndex].Name;
            cmbToolType.SelectedIndex = Globals.GameResources[_editorIndex].Tool;
            scrlSpawnDuration.Value = Globals.GameResources[_editorIndex].SpawnDuration;
            scrlAnimation.Value = Globals.GameResources[_editorIndex].Animation;
            txtHP.Text = Globals.GameResources[_editorIndex].MinHP.ToString();
            txtMaxHp.Text = Globals.GameResources[_editorIndex].MaxHP.ToString();
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            lblAnimation.Text = @"Animation: " + (scrlAnimation.Value + 1) + " " + Globals.GameAnimations[scrlAnimation.Value].Name;
            chkWalkableBefore.Checked = Globals.GameResources[_editorIndex].WalkableBefore;
            chkWalkableAfter.Checked = Globals.GameResources[_editorIndex].WalkableAfter;
            cmbInitialSprite.SelectedIndex = cmbInitialSprite.FindString(Globals.GameResources[_editorIndex].InitialGraphic.Sprite);
            cmbEndSprite.SelectedIndex = cmbEndSprite.FindString(Globals.GameResources[_editorIndex].EndGraphic.Sprite);
            scrlDropIndex.Value = 1;
            UpdateDropValues();
            Render();
            _changed[_editorIndex] = true;
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value - 1;
            lblDropIndex.Text = "Drop: " + (index + 1);
            scrlDropItem.Value = Globals.GameResources[_editorIndex].Drops[index].ItemNum;
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            txtDropAmount.Text = Globals.GameResources[_editorIndex].Drops[index].Amount.ToString();
            scrlDropChance.Value = Globals.GameResources[_editorIndex].Drops[index].Chance;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlDropIndex_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateDropValues();
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            Globals.GameResources[_editorIndex].Drops[scrlDropIndex.Value - 1].ItemNum = scrlDropItem.Value;
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            Globals.GameResources[_editorIndex].Drops[scrlDropIndex.Value - 1].Amount = x;
        }

        private void scrlDropChance_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameResources[_editorIndex].Drops[scrlDropIndex.Value - 1].Chance = scrlDropChance.Value;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlSpawnDuration_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            Globals.GameResources[_editorIndex].SpawnDuration = scrlSpawnDuration.Value;
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameResources[_editorIndex].Tool = cmbToolType.SelectedIndex;
        }

        private void chkWalkableBefore_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameResources[_editorIndex].WalkableBefore = chkWalkableBefore.Checked;
        }

        private void chkWalkableAfter_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameResources[_editorIndex].WalkableAfter = chkWalkableAfter.Checked;
        }

        private void cmbInitialSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInitialSprite.SelectedIndex > 0)
            {
                Globals.GameResources[_editorIndex].InitialGraphic.Sprite = cmbInitialSprite.Text;
                if (File.Exists("Resources/Tilesets/" + cmbInitialSprite.Text))
                {
                    _initialTileset = (Bitmap)Bitmap.FromFile("Resources/Tilesets/" + cmbInitialSprite.Text);
                    picInitialResource.Width = _initialTileset.Width;
                    picInitialResource.Height = _initialTileset.Height;
                    _initialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
                    UpdateInitialScrollBars();
                }
                else
                {
                    _initialTileset = null;
                }
            }
            else
            {
                Globals.GameResources[_editorIndex].InitialGraphic.Sprite = "";
                _initialTileset = null;
            }
            Render();
        }

        private void cmbEndSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndSprite.SelectedIndex > 0)
            {
                Globals.GameResources[_editorIndex].EndGraphic.Sprite = cmbEndSprite.Text;
                if (File.Exists("Resources/Tilesets/" + cmbEndSprite.Text))
                {
                    _endTileset = (Bitmap)Bitmap.FromFile("Resources/Tilesets/" + cmbEndSprite.Text);
                    picEndResource.Width = _endTileset.Width;
                    picEndResource.Height = _endTileset.Height;
                    _endBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
                    UpdateFinalScrollBars();
                }
                else
                {
                    _endTileset = null;
                }
            }
            else
            {
                Globals.GameResources[_editorIndex].EndGraphic.Sprite = "";
                _endTileset = null;
            }
            Render();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxResources; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendResource(i, Globals.GameResources[i].ResourceData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempResource = new ResourceStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempResource.ResourceData());
            Globals.GameResources[_editorIndex].Load(tempBuff.ToArray());
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxResources; i++)
            {
                Globals.GameResources[i].Load(_resourcesBackup[i].ToArray());
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        public void Render()
        {
            Rectangle tileSelection;
            Pen whitePen = new Pen(System.Drawing.Color.Red, 1);

            // Initial Sprite
            var gfx = System.Drawing.Graphics.FromImage(_initialBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picInitialResource.Width, picInitialResource.Height));
            if (cmbInitialSprite.SelectedIndex > 0 && _initialTileset != null)
            {
                gfx.DrawImage(_initialTileset, new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height), new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height), GraphicsUnit.Pixel);

                tileSelection = new Rectangle(Globals.GameResources[_editorIndex].InitialGraphic.X * Constants.TileWidth, Globals.GameResources[_editorIndex].InitialGraphic.Y * Constants.TileHeight, (Globals.GameResources[_editorIndex].InitialGraphic.Width + 1) * Constants.TileWidth, (Globals.GameResources[_editorIndex].InitialGraphic.Height + 1) * Constants.TileHeight);
                gfx.DrawRectangle(whitePen, tileSelection);
            }
            gfx.Dispose();
            gfx = picInitialResource.CreateGraphics();
            gfx.DrawImageUnscaled(_initialBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();

            // End Sprite
            gfx = System.Drawing.Graphics.FromImage(_endBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picEndResource.Width, picEndResource.Height));
            if (cmbEndSprite.SelectedIndex > 0 && _endTileset != null)
            {
                gfx.DrawImage(_endTileset, new Rectangle(0, 0, _endTileset.Width, _endTileset.Height), new Rectangle(0, 0, _endTileset.Width, _endTileset.Height), GraphicsUnit.Pixel);

                tileSelection = new Rectangle(Globals.GameResources[_editorIndex].EndGraphic.X * Constants.TileWidth, Globals.GameResources[_editorIndex].EndGraphic.Y * Constants.TileHeight, (Globals.GameResources[_editorIndex].EndGraphic.Width + 1) * Constants.TileWidth, (Globals.GameResources[_editorIndex].EndGraphic.Height + 1) * Constants.TileHeight);
                gfx.DrawRectangle(whitePen, tileSelection);
            }
            gfx.Dispose();
            gfx = picEndResource.CreateGraphics();
            gfx.DrawImageUnscaled(_endBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();
        }

        private void picInitialResource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picInitialResource.Width || e.Y > picInitialResource.Height) { return; }
            _tMouseDown = true;
            Globals.GameResources[_editorIndex].InitialGraphic.X = (int)Math.Floor((double)e.X / Constants.TileWidth);
            Globals.GameResources[_editorIndex].InitialGraphic.Y = (int)Math.Floor((double)e.Y / Constants.TileWidth);
            Globals.GameResources[_editorIndex].InitialGraphic.Width = 0;
            Globals.GameResources[_editorIndex].InitialGraphic.Height = 0;
            if (Globals.GameResources[_editorIndex].InitialGraphic.X < 0) { Globals.GameResources[_editorIndex].InitialGraphic.X = 0; }
            if (Globals.GameResources[_editorIndex].InitialGraphic.Y < 0) { Globals.GameResources[_editorIndex].InitialGraphic.Y = 0; }
        }

        private void picInitialResource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picInitialResource.Width || e.Y > picInitialResource.Height) { return; }
            if (_tMouseDown)
            {
                var tmpX = (int)Math.Floor((double)e.X / Constants.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Constants.TileHeight);
                Globals.GameResources[_editorIndex].InitialGraphic.Width = tmpX - Globals.GameResources[_editorIndex].InitialGraphic.X;
                Globals.GameResources[_editorIndex].InitialGraphic.Height = tmpY - Globals.GameResources[_editorIndex].InitialGraphic.Y;
            }
        }

        private void picInitialResource_MouseUp(object sender, MouseEventArgs e)
        {
            var selX = Globals.GameResources[_editorIndex].InitialGraphic.X;
            var selY = Globals.GameResources[_editorIndex].InitialGraphic.Y;
            var selW = Globals.GameResources[_editorIndex].InitialGraphic.Width;
            var selH = Globals.GameResources[_editorIndex].InitialGraphic.Height;
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
            Globals.GameResources[_editorIndex].InitialGraphic.X = selX;
            Globals.GameResources[_editorIndex].InitialGraphic.Y = selY;
            Globals.GameResources[_editorIndex].InitialGraphic.Width = selW;
            Globals.GameResources[_editorIndex].InitialGraphic.Height = selH;
            _tMouseDown = false;
        }

        private void picEndResource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picEndResource.Width || e.Y > picEndResource.Height) { return; }
            _tMouseDown = true;
            Globals.GameResources[_editorIndex].EndGraphic.X = (int)Math.Floor((double)e.X / Constants.TileWidth);
            Globals.GameResources[_editorIndex].EndGraphic.Y = (int)Math.Floor((double)e.Y / Constants.TileHeight);
            Globals.GameResources[_editorIndex].EndGraphic.Width = 0;
            Globals.GameResources[_editorIndex].EndGraphic.Height = 0;
            if (Globals.GameResources[_editorIndex].EndGraphic.X < 0) { Globals.GameResources[_editorIndex].EndGraphic.X = 0; }
            if (Globals.GameResources[_editorIndex].EndGraphic.Y < 0) { Globals.GameResources[_editorIndex].EndGraphic.Y = 0; }
        }

        private void picEndResource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picEndResource.Width || e.Y > picEndResource.Height) { return; }
            if (_tMouseDown)
            {
                var tmpX = (int)Math.Floor((double)e.X / Constants.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Constants.TileHeight);
                Globals.GameResources[_editorIndex].EndGraphic.Width = tmpX - Globals.GameResources[_editorIndex].EndGraphic.X;
                Globals.GameResources[_editorIndex].EndGraphic.Height = tmpY - Globals.GameResources[_editorIndex].EndGraphic.Y;
            }
        }

        private void picEndResource_MouseUp(object sender, MouseEventArgs e)
        {
            var selX = Globals.GameResources[_editorIndex].EndGraphic.X;
            var selY = Globals.GameResources[_editorIndex].EndGraphic.Y;
            var selW = Globals.GameResources[_editorIndex].EndGraphic.Width;
            var selH = Globals.GameResources[_editorIndex].EndGraphic.Height;
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
            Globals.GameResources[_editorIndex].EndGraphic.X = selX;
            Globals.GameResources[_editorIndex].EndGraphic.Y = selY;
            Globals.GameResources[_editorIndex].EndGraphic.Width = selW;
            Globals.GameResources[_editorIndex].EndGraphic.Height = selH;
            _tMouseDown = false;
        }

        private void UpdateInitialScrollBars()
        {
            vScrollStartTileset.Minimum = 0;
            vScrollStartTileset.Maximum = 1;
            vScrollStartTileset.Value = 0;
            vScrollStartTileset.Minimum = 0;
            vScrollStartTileset.Maximum = 1;
            vScrollStartTileset.Value = 0;
            picInitialResource.Left = 0;
            picInitialResource.Top = 0;

            if (picInitialResource.Width > grpInitialTileset.Width)
            {
                hScrollStartTileset.Enabled = true;
                hScrollStartTileset.Maximum = picInitialResource.Width - grpInitialTileset.Width;
            }
            else
            {
                hScrollStartTileset.Enabled = false;
            }
            if (picInitialResource.Height > grpInitialTileset.Height)
            {
                vScrollStartTileset.Enabled = true;
                vScrollStartTileset.Maximum = picInitialResource.Height - grpInitialTileset.Height;
            }
            else
            {
                vScrollStartTileset.Enabled = false;
            }
        }

        private void UpdateFinalScrollBars()
        {
            vScrollEndTileset.Minimum = 0;
            vScrollEndTileset.Maximum = 1;
            vScrollEndTileset.Value = 0;
            vScrollEndTileset.Minimum = 0;
            vScrollEndTileset.Maximum = 1;
            vScrollEndTileset.Value = 0;
            picEndResource.Left = 0;
            picEndResource.Top = 0;

            if (picEndResource.Width > grpEndTileset.Width)
            {
                hScrollEndTileset.Enabled = true;
                hScrollEndTileset.Maximum = picEndResource.Width - grpEndTileset.Width;
            }
            else
            {
                hScrollEndTileset.Enabled = false;
            }
            if (picEndResource.Height > grpEndTileset.Height)
            {
                vScrollEndTileset.Enabled = true;
                vScrollEndTileset.Maximum = picEndResource.Height - grpEndTileset.Height;
            }
            else
            {
                vScrollEndTileset.Enabled = false;
            }
        }

        private void hScrollStartTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picInitialResource.Left = -hScrollStartTileset.Value;
        }

        private void hScrollEndTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picEndResource.Left = -hScrollEndTileset.Value;
        }

        private void vScrollStartTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picInitialResource.Top = -vScrollStartTileset.Value;
        }

        private void vScrollEndTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picEndResource.Top = -vScrollEndTileset.Value;
        }

        private void lstResources_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameResources[_editorIndex].Name = txtName.Text;
            lstResources.Items[lstResources.SelectedIndex] = (lstResources.SelectedIndex + 1) + ". " + txtName.Text;

        }

        private void txtHP_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtHP.Text, out x);
            Globals.GameResources[_editorIndex].MinHP = x;
        }

        private void txtMaxHp_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMaxHp.Text, out x);
            Globals.GameResources[_editorIndex].MaxHP = x;
        }

        private void scrlAnimation_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlAnimation.Value >= 0)
            {
                lblAnimation.Text = "Animation: " + (scrlAnimation.Value + 1) + " " + Globals.GameAnimations[scrlAnimation.Value].Name;
            }
            else
            {
                lblAnimation.Text = "Animation: 0 None";
            }
            Globals.GameResources[_editorIndex].Animation = scrlAnimation.Value;
        }

        private void frmResource_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }
    }
}
