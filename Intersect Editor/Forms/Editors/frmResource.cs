using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmResource : EditorForm
    {
        private List<ResourceBase> mChanged = new List<ResourceBase>();
        private string mCopiedItem;
        private ResourceBase mEditorItem;
        private Bitmap mEndBitmap;
        private Bitmap mEndGraphic;
        private Bitmap mInitialBitmap;
        private Bitmap mInitialGraphic;
        private bool mMouseDown;

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
            
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbDropItem.Items.Clear();
            cmbDropItem.Items.Add(Strings.General.none);
            cmbDropItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            InitLocalization();
            UpdateEditor();
        }

        private void PopulateInitialGraphicList()
        {
            cmbInitialSprite.Items.Clear();
            cmbInitialSprite.Items.Add(Strings.General.none);
            string[] resources = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Resource);
            if (mEditorItem.InitialGraphicFromTileset)
            {
                resources = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tileset);
            }
            for (int i = 0; i < resources.Length; i++)
            {
                cmbInitialSprite.Items.Add(resources[i]);
            }
            if (mEditorItem != null)
            {
                if (mEditorItem.InitialGraphic != null && cmbInitialSprite.Items.Contains(mEditorItem.InitialGraphic))
                {
                    cmbInitialSprite.SelectedIndex = cmbInitialSprite.FindString(TextUtils.NullToNone(TextUtils.NullToNone(mEditorItem.InitialGraphic)));
                    return;
                }
            }
            cmbInitialSprite.SelectedIndex = 0;
        }

        private void PopulateEndGraphicList()
        {
            cmbEndSprite.Items.Clear();
            cmbEndSprite.Items.Add(Strings.General.none);
            string[] resources = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Resource);
            if (mEditorItem.EndGraphicFromTileset)
            {
                resources = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Tileset);
            }
            for (int i = 0; i < resources.Length; i++)
            {
                cmbEndSprite.Items.Add(resources[i]);
            }
            if (mEditorItem != null)
            {
                if (mEditorItem.EndGraphic != null && cmbEndSprite.Items.Contains(mEditorItem.EndGraphic))
                {
                    cmbEndSprite.SelectedIndex = cmbEndSprite.FindString(TextUtils.NullToNone(TextUtils.NullToNone(mEditorItem.EndGraphic)));
                    return;
                }
            }
            cmbEndSprite.SelectedIndex = 0;
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
                chkInitialFromTileset.Checked = mEditorItem.InitialGraphicFromTileset;
                chkExhaustedFromTileset.Checked = mEditorItem.EndGraphicFromTileset;
                PopulateInitialGraphicList();
                PopulateEndGraphicList();
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
                var graphic = Path.Combine("resources", mEditorItem.InitialGraphicFromTileset ? "tilesets" : "resources", cmbInitialSprite.Text);
                if (File.Exists(graphic))
                {
                    mInitialGraphic = (Bitmap) Image.FromFile(graphic);
                    picInitialResource.Width = mInitialGraphic.Width;
                    picInitialResource.Height = mInitialGraphic.Height;
                    mInitialBitmap = new Bitmap(picInitialResource.Width, picInitialResource.Height);
                }
                else
                {
                    mInitialGraphic = null;
                }
            }
            else
            {
                mEditorItem.InitialGraphic = null;
                mInitialGraphic = null;
            }
            picInitialResource.Visible = mInitialGraphic != null;
            Render();
        }

        private void cmbEndSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEndSprite.SelectedIndex > 0)
            {
                mEditorItem.EndGraphic = cmbEndSprite.Text;
                var graphic = Path.Combine("resources", mEditorItem.EndGraphicFromTileset ? "tilesets" : "resources",cmbEndSprite.Text);
                if (File.Exists(graphic))
                {
                    mEndGraphic = (Bitmap) Image.FromFile(graphic);
                    picEndResource.Width = mEndGraphic.Width;
                    picEndResource.Height = mEndGraphic.Height;
                    mEndBitmap = new Bitmap(picEndResource.Width, picEndResource.Height);
                }
                else
                {
                    mEndGraphic = null;
                }
            }
            else
            {
                mEditorItem.EndGraphic = null;
                mEndGraphic = null;
            }
            picEndResource.Visible = mEndGraphic != null;
            Render();
        }

        public void Render()
        {
            if (mEditorItem == null) return;
            // Initial Sprite
            var gfx = Graphics.FromImage(mInitialBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picInitialResource.Width, picInitialResource.Height));
            if (cmbInitialSprite.SelectedIndex > 0 && mInitialGraphic != null)
            {
                gfx.DrawImage(mInitialGraphic, new Rectangle(0, 0, mInitialGraphic.Width, mInitialGraphic.Height),
                    new Rectangle(0, 0, mInitialGraphic.Width, mInitialGraphic.Height), GraphicsUnit.Pixel);
            }
            if (mEditorItem.InitialGraphicFromTileset)
            {
                var selX = mEditorItem.InitialTilesetX;
                var selY = mEditorItem.InitialTilesetY;
                var selW = mEditorItem.InitialTilesetWidth;
                var selH = mEditorItem.InitialTilesetHeight;
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
                gfx.DrawRectangle(new Pen(System.Drawing.Color.White, 2f),
                    new Rectangle(selX * Options.TileWidth, selY * Options.TileHeight,
                        Options.TileWidth + (selW * Options.TileWidth),
                        Options.TileHeight + (selH * Options.TileHeight)));
            }
            gfx.Dispose();
            gfx = picInitialResource.CreateGraphics();
            gfx.DrawImageUnscaled(mInitialBitmap, new System.Drawing.Point(0, 0));

            gfx.Dispose();

            // End Sprite
            gfx = Graphics.FromImage(mEndBitmap);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picEndResource.Width, picEndResource.Height));
            if (cmbEndSprite.SelectedIndex > 0 && mEndGraphic != null)
            {
                gfx.DrawImage(mEndGraphic, new Rectangle(0, 0, mEndGraphic.Width, mEndGraphic.Height),
                    new Rectangle(0, 0, mEndGraphic.Width, mEndGraphic.Height), GraphicsUnit.Pixel);
            }
            if (mEditorItem.EndGraphicFromTileset)
            {
                var selX = mEditorItem.EndTilesetX;
                var selY = mEditorItem.EndTilesetY;
                var selW = mEditorItem.EndTilesetWidth;
                var selH = mEditorItem.EndTilesetHeight;
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
                gfx.DrawRectangle(new Pen(System.Drawing.Color.White, 2f),
                    new Rectangle(selX * Options.TileWidth, selY * Options.TileHeight,
                        Options.TileWidth + (selW * Options.TileWidth),
                        Options.TileHeight + (selH * Options.TileHeight)));
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

        private void chkInitialFromTileset_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.InitialGraphicFromTileset = chkInitialFromTileset.Checked;
            PopulateInitialGraphicList();
        }

        private void chkExhaustedFromTileset_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.EndGraphicFromTileset = chkExhaustedFromTileset.Checked;
            PopulateEndGraphicList();
        }

        private void picInitialResource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picInitialResource.Width || e.Y > picInitialResource.Height)
            {
                return;
            }
            if (!chkInitialFromTileset.Checked) return;
            mMouseDown = true;
            mEditorItem.InitialTilesetX = (int)Math.Floor((double)e.X / Options.TileWidth);
            mEditorItem.InitialTilesetY = (int)Math.Floor((double)e.Y / Options.TileHeight);
            mEditorItem.InitialTilesetWidth = 0;
            mEditorItem.InitialTilesetHeight = 0;
            if (mEditorItem.InitialTilesetX < 0)
            {
                mEditorItem.InitialTilesetX = 0;
            }
            if (mEditorItem.InitialTilesetY < 0)
            {
                mEditorItem.InitialTilesetY = 0;
            }
            Render();
        }

        private void picInitialResource_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X > picInitialResource.Width || e.Y > picInitialResource.Height)
            {
                return;
            }
            if (!chkInitialFromTileset.Checked) return;
            var selX = mEditorItem.InitialTilesetX;
            var selY = mEditorItem.InitialTilesetY;
            var selW = mEditorItem.InitialTilesetWidth;
            var selH = mEditorItem.InitialTilesetHeight;
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
            mEditorItem.InitialTilesetX = selX;
            mEditorItem.InitialTilesetY = selY;
            mEditorItem.InitialTilesetWidth = selW;
            mEditorItem.InitialTilesetHeight = selH;
            mMouseDown = false;
            Render();
        }

        private void picInitialResource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picInitialResource.Width || e.Y > picInitialResource.Height)
            {
                return;
            }
            if (!chkInitialFromTileset.Checked) return;
            if (mMouseDown)
            {
                var tmpX = (int)Math.Floor((double)e.X / Options.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Options.TileHeight);
                mEditorItem.InitialTilesetWidth = tmpX - mEditorItem.InitialTilesetX;
                mEditorItem.InitialTilesetHeight = tmpY - mEditorItem.InitialTilesetY;
            }
            Render();
        }

        private void picExhustedResource_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picEndResource.Width || e.Y > picEndResource.Height)
            {
                return;
            }
            if (!chkExhaustedFromTileset.Checked) return;
            mMouseDown = true;
            mEditorItem.EndTilesetX = (int)Math.Floor((double)e.X / Options.TileWidth);
            mEditorItem.EndTilesetY = (int)Math.Floor((double)e.Y / Options.TileHeight);
            mEditorItem.EndTilesetWidth = 0;
            mEditorItem.EndTilesetHeight = 0;
            if (mEditorItem.EndTilesetX < 0)
            {
                mEditorItem.EndTilesetX = 0;
            }
            if (mEditorItem.EndTilesetY < 0)
            {
                mEditorItem.EndTilesetY = 0;
            }
            Render();
        }

        private void picExhaustedResource_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X > picEndResource.Width || e.Y > picEndResource.Height)
            {
                return;
            }
            if (!chkExhaustedFromTileset.Checked) return;
            var selX = mEditorItem.EndTilesetX;
            var selY = mEditorItem.EndTilesetY;
            var selW = mEditorItem.EndTilesetWidth;
            var selH = mEditorItem.EndTilesetHeight;
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
            mEditorItem.EndTilesetX = selX;
            mEditorItem.EndTilesetY = selY;
            mEditorItem.EndTilesetWidth = selW;
            mEditorItem.EndTilesetHeight = selH;
            mMouseDown = false;
            Render();
        }

        private void picExhaustedResource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picEndResource.Width || e.Y > picEndResource.Height)
            {
                return;
            }
            if (!chkExhaustedFromTileset.Checked) return;
            if (mMouseDown)
            {
                var tmpX = (int)Math.Floor((double)e.X / Options.TileWidth);
                var tmpY = (int)Math.Floor((double)e.Y / Options.TileHeight);
                mEditorItem.EndTilesetWidth = tmpX - mEditorItem.EndTilesetX;
                mEditorItem.EndTilesetHeight = tmpY - mEditorItem.EndTilesetY;
            }
            Render();
        }
    }
}