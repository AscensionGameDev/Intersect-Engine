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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Intersect_Library;
using Intersect_Library.GameObjects;


namespace Intersect_Editor.Classes
{
    public partial class frmProjectile : Form
    {
        private List<ProjectileBase> _changed = new List<ProjectileBase>();
        private ProjectileBase _editorItem = null;

        private Bitmap _directionGrid;

        public frmProjectile()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Projectile)
            {
                InitEditor();
                if (_editorItem != null && !ProjectileBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Projectile);
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

        private void lstProjectiles_Click(object sender, EventArgs e)
        {
            _editorItem = ProjectileBase.GetProjectile(Database.GameObjectIdFromList(GameObject.Projectile, lstProjectiles.SelectedIndex));
            UpdateEditor();
        }

        private void frmProjectile_Load(object sender, EventArgs e)
        {
            _directionGrid = new Bitmap("resources/misc/directions.png");
            scrlAnimation.Maximum = AnimationBase.ObjectCount() - 1;
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstProjectiles.Items.Clear();
            lstProjectiles.Items.AddRange(Database.GetGameObjectList(GameObject.Projectile));
            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                scrlSpeed.Value = _editorItem.Speed;
                scrlSpawn.Value = _editorItem.Delay;
                scrlAmount.Value = _editorItem.Quantity;
                scrlRange.Value = _editorItem.Range;
                scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spell);
                scrlKnockback.Value = _editorItem.Knockback;
                chkIgnoreMapBlocks.Checked = _editorItem.IgnoreMapBlocks;
                chkIgnoreActiveResources.Checked = _editorItem.IgnoreActiveResources;
                chkIgnoreInactiveResources.Checked = _editorItem.IgnoreExhaustedResources;
                chkIgnoreZDimensionBlocks.Checked = _editorItem.IgnoreZDimension;
                chkHoming.Checked = _editorItem.Homing;
                chkGrapple.Checked = _editorItem.GrappleHook;

                if (scrlAnimation.Value == -1)
                {
                    lblAnimation.Text = "Animation: None";
                }
                else
                {
                    lblAnimation.Text = "Animation: " +
                                        AnimationBase.GetName(Database.GameObjectListIndex(GameObject.Animation,
                                            scrlAnimation.Value));
                }
                if (scrlSpell.Value == -1)
                {
                    lblSpell.Text = "Collision Spell: None";
                }
                else
                {
                    lblSpell.Text = "Collision Spell: " + SpellBase.GetName(_editorItem.Spell);
                }
                lblSpeed.Text = "Speed: " + scrlSpeed.Value + "ms";
                lblSpawn.Text = "Spawn Delay: " + scrlSpawn.Value + "ms";
                lblAmount.Text = "Quantity: " + scrlAmount.Value;
                lblRange.Text = "Range: " + scrlRange.Value;
                lblKnockback.Text = "Knockback: " + scrlKnockback.Value;

                if(lstAnimations.SelectedIndex < 0) { lstAnimations.SelectedIndex = 0; }
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
        }

