using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Controls;
using DarkUI.Forms;

using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmProjectile : EditorForm
    {

        private List<ProjectileBase> mChanged = new List<ProjectileBase>();

        private string mCopiedItem;

        private Bitmap mDirectionGrid;

        private ProjectileBase mEditorItem;

        private List<string> mKnownFolders = new List<string>();

        public FrmProjectile()
        {
            ApplyHooks();
            InitializeComponent();

            lstGameObjects.Init(UpdateToolStripItems, AssignEditorItem, toolStripItemNew_Click, toolStripItemCopy_Click, toolStripItemUndo_Click, toolStripItemPaste_Click, toolStripItemDelete_Click);
        }
        private void AssignEditorItem(Guid id)
        {
            mEditorItem = ProjectileBase.Get(id);
            UpdateEditor();
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Projectile)
            {
                InitEditor();
                if (mEditorItem != null && !ProjectileBase.Lookup.Values.Contains(mEditorItem))
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

        private void frmProjectile_Load(object sender, EventArgs e)
        {
            mDirectionGrid = new Bitmap("resources/misc/directions.png");
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(AnimationBase.Names);

            cmbItem.Items.Clear();
            cmbItem.Items.Add(Strings.General.none);
            cmbItem.Items.AddRange(ItemBase.Names);

            cmbSpell.Items.Clear();
            cmbSpell.Items.Add(Strings.General.none);
            cmbSpell.Items.AddRange(SpellBase.Names);

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.ProjectileEditor.title;
            toolStripItemNew.Text = Strings.ProjectileEditor.New;
            toolStripItemDelete.Text = Strings.ProjectileEditor.delete;
            toolStripItemCopy.Text = Strings.ProjectileEditor.copy;
            toolStripItemPaste.Text = Strings.ProjectileEditor.paste;
            toolStripItemUndo.Text = Strings.ProjectileEditor.undo;

            grpProjectiles.Text = Strings.ProjectileEditor.projectiles;

            grpProperties.Text = Strings.ProjectileEditor.properties;
            lblName.Text = Strings.ProjectileEditor.name;
            lblSpeed.Text = Strings.ProjectileEditor.speed;
            lblSpawn.Text = Strings.ProjectileEditor.spawndelay;
            lblAmount.Text = Strings.ProjectileEditor.quantity;
            lblRange.Text = Strings.ProjectileEditor.range;
            lblKnockback.Text = Strings.ProjectileEditor.knockback;
            lblSpell.Text = Strings.ProjectileEditor.spell;
            chkGrapple.Text = Strings.ProjectileEditor.grapple;

            grpSpawns.Text = Strings.ProjectileEditor.spawns;

            grpAnimations.Text = Strings.ProjectileEditor.animations;
            lblAnimation.Text = Strings.ProjectileEditor.animation;
            chkRotation.Text = Strings.ProjectileEditor.autorotate;
            btnAdd.Text = Strings.ProjectileEditor.addanimation;
            btnRemove.Text = Strings.ProjectileEditor.removeanimation;

            grpCollisions.Text = Strings.ProjectileEditor.collisions;
            chkIgnoreMapBlocks.Text = Strings.ProjectileEditor.ignoreblocks;
            chkIgnoreActiveResources.Text = Strings.ProjectileEditor.ignoreactiveresources;
            chkIgnoreInactiveResources.Text = Strings.ProjectileEditor.ignoreinactiveresources;
            chkIgnoreZDimensionBlocks.Text = Strings.ProjectileEditor.ignorezdimension;
            chkPierce.Text = Strings.ProjectileEditor.piercetarget;

            grpAmmo.Text = Strings.ProjectileEditor.ammo;
            lblAmmoItem.Text = Strings.ProjectileEditor.ammoitem;
            lblAmmoAmount.Text = Strings.ProjectileEditor.ammoamount;

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.ProjectileEditor.sortchronologically;
            txtSearch.Text = Strings.ProjectileEditor.searchplaceholder;
            lblFolder.Text = Strings.ProjectileEditor.folderlabel;

            btnSave.Text = Strings.ProjectileEditor.save;
            btnCancel.Text = Strings.ProjectileEditor.cancel;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                nudSpeed.Value = mEditorItem.Speed;
                nudSpawn.Value = mEditorItem.Delay;
                nudAmount.Value = mEditorItem.Quantity;
                nudRange.Value = mEditorItem.Range;
                cmbSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.SpellId) + 1;
                nudKnockback.Value = mEditorItem.Knockback;
                chkIgnoreMapBlocks.Checked = mEditorItem.IgnoreMapBlocks;
                chkIgnoreActiveResources.Checked = mEditorItem.IgnoreActiveResources;
                chkIgnoreInactiveResources.Checked = mEditorItem.IgnoreExhaustedResources;
                chkIgnoreZDimensionBlocks.Checked = mEditorItem.IgnoreZDimension;
                chkGrapple.Checked = mEditorItem.GrappleHook;
                chkPierce.Checked = mEditorItem.PierceTarget;
                cmbItem.SelectedIndex = ItemBase.ListIndex(mEditorItem.AmmoItemId) + 1;
                nudConsume.Value = mEditorItem.AmmoRequired;

                if (lstAnimations.SelectedIndex < 0)
                {
                    lstAnimations.SelectedIndex = 0;
                }

                UpdateAnimationData(0);
                lstAnimations.SelectedIndex = 0;

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

        private void UpdateAnimationData(int index)
        {
            UpdateAnimations(true);
            cmbAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.Animations[index].AnimationId) + 1;
            scrlSpawnRange.Value = Math.Min(mEditorItem.Animations[index].SpawnRange, scrlSpawnRange.Maximum);
            chkRotation.Checked = mEditorItem.Animations[index].AutoRotate;
            UpdateAnimations(true);
        }

        private void UpdateAnimations(bool saveIndex = true)
        {
            var n = 1;
            var selectedIndex = 0;

            // if there are no animations, add one by default.
            if (mEditorItem.Animations.Count == 0)
            {
                mEditorItem.Animations.Add(new ProjectileAnimation(Guid.Empty, mEditorItem.Quantity, false));
            }

            //Update the spawn range maximum
            if (nudAmount.Value < scrlSpawnRange.Value)
            {
                scrlSpawnRange.Value = (int) nudAmount.Value;
            }

            scrlSpawnRange.Maximum = (int) nudAmount.Value;

            //Save the index
            if (saveIndex == true)
            {
                selectedIndex = lstAnimations.SelectedIndex;
            }

            // Add the animations to the list
            lstAnimations.Items.Clear();
            for (var i = 0; i < mEditorItem.Animations.Count; i++)
            {
                if (mEditorItem.Animations[i].AnimationId != Guid.Empty)
                {
                    lstAnimations.Items.Add(
                        Strings.ProjectileEditor.animationline.ToString(
                            n, mEditorItem.Animations[i].SpawnRange,
                            AnimationBase.GetName(mEditorItem.Animations[i].AnimationId)
                        )
                    );
                }
                else
                {
                    lstAnimations.Items.Add(
                        Strings.ProjectileEditor.animationline.ToString(
                            n, mEditorItem.Animations[i].SpawnRange, Strings.General.none
                        )
                    );
                }

                n = mEditorItem.Animations[i].SpawnRange + 1;
            }

            lstAnimations.SelectedIndex = selectedIndex;
            if (lstAnimations.SelectedIndex < 0)
            {
                lstAnimations.SelectedIndex = 0;
            }

            if (lstAnimations.SelectedIndex > 0)
            {
                lblSpawnRange.Text = Strings.ProjectileEditor.spawnrange.ToString(
                    mEditorItem.Animations[lstAnimations.SelectedIndex - 1].SpawnRange + 1,
                    mEditorItem.Animations[lstAnimations.SelectedIndex].SpawnRange
                );
            }
            else
            {
                lblSpawnRange.Text = Strings.ProjectileEditor.spawnrange.ToString(
                    1, mEditorItem.Animations[lstAnimations.SelectedIndex].SpawnRange
                );
            }
        }

        private void Render()
        {
            Bitmap img;
            if (picSpawns.BackgroundImage == null)
            {
                img = new Bitmap(160, 160);
                picSpawns.BackgroundImage = img;
            }
            else
            {
                img = (Bitmap) picSpawns.BackgroundImage;
            }

            var gfx = Graphics.FromImage(img);
            gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, picSpawns.Width, picSpawns.Height));

            for (var x = 0; x < ProjectileBase.SPAWN_LOCATIONS_WIDTH; x++)
            {
                for (var y = 0; y < ProjectileBase.SPAWN_LOCATIONS_HEIGHT; y++)
                {
                    gfx.DrawImage(
                        mDirectionGrid, new Rectangle(x * 32, y * 32, 32, 32), new Rectangle(0, 0, 32, 32),
                        GraphicsUnit.Pixel
                    );

                    for (var i = 0; i < ProjectileBase.MAX_PROJECTILE_DIRECTIONS; i++)
                    {
                        if (mEditorItem.SpawnLocations[x, y].Directions[i] == true)
                        {
                            gfx.DrawImage(
                                mDirectionGrid,
                                new Rectangle(
                                    x * 32 + DirectionOffsetX(i), y * 32 + DirectionOffsetY(i), (32 - 2) / 3,
                                    (32 - 2) / 3
                                ),
                                new Rectangle(
                                    32 + DirectionOffsetX(i), DirectionOffsetY(i), (32 - 2) / 3, (32 - 2) / 3
                                ), GraphicsUnit.Pixel
                            );
                        }
                    }
                }
            }

            gfx.DrawImage(
                mDirectionGrid,
                new Rectangle(160 / 2 - (32 - 2) / 3 / 2, 160 / 2 - (32 - 2) / 3 / 2, (32 - 2) / 3, (32 - 2) / 3),
                new Rectangle(43, 11, (32 - 2) / 3, (32 - 2) / 3), GraphicsUnit.Pixel
            );

            gfx.Dispose();
            picSpawns.Refresh();
        }

        private int DirectionOffsetX(int dir)
        {
            switch (dir)
            {
                case 0: //Up
                    return 10;
                case 1: //Down
                    return 10;
                case 2: //Left
                    return 1;
                case 3: //Right
                    return 20;
                case 4: //UpLeft
                    return 1;
                case 5: //UpRight
                    return 20;
                case 6: //DownLeft
                    return 1;
                case 7: //DownRight
                    return 20;
                default:
                    return 1;
            }
        }

        private int DirectionOffsetY(int dir)
        {
            switch (dir)
            {
                case 0: //Up
                    return 1;
                case 1: //Down
                    return 20;
                case 2: //Left
                    return 10;
                case 3: //Right
                    return 10;
                case 4: //UpLeft
                    return 1;
                case 5: //UpRight
                    return 1;
                case 6: //DownLeft
                    return 20;
                case 7: //DownRight
                    return 20;
                default:
                    return 1;
            }
        }

        private int FindDirection(int x, int y)
        {
            switch (x)
            {
                case 0: //Left
                    switch (y)
                    {
                        case 0: //Up
                            return 4;
                        case 1: //Center
                            return 2;
                        case 2: //Down
                            return 6;
                    }

                    return 0;
                case 1: //Center
                    switch (y)
                    {
                        case 0: //Up
                            return 0;
                        case 2: //Down
                            return 1;
                    }

                    return 0;
                case 2: //Right
                    switch (y)
                    {
                        case 0: //Up
                            return 5;
                        case 1: //Center
                            return 3;
                        case 2: //Down
                            return 7;
                    }

                    return 0;
                default:
                    return 0;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Name = txtName.Text;
            lstGameObjects.UpdateText(txtName.Text);
        }

        private void chkRotation_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Animations[lstAnimations.SelectedIndex].AutoRotate = chkRotation.Checked;
        }

        private void picSpawns_MouseDown(object sender, MouseEventArgs e)
        {
            double scaledX = e.X * (160f / picSpawns.Width);
            double scaledY = e.Y * (160f / picSpawns.Height);
            var x = scaledX / 32;
            var y = scaledY / 32;
            double i, j;

            x = Math.Floor(x);
            y = Math.Floor(y);

            if (x > 4 || y > 4)
            {
                return;
            }

            i = (scaledX - x * 32) / (32 / 3);
            j = (scaledY - y * 32) / (32 / 3);

            i = Math.Floor(i);
            j = Math.Floor(j);

            mEditorItem.SpawnLocations[(int) x, (int) y].Directions[FindDirection((int) i, (int) j)] =
                !mEditorItem.SpawnLocations[(int) x, (int) y].Directions[FindDirection((int) i, (int) j)];

            Render();
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreMapBlocks = chkIgnoreMapBlocks.Checked;
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreActiveResources = chkIgnoreActiveResources.Checked;
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreExhaustedResources = chkIgnoreInactiveResources.Checked;
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.IgnoreZDimension = chkIgnoreZDimensionBlocks.Checked;
        }

        private void chkPierce_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.PierceTarget = chkPierce.Checked;
        }

        private void chkGrapple_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.GrappleHook = chkGrapple.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Clone the previous animation to save time, set the end point to always be the quantity of spawns.
            mEditorItem.Animations.Add(
                new ProjectileAnimation(
                    mEditorItem.Animations[mEditorItem.Animations.Count - 1].AnimationId, mEditorItem.Quantity,
                    mEditorItem.Animations[mEditorItem.Animations.Count - 1].AutoRotate
                )
            );

            UpdateAnimations();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (mEditorItem.Animations.Count > 1)
            {
                mEditorItem.Animations.RemoveAt(mEditorItem.Animations.Count - 1);
                lstAnimations.SelectedIndex = 0;
                UpdateAnimations(false);
            }
        }

        private void scrlSpawnRange_Scroll(object sender, ScrollValueEventArgs e)
        {
            mEditorItem.Animations[lstAnimations.SelectedIndex].SpawnRange = scrlSpawnRange.Value;
            UpdateAnimations();
        }

        private void lstAnimations_Click(object sender, EventArgs e)
        {
            if (lstAnimations.SelectedIndex > -1)
            {
                UpdateAnimationData(lstAnimations.SelectedIndex);
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Projectile);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ProjectileEditor.deleteprompt, Strings.ProjectileEditor.deletetitle,
                        DarkDialogButton.YesNo, Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstGameObjects.Focused)
            {
                mCopiedItem = mEditorItem.JsonData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused)
            {
                mEditorItem.Load(mCopiedItem, true);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ProjectileEditor.undoprompt, Strings.ProjectileEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
                    UpdateEditor();
                }
            }
        }

        private void UpdateToolStripItems()
        {
            toolStripItemCopy.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstGameObjects.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstGameObjects.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstGameObjects.Focused;
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

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Ammo = ItemBase.Get(ItemBase.IdFromList(cmbItem.SelectedIndex - 1));
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animations[lstAnimations.SelectedIndex].AnimationId =
                AnimationBase.IdFromList(cmbAnimation.SelectedIndex - 1);

            UpdateAnimations();
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Speed = (int) nudSpeed.Value;
        }

        private void nudSpawnDelay_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Delay = (int) nudSpawn.Value;
        }

        private void nudAmount_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Quantity = (int) nudAmount.Value;
            UpdateAnimations();
        }

        private void nudRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Range = (int) nudRange.Value;
        }

        private void nudKnockback_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Knockback = (int) nudKnockback.Value;
        }

        private void nudConsume_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.AmmoRequired = (int) nudConsume.Value;
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSpell.SelectedIndex > 0)
            {
                mEditorItem.Spell = SpellBase.Get(SpellBase.IdFromList(cmbSpell.SelectedIndex - 1));
            }
            else
            {
                mEditorItem.Spell = null;
            }
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in ProjectileBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ProjectileBase) itm.Value).Folder) &&
                    !mFolders.Contains(((ProjectileBase) itm.Value).Folder))
                {
                    mFolders.Add(((ProjectileBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((ProjectileBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((ProjectileBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            var items = ProjectileBase.Lookup.OrderBy(p => p.Value?.TimeCreated).Select(pair => new KeyValuePair<Guid, KeyValuePair<string, string>>(pair.Key,
                new KeyValuePair<string, string>(((ProjectileBase)pair.Value)?.Name ?? Models.DatabaseObject<ProjectileBase>.Deleted, ((ProjectileBase)pair.Value)?.Folder ?? ""))).ToArray();
            lstGameObjects.Repopulate(items, mFolders, btnChronological.Checked, CustomSearch(), txtSearch.Text);
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ProjectileEditor.folderprompt, Strings.ProjectileEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    lstGameObjects.ExpandFolder(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            InitEditor();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = Strings.ProjectileEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.ProjectileEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                   txtSearch.Text != Strings.ProjectileEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.ProjectileEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
