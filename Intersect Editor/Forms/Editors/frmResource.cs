using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Classes
{
    public partial class frmResource : EditorForm
    {
        private List<ResourceBase> _changed = new List<ResourceBase>();
        private byte[] _copiedItem = null;
        private ResourceBase _editorItem = null;
        private Bitmap _endBitmap;
        private Bitmap _endTileset;
        private Bitmap _initialBitmap;

        private Bitmap _initialTileset;
        //General Editting Variables
        bool _tMouseDown;

        public frmResource()
        {
            ApplyHooks();
            InitializeComponent();
            lstResources.LostFocus += itemList_FocusChanged;
            lstResources.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Resource)
            {
                InitEditor();
                if (_editorItem != null && !ResourceBase.Lookup.Values.Contains(_editorItem))
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
            _editorItem =
                ResourceBase.Lookup.Get<ResourceBase>(Database.GameObjectIdFromList(GameObjectType.Resource, lstResources.SelectedIndex));
            UpdateEditor();
        }

        private void frmResource_Load(object sender, EventArgs e)
        {
            _initialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            _endBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            cmbInitialSprite.Items.Clear();
            cmbEndSprite.Items.Clear();
            cmbInitialSprite.Items.Add(Strings.Get("general", "none"));
            cmbEndSprite.Items.Add(Strings.Get("general", "none"));
            string[] resources = GameContentManager.GetTextureNames(GameContentManager.TextureType.Resource);
            for (int i = 0; i < resources.Length; i++)
            {
                cmbInitialSprite.Items.Add(resources[i]);
                cmbEndSprite.Items.Add(resources[i]);
            }
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbItem.Items.Clear();
            cmbItem.Items.Add(Strings.Get("general", "none"));
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("resourceeditor", "title");
            toolStripItemNew.Text = Strings.Get("resourceeditor", "new");
            toolStripItemDelete.Text = Strings.Get("resourceeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("resourceeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("resourceeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("resourceeditor", "undo");

            grpResources.Text = Strings.Get("resourceeditor", "resources");

            grpGeneral.Text = Strings.Get("resourceeditor", "general");
            lblName.Text = Strings.Get("resourceeditor", "name");
            lblToolType.Text = Strings.Get("resourceeditor", "tooltype");
            lblHP.Text = Strings.Get("resourceeditor", "minhp");
            lblMaxHp.Text = Strings.Get("resourceeditor", "maxhp");
            lblSpawnDuration.Text = Strings.Get("resourceeditor", "spawnduration");
            lblAnimation.Text = Strings.Get("resourceeditor", "animation");
            chkWalkableBefore.Text = Strings.Get("resourceeditor", "walkablebefore");
            chkWalkableAfter.Text = Strings.Get("resourceeditor", "walkableafter");
            btnRequirements.Text = Strings.Get("resourceeditor", "requirements");

            grpDrops.Text = Strings.Get("resourceeditor", "drops");
            lblDropIndex.Text = Strings.Get("resourceeditor", "dropindex");
            lblDropItem.Text = Strings.Get("resourceeditor", "dropitem");
            lblDropAmount.Text = Strings.Get("resourceeditor", "dropamount");
            lblDropChance.Text = Strings.Get("resourceeditor", "dropchance");

            grpGraphics.Text = Strings.Get("resourceeditor", "graphics");
            lblPic.Text = Strings.Get("resourceeditor", "initialgraphic");
            lblPic2.Text = Strings.Get("resourceeditor", "exhaustedgraphic");

            btnSave.Text = Strings.Get("resourceeditor", "save");
            btnCancel.Text = Strings.Get("resourceeditor", "cancel");
        }

        public void InitEditor()
        {
            lstResources.Items.Clear();
            lstResources.Items.AddRange(Database.GetGameObjectList(GameObjectType.Resource));
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.Get("general", "none"));
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
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.Animation) +
                                             1;
                nudMinHp.Value = _editorItem.MinHP;
                nudMaxHp.Value = _editorItem.MaxHP;
                chkWalkableBefore.Checked = _editorItem.WalkableBefore;
                chkWalkableAfter.Checked = _editorItem.WalkableAfter;
                cmbInitialSprite.SelectedIndex =
                    cmbInitialSprite.FindString(_editorItem.InitialGraphic);
                cmbEndSprite.SelectedIndex = cmbEndSprite.FindString(_editorItem.EndGraphic);
                nudDropIndex.Value = 1;
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
            int index = (int)nudDropIndex.Value - 1;
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _editorItem.Drops[index].ItemNum) + 1;
            nudDropAmount.Value = _editorItem.Drops[index].Amount;
            nudDropChance.Value = _editorItem.Drops[index].Chance;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnDuration = (int) nudSpawnDuration.Value;
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[(int)nudDropIndex.Value - 1].Chance = (int) nudDropChance.Value;
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Tool = cmbToolType.SelectedIndex - 1;
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
                    _initialTileset = (Bitmap) Image.FromFile("resources/resources/" + cmbInitialSprite.Text);
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
                _editorItem.InitialGraphic = Strings.Get("general", "none");
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
                    _endTileset = (Bitmap) Image.FromFile("resources/resources/" + cmbEndSprite.Text);
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
                _editorItem.EndGraphic = Strings.Get("general", "none");
                _endTileset = null;
            }
            Render();
        }

        public void Render()
        {
            Pen whitePen = new Pen(System.Drawing.Color.Red, 1);

            // Initial Sprite
            var gfx = Graphics.FromImage(_initialBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picInitialResource.Width, picInitialResource.Height));
            if (cmbInitialSprite.SelectedIndex > 0 && _initialTileset != null)
            {
                gfx.DrawImage(_initialTileset, new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height),
                    new Rectangle(0, 0, _initialTileset.Width, _initialTileset.Height), GraphicsUnit.Pixel);
            }
            gfx.Dispose();
            gfx = picInitialResource.CreateGraphics();
            gfx.DrawImageUnscaled(_initialBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();

            // End Sprite
            gfx = Graphics.FromImage(_endBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picEndResource.Width, picEndResource.Height));
            if (cmbEndSprite.SelectedIndex > 0 && _endTileset != null)
            {
                gfx.DrawImage(_endTileset, new Rectangle(0, 0, _endTileset.Width, _endTileset.Height),
                    new Rectangle(0, 0, _endTileset.Width, _endTileset.Height), GraphicsUnit.Pixel);
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
            PacketSender.SendCreateObject(GameObjectType.Resource);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstResources.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("resourceeditor", "deleteprompt"),
                        Strings.Get("resourceeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstResources.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
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
                if (DarkMessageBox.ShowWarning(Strings.Get("resourceeditor", "undoprompt"),
                        Strings.Get("resourceeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
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
            _editorItem.Drops[(int)nudDropIndex.Value - 1].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item,
                cmbItem.SelectedIndex - 1);
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Animation = Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
        }

        private void nudDropAmount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Drops[(int)nudDropIndex.Value - 1].Amount = (int) nudDropAmount.Value;
        }

        private void nudMinHp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.MinHP = (int) nudMinHp.Value;
        }

        private void nudMaxHp_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.MaxHP = (int) nudMaxHp.Value;
        }

        private void nudDropIndex_ValueChanged(object sender, EventArgs e)
        {
            UpdateDropValues();
        }
    }
}