        private void updateAnimationData(int index)
        {
            scrlAnimation.Value = Database.GameObjectListIndex(GameObject.Animation,_editorItem.Animations[index].Animation);
            scrlSpawnRange.Value = _editorItem.Animations[index].SpawnRange;
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
            if (scrlAmount.Value < scrlSpawnRange.Value) { scrlSpawnRange.Value = scrlAmount.Value; }
            scrlSpawnRange.Maximum = scrlAmount.Value;

            //Save the index
            if (SaveIndex == true) { selectedIndex = lstAnimations.SelectedIndex; }

            // Add the animations to the list
            lstAnimations.Items.Clear();
            for (int i = 0; i < _editorItem.Animations.Count; i++)
            {
                if (_editorItem.Animations[i].Animation != -1)
                {
                    lstAnimations.Items.Add("[Spawn Range: " + n + " - " + _editorItem.Animations[i].SpawnRange +
                        "] Animation: " + AnimationBase.GetName(_editorItem.Animations[i].Animation));
                }
                else
                {
                    lstAnimations.Items.Add("[Spawn Range: " + n + " - " + _editorItem.Animations[i].SpawnRange + "] Animation: None");
                }
                n = _editorItem.Animations[i].SpawnRange + 1;
            }
            lstAnimations.SelectedIndex = selectedIndex;
            if (lstAnimations.SelectedIndex < 0) { lstAnimations.SelectedIndex = 0; }

            if (scrlAnimation.Value == -1)
            {
                lblAnimation.Text = "Animation: None";
            }
            else
            {
                lblAnimation.Text = "Animation: " +
                                    AnimationBase.GetName(Database.GameObjectIdFromList(GameObject.Animation,
                                        scrlAnimation.Value));
            }

            if (lstAnimations.SelectedIndex > 0)
            {
                lblSpawnRange.Text = "Spawn Range: " + (_editorItem.Animations[lstAnimations.SelectedIndex - 1].SpawnRange + 1) +
                    " - " + _editorItem.Animations[lstAnimations.SelectedIndex].SpawnRange;
            }
            else
            {
                lblSpawnRange.Text = "Spawn Range: 1 - " + _editorItem.Animations[lstAnimations.SelectedIndex].SpawnRange;
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
                img = (Bitmap)picSpawns.BackgroundImage;
            }
            var gfx = System.Drawing.Graphics.FromImage(img);
            gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, picSpawns.Width, picSpawns.Height));

            for (var x = 0; x < ProjectileBase.SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < ProjectileBase.SpawnLocationsHeight; y++)
                {
                    gfx.DrawImage(_directionGrid, new Rectangle(x * Options.TileWidth, y * Options.TileHeight, Options.TileWidth, Options.TileHeight), new Rectangle(0, 0, Options.TileWidth, Options.TileHeight), GraphicsUnit.Pixel);
                    for (var i = 0; i < ProjectileBase.MaxProjectileDirections; i++)
                    {
                        if (_editorItem.SpawnLocations[x, y].Directions[i] == true)
                        {
                            gfx.DrawImage(_directionGrid, new Rectangle((x * Options.TileWidth) + DirectionOffsetX(i), (y * Options.TileHeight) + DirectionOffsetY(i), (Options.TileWidth - 2) / 3, (Options.TileHeight - 2) / 3), new Rectangle(Options.TileWidth + DirectionOffsetX(i), DirectionOffsetY(i), (Options.TileWidth - 2) / 3, (Options.TileHeight - 2) / 3), GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            gfx.DrawImage(_directionGrid, new Rectangle((picSpawns.Width / 2) - (((Options.TileHeight - 2) / 3) / 2), (picSpawns.Height / 2) - (((Options.TileHeight - 2) / 3) / 2), (Options.TileWidth - 2) / 3, (Options.TileHeight - 2) / 3), new Rectangle(43, 11, (Options.TileWidth - 2) / 3, (Options.TileHeight - 2) / 3), GraphicsUnit.Pixel);
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
            lstProjectiles.Items[Database.GameObjectListIndex(GameObject.Projectile,_editorItem.GetId())] = txtName.Text;
        }

        private void scrlAnimation_Scroll(object sender, ScrollEventArgs e)
        {
            _editorItem.Animations[lstAnimations.SelectedIndex].Animation = Database.GameObjectIdFromList(GameObject.Animation, scrlAnimation.Value);
            updateAnimations();
        }

        private void scrlSpeed_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpeed.Text = "Speed: " + scrlSpeed.Value + "ms";
            _editorItem.Speed = scrlSpeed.Value;
        }

        private void scrlSpawn_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawn.Text = "Spawn Delay: " + scrlSpawn.Value + "ms";
            _editorItem.Delay = scrlSpawn.Value;
        }

        private void scrlRange_Scroll(object sender, ScrollEventArgs e)
        {
            lblRange.Text = "Range: " + scrlRange.Value;
            _editorItem.Range = scrlRange.Value;
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
            double x = e.X / Options.TileWidth;
            double y = e.Y / Options.TileHeight;
            double i, j;

            x = Math.Floor(x);
            y = Math.Floor(y);

            i = (e.X - (x * Options.TileWidth)) / (Options.TileWidth / 3);
            j = (e.Y - (y * Options.TileHeight)) / (Options.TileWidth / 3);

            i = Math.Floor(i);
            j = Math.Floor(j);

            _editorItem.SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)] = !_editorItem.SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)];

            Render();
        }

        private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlSpell.Value == -1)
            {
                _editorItem.Spell = -1;
                lblSpell.Text = "Collision Spell: None";
            }
            else
            {
                _editorItem.Spell = Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value);
                lblSpell.Text = "Collision Spell: " + SpellBase.GetName(_editorItem.Spell);
            }
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

        private void scrlSpawnRange_Scroll(object sender, ScrollEventArgs e)
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

        private void scrlAmount_Scroll(object sender, ScrollEventArgs e)
        {
            lblAmount.Text = "Quantity: " + scrlAmount.Value;
            _editorItem.Quantity = scrlAmount.Value;
            updateAnimations();
        }

        private void scrlKnockback_Scroll(object sender, ScrollEventArgs e)
        {
            lblKnockback.Text = "Knockback: " + scrlKnockback.Value;
            _editorItem.Knockback = scrlKnockback.Value;
        }
    }
}
