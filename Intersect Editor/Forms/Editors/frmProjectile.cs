using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;

namespace Intersect.Editor.Classes
{
    public partial class frmProjectile : EditorForm
    {
        private List<ProjectileBase> _changed = new List<ProjectileBase>();
        private byte[] _copiedItem;

        private Bitmap _directionGrid;
        private ProjectileBase _editorItem;

        public frmProjectile()
        {
            ApplyHooks();
            InitializeComponent();
            lstProjectiles.LostFocus += itemList_FocusChanged;
            lstProjectiles.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Projectile)
            {
                InitEditor();
                if (_editorItem != null && !ProjectileBase.Lookup.Values.Contains(_editorItem))
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

        private void lstProjectiles_Click(object sender, EventArgs e)
        {
            _editorItem =
                ProjectileBase.Lookup.Get<ProjectileBase>(Database.GameObjectIdFromList(GameObjectType.Projectile,
                    lstProjectiles.SelectedIndex));
            UpdateEditor();
        }

        private void frmProjectile_Load(object sender, EventArgs e)
        {
            _directionGrid = new Bitmap("resources/misc/directions.png");
            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));

            cmbItem.Items.Clear();
            cmbItem.Items.Add("None.");
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));

            cmbSpell.Items.Clear();
            cmbSpell.Items.Add("None.");
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("projectileeditor", "title");
            toolStripItemNew.Text = Strings.Get("projectileeditor", "new");
            toolStripItemDelete.Text = Strings.Get("projectileeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("projectileeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("projectileeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("projectileeditor", "undo");

            grpProjectiles.Text = Strings.Get("projectileeditor", "projectiles");

            grpProperties.Text = Strings.Get("projectileeditor", "properties");
            lblName.Text = Strings.Get("projectileeditor", "name");
            lblSpeed.Text = Strings.Get("projectileeditor", "speed");
            lblSpawn.Text = Strings.Get("projectileeditor", "spawndelay");
            lblAmount.Text = Strings.Get("projectileeditor", "quantity");
            lblRange.Text = Strings.Get("projectileeditor", "range");
            lblKnockback.Text = Strings.Get("projectileeditor", "knockback");
            lblSpell.Text = Strings.Get("projectileeditor", "spell");
            chkGrapple.Text = Strings.Get("projectileeditor", "grapple");
            chkHoming.Text = Strings.Get("projectileeditor", "homing");

            grpSpawns.Text = Strings.Get("projectileeditor", "spawns");

            grpAnimations.Text = Strings.Get("projectileeditor", "animations");
            lblAnimation.Text = Strings.Get("projectileeditor", "animation");
            chkRotation.Text = Strings.Get("projectileeditor", "autorotate");
            btnAdd.Text = Strings.Get("projectileeditor", "addanimation");
            btnRemove.Text = Strings.Get("projectileeditor", "removeanimation");

            grpCollisions.Text = Strings.Get("projectileeditor", "collisions");
            chkIgnoreMapBlocks.Text = Strings.Get("projectileeditor", "ignoreblocks");
            chkIgnoreActiveResources.Text = Strings.Get("projectileeditor", "ignoreactiveresources");
            chkIgnoreInactiveResources.Text = Strings.Get("projectileeditor", "ignoreinactiveresources");
            chkIgnoreZDimensionBlocks.Text = Strings.Get("projectileeditor", "ignorezdimension");

            grpAmmo.Text = Strings.Get("projectileeditor", "ammo");
            lblAmmoItem.Text = Strings.Get("projectileeditor", "ammoitem");
            lblAmmoAmount.Text = Strings.Get("projectileeditor", "ammoamount");

            btnSave.Text = Strings.Get("projectileeditor", "save");
            btnCancel.Text = Strings.Get("projectileeditor", "cancel");
        }

        public void InitEditor()
        {
            lstProjectiles.Items.Clear();
            lstProjectiles.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                nudSpeed.Value = _editorItem.Speed;
                nudSpawn.Value = _editorItem.Delay;
                nudAmount.Value = _editorItem.Quantity;
                nudRange.Value = _editorItem.Range;
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell, _editorItem.Spell) + 1;
                nudKnockback.Value = _editorItem.Knockback;
                chkIgnoreMapBlocks.Checked = _editorItem.IgnoreMapBlocks;
                chkIgnoreActiveResources.Checked = _editorItem.IgnoreActiveResources;
                chkIgnoreInactiveResources.Checked = _editorItem.IgnoreExhaustedResources;
                chkIgnoreZDimensionBlocks.Checked = _editorItem.IgnoreZDimension;
                chkHoming.Checked = _editorItem.Homing;
                chkGrapple.Checked = _editorItem.GrappleHook;
                cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Item, _editorItem.Ammo) + 1;
                nudConsume.Value = _editorItem.AmmoRequired;

                if (lstAnimations.SelectedIndex < 0)
                {
                    lstAnimations.SelectedIndex = 0;
                }
                updateAnimationData(0);
                lstAnimations.SelectedIndex = 0;

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

        private void updateAnimationData(int index)
        {
            updateAnimations(true);
            cmbAnimation.SelectedIndex =
                Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.Animations[index].Animation) + 1;
            scrlSpawnRange.Value = Math.Min(_editorItem.Animations[index].SpawnRange, scrlSpawnRange.Maximum);
            chkRotation.Checked = _editorItem.Animations[index].AutoRotate;
            updateAnimations(true);
        }

        private void updateAnimations(bool SaveIndex = true)
        {
            int n = 1;
            int selectedIndex = 0;

            // if there are no animations, add one by default.
            if (_editorItem.Animations.Count == 0)
            {
                _editorItem.Animations.Add(new ProjectileAnimation(-1, _editorItem.Quantity, false));
            }

            //Update the spawn range maximum
            if (nudAmount.Value < scrlSpawnRange.Value)
            {
                scrlSpawnRange.Value = (int) nudAmount.Value;
            }
            scrlSpawnRange.Maximum = (int) nudAmount.Value;

            //Save the index
            if (SaveIndex == true)
            {
                selectedIndex = lstAnimations.SelectedIndex;
            }

            // Add the animations to the list
            lstAnimations.Items.Clear();
            for (int i = 0; i < _editorItem.Animations.Count; i++)
            {
                if (_editorItem.Animations[i].Animation != -1)
                {
                    lstAnimations.Items.Add(Strings.Get("projectileeditor", "animationline", n,
                        _editorItem.Animations[i].SpawnRange,
                        AnimationBase.GetName(_editorItem.Animations[i].Animation)));
                }
                else
                {
                    lstAnimations.Items.Add(Strings.Get("projectileeditor", "animationline", n,
                        _editorItem.Animations[i].SpawnRange, Strings.Get("general", "none")));
                }
                n = _editorItem.Animations[i].SpawnRange + 1;
            }
            lstAnimations.SelectedIndex = selectedIndex;
            if (lstAnimations.SelectedIndex < 0)
            {
                lstAnimations.SelectedIndex = 0;
            }

            if (lstAnimations.SelectedIndex > 0)
            {
                lblSpawnRange.Text = Strings.Get("projectileeditor", "spawnrange",
                    (_editorItem.Animations[lstAnimations.SelectedIndex - 1].SpawnRange + 1),
                    _editorItem.Animations[lstAnimations.SelectedIndex].SpawnRange);
            }
            else
            {
                lblSpawnRange.Text = Strings.Get("projectileeditor", "spawnrange", 1,
                    _editorItem.Animations[lstAnimations.SelectedIndex].SpawnRange);
            }
        }

        private void Render()
        {
            Bitmap img;
            if (picSpawns.BackgroundImage == null)
            {
                img = new Bitmap(picSpawns.Width, picSpawns.Height);
                picSpawns.BackgroundImage = img;
            }
            else
            {
                img = (Bitmap) picSpawns.BackgroundImage;
            }
            var gfx = Graphics.FromImage(img);
            gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, picSpawns.Width, picSpawns.Height));

            for (var x = 0; x < ProjectileBase.SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < ProjectileBase.SpawnLocationsHeight; y++)
                {
                    gfx.DrawImage(_directionGrid, new Rectangle(x * 32, y * 32, 32, 32), new Rectangle(0, 0, 32, 32),
                        GraphicsUnit.Pixel);
                    for (var i = 0; i < ProjectileBase.MaxProjectileDirections; i++)
                    {
                        if (_editorItem.SpawnLocations[x, y].Directions[i] == true)
                        {
                            gfx.DrawImage(_directionGrid,
                                new Rectangle((x * 32) + DirectionOffsetX(i), (y * 32) + DirectionOffsetY(i),
                                    (32 - 2) / 3, (32 - 2) / 3),
                                new Rectangle(32 + DirectionOffsetX(i), DirectionOffsetY(i), (32 - 2) / 3,
                                    (32 - 2) / 3),
                                GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            gfx.DrawImage(_directionGrid,
                new Rectangle((picSpawns.Width / 2) - (((32 - 2) / 3) / 2),
                    (picSpawns.Height / 2) - (((32 - 2) / 3) / 2), (32 - 2) / 3, (32 - 2) / 3),
                new Rectangle(43, 11, (32 - 2) / 3, (32 - 2) / 3), GraphicsUnit.Pixel);
            gfx.Dispose();
            picSpawns.Refresh();
        }

        private int DirectionOffsetX(int Dir)
        {
            switch (Dir)
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

        private int DirectionOffsetY(int Dir)
        {
            switch (Dir)
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
            _editorItem.Name = txtName.Text;
            lstProjectiles.Items[Database.GameObjectListIndex(GameObjectType.Projectile, _editorItem.Index)] =
                txtName.Text;
        }

        private void chkHoming_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Homing = chkHoming.Checked;
        }

        private void chkRotation_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Animations[lstAnimations.SelectedIndex].AutoRotate = chkRotation.Checked;
        }

        private void picSpawns_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X / 32;
            double y = e.Y / 32;
            double i, j;

            x = Math.Floor(x);
            y = Math.Floor(y);

            i = (e.X - (x * 32)) / (32 / 3);
            j = (e.Y - (y * 32)) / (32 / 3);

            i = Math.Floor(i);
            j = Math.Floor(j);

            _editorItem.SpawnLocations[(int) x, (int) y].Directions[FindDirection((int) i, (int) j)] =
                !_editorItem.SpawnLocations[(int) x, (int) y].Directions[FindDirection((int) i, (int) j)];

            Render();
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IgnoreMapBlocks = chkIgnoreMapBlocks.Checked;
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IgnoreActiveResources = chkIgnoreActiveResources.Checked;
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IgnoreExhaustedResources = chkIgnoreInactiveResources.Checked;
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IgnoreZDimension = chkIgnoreZDimensionBlocks.Checked;
        }

        private void chkGrapple_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.GrappleHook = chkGrapple.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Clone the previous animation to save time, set the end point to always be the quantity of spawns.
            _editorItem.Animations.Add(
                new ProjectileAnimation(_editorItem.Animations[_editorItem.Animations.Count - 1].Animation,
                    _editorItem.Quantity,
                    _editorItem.Animations[_editorItem.Animations.Count - 1].AutoRotate));
            updateAnimations();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (_editorItem.Animations.Count > 1)
            {
                _editorItem.Animations.RemoveAt(_editorItem.Animations.Count - 1);
                lstAnimations.SelectedIndex = 0;
                updateAnimations(false);
            }
        }

        private void scrlSpawnRange_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.Animations[lstAnimations.SelectedIndex].SpawnRange = scrlSpawnRange.Value;
            updateAnimations();
        }

        private void lstAnimations_Click(object sender, EventArgs e)
        {
            if (lstAnimations.SelectedIndex > -1)
            {
                updateAnimationData(lstAnimations.SelectedIndex);
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Projectile);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstProjectiles.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("projectileeditor", "deleteprompt"),
                        Strings.Get("projectileeditor", "deletetitle"), DarkDialogButton.YesNo,
                        Properties.Resources.Icon) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstProjectiles.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstProjectiles.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("projectileeditor", "undoprompt"),
                        Strings.Get("projectileeditor", "undotitle"), DarkDialogButton.YesNo,
                        Properties.Resources.Icon) ==
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
            toolStripItemCopy.Enabled = _editorItem != null && lstProjectiles.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstProjectiles.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstProjectiles.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstProjectiles.Focused;
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

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Ammo = Database.GameObjectIdFromList(GameObjectType.Item, cmbItem.SelectedIndex - 1);
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Animations[lstAnimations.SelectedIndex].Animation =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
            updateAnimations();
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Speed = (int) nudSpeed.Value;
        }

        private void nudSpawnDelay_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Delay = (int) nudSpawn.Value;
        }

        private void nudAmount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Quantity = (int) nudAmount.Value;
            updateAnimations();
        }

        private void nudRange_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Range = (int) nudRange.Value;
        }

        private void nudKnockback_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Knockback = (int) nudKnockback.Value;
        }

        private void nudConsume_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.AmmoRequired = (int) nudConsume.Value;
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSpell.SelectedIndex > 0)
            {
                _editorItem.Spell = Database.GameObjectIdFromList(GameObjectType.Spell, cmbSpell.SelectedIndex - 1);
            }
            else
            {
                _editorItem.Spell = -1;
            }
        }
    }
}