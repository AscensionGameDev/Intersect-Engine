using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Editor.Classes.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Classes
{
    public partial class FrmResource : EditorForm
    {
        private List<ResourceBase> mChanged = new List<ResourceBase>();
        private string mCopiedItem;
        private ResourceBase mEditorItem;
        private Bitmap mEndBitmap;
        private Bitmap mEndTileset;
        private Bitmap mInitialBitmap;

        private Bitmap mInitialTileset;

        //General Editting Variables
        bool mTMouseDown;

        public FrmResource()
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
                if (mEditorItem != null && !ResourceBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (var item in mChanged)
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
            foreach (var item in mChanged)
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
            if (mChangingName) return;
            mEditorItem =
                ResourceBase.Lookup.Get<ResourceBase>(
                    Database.GameObjectIdFromList(GameObjectType.Resource, lstResources.SelectedIndex));
            UpdateEditor();
        }

        private void frmResource_Load(object sender, EventArgs e)
        {
            mInitialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            mEndBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
            cmbInitialSprite.Items.Clear();
            cmbEndSprite.Items.Clear();
            cmbInitialSprite.Items.Add(Strings.General.none);
            cmbEndSprite.Items.Add(Strings.General.none);
            string[] resources = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Resource);
            for (int i = 0; i < resources.Length; i++)
            {
                cmbInitialSprite.Items.Add(resources[i]);
                cmbEndSprite.Items.Add(resources[i]);
            }
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbDropItem.Items.Clear();
            cmbDropItem.Items.Add(Strings.General.none);
            cmbDropItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.ResourceEditor.title;
            toolStripItemNew.Text = Strings.ResourceEditor.New;
            toolStripItemDelete.Text = Strings.ResourceEditor.delete;
            toolStripItemCopy.Text = Strings.ResourceEditor.copy;
            toolStripItemPaste.Text = Strings.ResourceEditor.paste;
            toolStripItemUndo.Text = Strings.ResourceEditor.undo;

            grpResources.Text = Strings.ResourceEditor.resources;

            grpGeneral.Text = Strings.ResourceEditor.general;
            lblName.Text = Strings.ResourceEditor.name;
            lblToolType.Text = Strings.ResourceEditor.tooltype;
            lblHP.Text = Strings.ResourceEditor.minhp;
            lblMaxHp.Text = Strings.ResourceEditor.maxhp;
            lblSpawnDuration.Text = Strings.ResourceEditor.spawnduration;
            lblAnimation.Text = Strings.ResourceEditor.animation;
            chkWalkableBefore.Text = Strings.ResourceEditor.walkablebefore;
            chkWalkableAfter.Text = Strings.ResourceEditor.walkableafter;
            btnRequirements.Text = Strings.ResourceEditor.requirements;

            grpDrops.Text = Strings.ResourceEditor.drops;
            lblDropItem.Text = Strings.ResourceEditor.dropitem;
            lblDropAmount.Text = Strings.ResourceEditor.dropamount;
            lblDropChance.Text = Strings.ResourceEditor.dropchance;
            btnDropAdd.Text = Strings.ResourceEditor.dropadd;
            btnDropRemove.Text = Strings.ResourceEditor.dropremove;

            grpGraphics.Text = Strings.ResourceEditor.graphics;
            lblPic.Text = Strings.ResourceEditor.initialgraphic;
            lblPic2.Text = Strings.ResourceEditor.exhaustedgraphic;

            btnSave.Text = Strings.ResourceEditor.save;
            btnCancel.Text = Strings.ResourceEditor.cancel;
        }

        public void InitEditor()
        {
            lstResources.Items.Clear();
            lstResources.Items.AddRange(Database.GetGameObjectList(GameObjectType.Resource));
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.General.none);
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbToolType.SelectedIndex = mEditorItem.Tool + 1;
                nudSpawnDuration.Value = mEditorItem.SpawnDuration;
                cmbAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.Animation) +
                    1;
                nudMinHp.Value = mEditorItem.MinHp;
                nudMaxHp.Value = mEditorItem.MaxHp;
                chkWalkableBefore.Checked = mEditorItem.WalkableBefore;
                chkWalkableAfter.Checked = mEditorItem.WalkableAfter;
                cmbInitialSprite.SelectedIndex = cmbInitialSprite.FindString(TextUtils.NullToNone(TextUtils.NullToNone(mEditorItem.InitialGraphic)));
                cmbEndSprite.SelectedIndex = cmbEndSprite.FindString(TextUtils.NullToNone(TextUtils.NullToNone(mEditorItem.EndGraphic)));
                UpdateDropValues();
                Render();
                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
                }
            }
            else
            {
                pnlContainer.Hide();
            }
            UpdateToolStripItems();
        }

        private void UpdateDropValues(bool keepIndex = false)
        {
            var index = lstDrops.SelectedIndex;
            lstDrops.Items.Clear();

            var drops = mEditorItem.Drops.ToArray();
            foreach (var drop in drops)
            {
                if (ItemBase.Lookup.Get<ItemBase>(drop.ItemNum) == null) mEditorItem.Drops.Remove(drop);
            }
            for (int i = 0; i < mEditorItem.Drops.Count; i++)
            {
                if (mEditorItem.Drops[i].ItemNum != -1)
                {
                    lstDrops.Items.Add(Strings.ResourceEditor.dropdisplay.ToString(ItemBase.GetName(mEditorItem.Drops[i].ItemNum), mEditorItem.Drops[i].Amount, mEditorItem.Drops[i].Chance));
                }
                else
                {
                    lstDrops.Items.Add(TextUtils.None);
                }
            }
            if (keepIndex && index < lstDrops.Items.Count) lstDrops.SelectedIndex = index;
        }

        private void nudSpawnDuration_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SpawnDuration = (int) nudSpawnDuration.Value;
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Tool = cmbToolType.SelectedIndex - 1;
        }

        private void chkWalkableBefore_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.WalkableBefore = chkWalkableBefore.Checked;
        }

        private void chkWalkableAfter_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.WalkableAfter = chkWalkableAfter.Checked;
        }

        private void cmbInitialSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInitialSprite.SelectedIndex > 0)
            {
                mEditorItem.InitialGraphic = cmbInitialSprite.Text;
                if (File.Exists("resources/resources/" + cmbInitialSprite.Text))
                {
                    mInitialTileset = (Bitmap) Image.FromFile("resources/resources/" + cmbInitialSprite.Text);
                    picInitialResource.Width = mInitialTileset.Width;
                    picInitialResource.Height = mInitialTileset.Height;
                    mInitialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
                }
                else
                {
                    mInitialTileset = null;
                }
            }
            else
            {
                mEditorItem.InitialGraphic = null;
                mInitialTileset = null;
            }
            Render();
        }

        private void cmbEndSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndSprite.SelectedIndex > 0)
            {
                mEditorItem.EndGraphic = cmbEndSprite.Text;
                if (File.Exists("resources/resources/" + cmbEndSprite.Text))
                {
                    mEndTileset = (Bitmap) Image.FromFile("resources/resources/" + cmbEndSprite.Text);
                    picEndResource.Width = mEndTileset.Width;
                    picEndResource.Height = mEndTileset.Height;
                    mEndBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
                }
                else
                {
                    mEndTileset = null;
                }
            }
            else
            {
                mEditorItem.EndGraphic = null;
                mEndTileset = null;
            }
            Render();
        }

        public void Render()
        {
            Pen whitePen = new Pen(System.Drawing.Color.Red, 1);

            // Initial Sprite
            var gfx = Graphics.FromImage(mInitialBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picInitialResource.Width, picInitialResource.Height));
            if (cmbInitialSprite.SelectedIndex > 0 && mInitialTileset != null)
            {
                gfx.DrawImage(mInitialTileset, new Rectangle(0, 0, mInitialTileset.Width, mInitialTileset.Height),
                    new Rectangle(0, 0, mInitialTileset.Width, mInitialTileset.Height), GraphicsUnit.Pixel);
            }
            gfx.Dispose();
            gfx = picInitialResource.CreateGraphics();
            gfx.DrawImageUnscaled(mInitialBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();

            // End Sprite
            gfx = Graphics.FromImage(mEndBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picEndResource.Width, picEndResource.Height));
            if (cmbEndSprite.SelectedIndex > 0 && mEndTileset != null)
            {
                gfx.DrawImage(mEndTileset, new Rectangle(0, 0, mEndTileset.Width, mEndTileset.Height),
                    new Rectangle(0, 0, mEndTileset.Width, mEndTileset.Height), GraphicsUnit.Pixel);
            }
            gfx.Dispose();
            gfx = picEndResource.CreateGraphics();
            gfx.DrawImageUnscaled(mEndBitmap, new System.Drawing.Point(0, 0));
            gfx.Dispose();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstResources.Items[lstResources.SelectedIndex] = txtName.Text;
            mChangingName = false;
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
            if (mEditorItem != null && lstResources.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.ResourceEditor.deleteprompt,
                        Strings.ResourceEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstResources.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstResources.Focused)
            {
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.ResourceEditor.undoprompt,
                        Strings.ResourceEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstResources.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstResources.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstResources.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstResources.Focused;
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
            var frm = new FrmDynamicRequirements(mEditorItem.HarvestingReqs, RequirementType.Resource);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animation =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
        }

        private void nudMinHp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MinHp = (int) nudMinHp.Value;
        }

        private void nudMaxHp_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.MaxHp = (int) nudMaxHp.Value;
        }


        private void cmbDropItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1 && lstDrops.SelectedIndex < mEditorItem.Drops.Count)
            {
                mEditorItem.Drops[lstDrops.SelectedIndex].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item, cmbDropItem.SelectedIndex - 1);
            }
            UpdateDropValues(true);
        }

        private void nudDropAmount_ValueChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex < lstDrops.Items.Count) return;
            mEditorItem.Drops[(int)lstDrops.SelectedIndex].Amount = (int)nudDropAmount.Value;
            UpdateDropValues(true);
        }

        private void lstDrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                cmbDropItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, mEditorItem.Drops[lstDrops.SelectedIndex].ItemNum) + 1;
                nudDropAmount.Value = mEditorItem.Drops[lstDrops.SelectedIndex].Amount;
                nudDropChance.Value = (decimal)mEditorItem.Drops[lstDrops.SelectedIndex].Chance;
            }
        }

        private void btnDropAdd_Click(object sender, EventArgs e)
        {
            mEditorItem.Drops.Add(new ResourceBase.ResourceDrop());
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].ItemNum = Database.GameObjectIdFromList(GameObjectType.Item, cmbDropItem.SelectedIndex - 1);
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Amount = (int)nudDropAmount.Value;
            mEditorItem.Drops[mEditorItem.Drops.Count - 1].Chance = (double)nudDropChance.Value;

            UpdateDropValues();
        }

        private void btnDropRemove_Click(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex > -1)
            {
                int i = lstDrops.SelectedIndex;
                lstDrops.Items.RemoveAt(i);
                mEditorItem.Drops.RemoveAt(i);
            }
            UpdateDropValues(true);
        }

        private void nudDropChance_ValueChanged(object sender, EventArgs e)
        {
            if (lstDrops.SelectedIndex < lstDrops.Items.Count) return;
            mEditorItem.Drops[(int)lstDrops.SelectedIndex].Chance = (double)nudDropChance.Value;
            UpdateDropValues(true);
        }
    }
}