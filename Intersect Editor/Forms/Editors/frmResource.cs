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
using DarkUI.Controls;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Forms.Editors;
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
        private byte[] _copiedItem = null;

        private Bitmap _initialTileset;
        private Bitmap _endTileset;
        private Bitmap _initialBitmap;
        private Bitmap _endBitmap;

        public frmResource()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstResources.LostFocus += itemList_FocusChanged;
            lstResources.GotFocus += itemList_FocusChanged;
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
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add("None");
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbItem.Items.Clear();
            cmbItem.Items.Add("None");
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstResources.Items.Clear();
            lstResources.Items.AddRange(Database.GetGameObjectList(GameObject.Resource));
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add("None");
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbToolType.SelectedIndex = _editorItem.Tool + 1;
                nudSpawnDuration.Value = _editorItem.SpawnDuration;
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.Animation) + 1;
                txtHP.Text = _editorItem.MinHP.ToString();
                txtMaxHp.Text = _editorItem.MaxHP.ToString();
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
            UpdateToolStripItems();
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value - 1;
            lblDropIndex.Text = "Drop: " + (index + 1);
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObject.Item, _editorItem.Drops[index].ItemNum) + 1;
            txtDropAmount.Text = _editorItem.Drops[index].Amount.ToString();
            nudDropChance.Value = _editorItem.Drops[index].Chance;
        }

        private void scrlDropIndex_Scroll(object sender, ScrollValueEventArgs e)
        {
            UpdateDropValues();
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            _editorItem.Drops[scrlDropIndex.Value - 1].Amount = x;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnDuration = (int)nudSpawnDuration.Value;
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[scrlDropIndex.Value - 1].Chance = (int)nudDropChance.Value;
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Tool = cmbToolType.SelectedIndex -1;
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

        private void frmResource_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void tmrRender_Tick(object sender, EventArgs e)
        {
            Render();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Resource);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstResources.Focused)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstResources.Focused)
            {
                _copiedItem = _editorItem.GetData();
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstResources.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (MessageBox.Show("Are you sure you want to undo changes made to this game object? This action cannot be reverted!",
                        "Undo Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _editorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void itemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    toolStripItemUndo_Click(null, null);
                }
                else if (e.KeyCode == Keys.V)
                {
                    toolStripItemPaste_Click(null, null);
                }
                else if (e.KeyCode == Keys.C)
                {
                    toolStripItemCopy_Click(null, null);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Delete)
                {
                    toolStripItemDelete_Click(null, null);
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = _editorItem != null && lstResources.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstResources.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstResources.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstResources.Focused;
        }

        private void itemList_FocusChanged(object sender, EventArgs e)
        {
            UpdateToolStripItems();
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.N)
                {
                    toolStripItemNew_Click(null, null);
                }
            }
        }

        private void btnRequirements_Click(object sender, EventArgs e)
        {
            var frm = new frmDynamicRequirements(_editorItem.HarvestingReqs, RequirementType.Resource);
            frm.ShowDialog();
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[scrlDropIndex.Value - 1].ItemNum = Database.GameObjectIdFromList(GameObject.Item,
                cmbItem.SelectedIndex -1);
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Animation = Database.GameObjectIdFromList(GameObject.Animation, cmbAnimation.SelectedIndex -1);
        }
    }
}
