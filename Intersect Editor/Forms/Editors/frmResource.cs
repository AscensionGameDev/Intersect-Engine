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
using System.Drawing;
using System.IO;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;


namespace Intersect_Editor.Classes
{
    public partial class frmResource : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        private List<ResourceBase> _changed = new List<ResourceBase>();
        private ResourceBase _editorItem = null;

        private Bitmap _initialTileset;
        private Bitmap _endTileset;
        private Bitmap _initialBitmap;
        private Bitmap _endBitmap;

        public frmResource()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Resource)
            {
                InitEditor();
                if (_editorItem != null && !ResourceBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Resource);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                _editorItem.RestoreBackup();
                UpdateEditor();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null)
            {
                if (
                    MessageBox.Show(
                        "Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in _changed)
            {
                item.RestoreBackup();
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Send Changed items
            foreach (var item in _changed)
            {
                PacketSender.SendSaveObject(item);
                item.DeleteBackup();
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstResources_Click(object sender, EventArgs e)
        {
            _editorItem = ResourceBase.GetResource(Database.GameObjectIdFromList(GameObject.Resource, lstResources.SelectedIndex));
            UpdateEditor();
        }

        private void frmResource_Load(object sender, EventArgs e)
        {
            _initialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            _endBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            cmbInitialSprite.Items.Clear();
            cmbEndSprite.Items.Clear();
            cmbInitialSprite.Items.Add("None");
            cmbEndSprite.Items.Add("None");
            string[] resources = GameContentManager.GetTextureNames(GameContentManager.TextureType.Resource);
            for (int i = 0; i < resources.Length; i++)
            {
                cmbInitialSprite.Items.Add(resources[i]);
                cmbEndSprite.Items.Add(resources[i]);
            }
            scrlDropItem.Maximum = ItemBase.ObjectCount() - 1;
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstResources.Items.Clear();
            lstResources.Items.AddRange(Database.GetGameObjectList(GameObject.Resource));
            cmbToolType.Items.Add("None");
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            UpdateInitialScrollBars();
            UpdateFinalScrollBars();
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbToolType.SelectedIndex = _editorItem.Tool;
                scrlSpawnDuration.Value = _editorItem.SpawnDuration;
                scrlAnimation.Value = Database.GameObjectListIndex(GameObject.Animation, _editorItem.Animation);
                txtHP.Text = _editorItem.MinHP.ToString();
                txtMaxHp.Text = _editorItem.MaxHP.ToString();
                lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
                if (scrlAnimation.Value == -1)
                {
                    lblAnimation.Text = @"Animation: None";
                }
                else
                {
                    lblAnimation.Text = @"Animation: " + AnimationBase.GetName(_editorItem.Animation);
                }
                chkWalkableBefore.Checked = _editorItem.WalkableBefore;
                chkWalkableAfter.Checked = _editorItem.WalkableAfter;
                cmbInitialSprite.SelectedIndex =
                    cmbInitialSprite.FindString(_editorItem.InitialGraphic);
                cmbEndSprite.SelectedIndex = cmbEndSprite.FindString(_editorItem.EndGraphic);
                scrlDropIndex.Value = 1;
                UpdateDropValues();
                Render();
                if (_changed.IndexOf(_editorItem) == -1)
                {
                    _changed.Add(_editorItem);
                    _editorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value - 1;
            lblDropIndex.Text = "Drop: " + (index + 1);
            scrlDropItem.Value = _editorItem.Drops[index].ItemNum;
            lblDropItem.Text = @"Item " + ItemBase.GetName(Database.GameObjectIdFromList(GameObject.Item,scrlDropItem.Value));
            txtDropAmount.Text = _editorItem.Drops[index].Amount.ToString();
            scrlDropChance.Value = _editorItem.Drops[index].Chance;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlDropIndex_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateDropValues();
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            lblDropItem.Text = @"Item " + ItemBase.GetName(Database.GameObjectIdFromList(GameObject.Item, scrlDropItem.Value));
            _editorItem.Drops[scrlDropIndex.Value - 1].ItemNum = Database.GameObjectIdFromList(GameObject.Item,
                scrlDropItem.Value);
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            _editorItem.Drops[scrlDropIndex.Value - 1].Amount = x;
        }

        private void scrlDropChance_Scroll(object sender, ScrollEventArgs e)
        {
            _editorItem.Drops[scrlDropIndex.Value - 1].Chance = scrlDropChance.Value;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlSpawnDuration_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            _editorItem.SpawnDuration = scrlSpawnDuration.Value;
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Tool = cmbToolType.SelectedIndex;
        }

        private void chkWalkableBefore_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.WalkableBefore = chkWalkableBefore.Checked;
        }

        private void chkWalkableAfter_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.WalkableAfter = chkWalkableAfter.Checked;
        }

        private void cmbInitialSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInitialSprite.SelectedIndex > 0)
            {
                _editorItem.InitialGraphic = cmbInitialSprite.Text;
                if (File.Exists("resources/resources/" + cmbInitialSprite.Text))
                {
                    _initialTileset = (Bitmap)Bitmap.FromFile("resources/resources/" + cmbInitialSprite.Text);
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
                _editorItem.InitialGraphic = "None";
                _initialTileset = null;
            }
            Render();
        }

        private void cmbEndSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndSprite.SelectedIndex > 0)
            {
                _editorItem.EndGraphic = cmbEndSprite.Text;
                if (File.Exists("resources/resources/" + cmbEndSprite.Text))
                {
                    _endTileset = (Bitmap)Bitmap.FromFile("resources/resources/" + cmbEndSprite.Text);
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
                _editorItem.EndGraphic = "None";
                _endTileset = null;
            }
            Render();
        }

        public void Render()
        {
            Pen whitePen = new Pen(System.Drawing.Color.Red, 1);

            // Initial Sprite
            var gfx = System.Drawing.Graphics.FromImage(_initialBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picInitialResource.Width, picInitialResource.Height));
            if (cmbInitialSprite.SelectedIndex > 0 && _initialTileset != null)
            {
                gfx.DrawImage(_initialTileset, new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height), new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height), GraphicsUnit.Pixel);
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
            }
            gfx.Dispose();
            gfx = picEndResource.CreateGraphics();
            gfx.DrawImageUnscaled(_endBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();
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

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstResources.Items[lstResources.SelectedIndex] = txtName.Text;

        }

        private void txtHP_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtHP.Text, out x);
            _editorItem.MinHP = x;
        }

        private void txtMaxHp_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMaxHp.Text, out x);
            _editorItem.MaxHP = x;
        }

        private void scrlAnimation_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlAnimation.Value >= 0)
            {
                _editorItem.Animation = Database.GameObjectIdFromList(GameObject.Animation,scrlAnimation.Value);
                lblAnimation.Text = "Animation: " + AnimationBase.GetName(_editorItem.Animation);
            }
            else
            {
                _editorItem.Animation = -1;
                lblAnimation.Text = "Animation: None";
            }
        }

        private void frmResource_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void tmrRender_Tick(object sender, EventArgs e)
        {
            Render();
        }
    }
}
