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
using Intersect_Library;
using Intersect_Library.GameObjects;
using Options = Intersect_Editor.Classes.General.Options;

namespace Intersect_Editor.Classes
{
    public partial class frmProjectile : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        private ByteBuffer[] _projectilesBackup;
        private bool[] _changed;
        private int _editorIndex;

        private Bitmap _directionGrid;

        public frmProjectile()
        {
            InitializeComponent();
        }

        private void frmProjectile_Load(object sender, EventArgs e)
        {
            _directionGrid = new Bitmap("resources/misc/directions.png");
            lstProjectiles.SelectedIndex = 0;
            scrlAnimation.Maximum = Options.MaxAnimations - 1;
            UpdateEditor();
        }

        private void lstProjectiles_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        public void InitEditor()
        {
            _projectilesBackup = new ByteBuffer[Options.MaxProjectiles];
            _changed = new bool[Options.MaxProjectiles];
            for (var i = 0; i < Options.MaxProjectiles; i++)
            {
                _projectilesBackup[i] = new ByteBuffer();
                _projectilesBackup[i].WriteBytes(Globals.GameProjectiles[i].ProjectileData());
                lstProjectiles.Items.Add((i + 1) + ") " + Globals.GameProjectiles[i].Name);
                _changed[i] = false;
            }
        }

        private void UpdateEditor()
        {
            _editorIndex = lstProjectiles.SelectedIndex;
            txtName.Text = Globals.GameProjectiles[_editorIndex].Name;
            scrlSpeed.Value = Globals.GameProjectiles[_editorIndex].Speed;
            scrlSpawn.Value = Globals.GameProjectiles[_editorIndex].Delay;
            scrlAmount.Value = Globals.GameProjectiles[_editorIndex].Quantity;
            scrlRange.Value = Globals.GameProjectiles[_editorIndex].Range;
            scrlSpell.Value = Globals.GameProjectiles[_editorIndex].Spell;
            chkIgnoreMapBlocks.Checked = Globals.GameProjectiles[_editorIndex].IgnoreMapBlocks;
            chkIgnoreActiveResources.Checked = Globals.GameProjectiles[_editorIndex].IgnoreActiveResources;
            chkIgnoreInactiveResources.Checked = Globals.GameProjectiles[_editorIndex].IgnoreExhaustedResources;
            chkIgnoreZDimensionBlocks.Checked = Globals.GameProjectiles[_editorIndex].IgnoreZDimension;
            chkHoming.Checked = Globals.GameProjectiles[_editorIndex].Homing;
            chkGrapple.Checked = Globals.GameProjectiles[_editorIndex].GrappleHook;

            if (scrlAnimation.Value == -1)
            {
                lblAnimation.Text = "Animation: None";
            }
            else
            {
                lblAnimation.Text = "Animation: " + (scrlAnimation.Value + 1) + ".  " + Globals.GameAnimations[scrlAnimation.Value].Name;
            }
            if (scrlSpell.Value == 0)
            {
                lblSpell.Text = "Collision Spell: 0 None";
            }
            else
            {
                lblSpell.Text = "Collision Spell: " + scrlSpell.Value + " " + Globals.GameSpells[scrlSpell.Value - 1].Name;
            }
            lblSpeed.Text = "Speed: " + scrlSpeed.Value + "ms";
            lblSpawn.Text = "Spawn Delay: " + scrlSpawn.Value + "ms";
            lblAmount.Text = "Quantity: " + scrlAmount.Value;
            lblRange.Text = "Range: " + scrlRange.Value;

            updateAnimationData(0);
            lstAnimations.SelectedIndex = 0;
            
            Render();
            _changed[_editorIndex] = true;
        }

        private void updateAnimationData(int index)
        {
            scrlAnimation.Value = Globals.GameProjectiles[_editorIndex].Animations[index].Animation;
            scrlSpawnRange.Value = Globals.GameProjectiles[_editorIndex].Animations[index].SpawnRange;
            chkRotation.Checked = Globals.GameProjectiles[_editorIndex].Animations[index].AutoRotate;
            updateAnimations(true);
        }

        private void updateAnimations(bool SaveIndex = true)
        {
            int n = 1;
            int selectedIndex = 0;

            // if there are no animations, add one by default.
            if (Globals.GameProjectiles[_editorIndex].Animations.Count == 0)
            {
                Globals.GameProjectiles[_editorIndex].Animations.Add(new ProjectileAnimation(-1, Globals.GameProjectiles[_editorIndex].Quantity, false));
            }

            //Update the spawn range maximum
            if (scrlAmount.Value < scrlSpawnRange.Value) { scrlSpawnRange.Value = scrlAmount.Value; }
            scrlSpawnRange.Maximum = scrlAmount.Value;

            //Save the index
            if (SaveIndex == true) { selectedIndex = lstAnimations.SelectedIndex; }

            // Add the animations to the list
            lstAnimations.Items.Clear();
            for (int i = 0; i < Globals.GameProjectiles[_editorIndex].Animations.Count; i++)
            {
                if (Globals.GameProjectiles[_editorIndex].Animations[i].Animation != -1)
                {
                    lstAnimations.Items.Add("[Spawn Range: " + n + " - " + Globals.GameProjectiles[_editorIndex].Animations[i].SpawnRange +
                        "] Animation: " + (Globals.GameProjectiles[_editorIndex].Animations[i].Animation + 1) + ". " +
                        Globals.GameAnimations[Globals.GameProjectiles[_editorIndex].Animations[i].Animation].Name);
                }
                else
                {
                    lstAnimations.Items.Add("[Spawn Range: " + n + " - " + Globals.GameProjectiles[_editorIndex].Animations[i].SpawnRange + "] Animation: None");
                }
                n = Globals.GameProjectiles[_editorIndex].Animations[i].SpawnRange + 1;
            }
            lstAnimations.SelectedIndex = selectedIndex;
            if (lstAnimations.SelectedIndex < 0) { lstAnimations.SelectedIndex = 0; }

            if (scrlAnimation.Value == -1)
            {
                lblAnimation.Text = "Animation: None";
            }
            else
            {
                lblAnimation.Text = "Animation: " + (scrlAnimation.Value + 1) + ". " + Globals.GameAnimations[scrlAnimation.Value].Name;
            }

            if (lstAnimations.SelectedIndex > 0)
            {
                lblSpawnRange.Text = "Spawn Range: " + (Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex - 1].SpawnRange + 1) +
                    " - " + Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex].SpawnRange;
            }
            else
            {
                lblSpawnRange.Text = "Spawn Range: 1 - " + Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex].SpawnRange;
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

            for (var x = 0; x < ProjectileStruct.SpawnLocationsWidth; x++)
            {
                for (var y = 0; y < ProjectileStruct.SpawnLocationsHeight; y++)
                {
                    gfx.DrawImage(_directionGrid, new Rectangle(x * Options.TileWidth, y * Options.TileHeight, Options.TileWidth, Options.TileHeight), new Rectangle(0, 0, Options.TileWidth, Options.TileHeight), GraphicsUnit.Pixel);
                    for (var i = 0; i < ProjectileStruct.MaxProjectileDirections; i++)
                    {
                        if (Globals.GameProjectiles[_editorIndex].SpawnLocations[x, y].Directions[i] == true)
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
            Globals.GameProjectiles[_editorIndex].Name = txtName.Text;
            lstProjectiles.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void scrlAnimation_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex].Animation = scrlAnimation.Value;
            updateAnimations();
        }

        private void scrlSpeed_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpeed.Text = "Speed: " + scrlSpeed.Value + "ms";
            Globals.GameProjectiles[_editorIndex].Speed = scrlSpeed.Value;
        }

        private void scrlSpawn_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawn.Text = "Spawn Delay: " + scrlSpawn.Value + "ms";
            Globals.GameProjectiles[_editorIndex].Delay = scrlSpawn.Value;
        }

        private void scrlRange_Scroll(object sender, ScrollEventArgs e)
        {
            lblRange.Text = "Range: " + scrlRange.Value;
            Globals.GameProjectiles[_editorIndex].Range = scrlRange.Value;
        }

        private void chkHoming_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].Homing = chkHoming.Checked;
        }

        private void chkRotation_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex].AutoRotate = chkRotation.Checked;
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

            Globals.GameProjectiles[_editorIndex].SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)] = !Globals.GameProjectiles[_editorIndex].SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)];

            Render();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxProjectiles; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendProjectile(i, Globals.GameProjectiles[i].ProjectileData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempProjectile = new ProjectileStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempProjectile.ProjectileData());
            Globals.GameProjectiles[_editorIndex].Load(tempBuff.ToArray(),_editorIndex);
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxProjectiles; i++)
            {
                Globals.GameProjectiles[i].Load(_projectilesBackup[i].ToArray(),i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlSpell.Value == 0)
            {
                lblSpell.Text = "Collision Spell: 0 None";
            }
            else
            {
                lblSpell.Text = "Collision Spell: " + scrlSpell.Value + " " + Globals.GameSpells[scrlSpell.Value - 1].Name;
            }
            Globals.GameProjectiles[_editorIndex].Spell = scrlSpell.Value;
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].IgnoreMapBlocks = chkIgnoreMapBlocks.Checked;
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].IgnoreActiveResources = chkIgnoreActiveResources.Checked;
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].IgnoreExhaustedResources = chkIgnoreInactiveResources.Checked;
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].IgnoreZDimension = chkIgnoreZDimensionBlocks.Checked;
        }

        private void chkGrapple_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].GrappleHook = chkGrapple.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Clone the previous animation to save time, set the end point to always be the quantity of spawns.
            Globals.GameProjectiles[_editorIndex].Animations.Add(
                new ProjectileAnimation(Globals.GameProjectiles[_editorIndex].Animations[Globals.GameProjectiles[_editorIndex].Animations.Count - 1].Animation,
                Globals.GameProjectiles[_editorIndex].Quantity,
                Globals.GameProjectiles[_editorIndex].Animations[Globals.GameProjectiles[_editorIndex].Animations.Count - 1].AutoRotate));
            updateAnimations();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (Globals.GameProjectiles[_editorIndex].Animations.Count > 1)
            {
                Globals.GameProjectiles[_editorIndex].Animations.RemoveAt(Globals.GameProjectiles[_editorIndex].Animations.Count - 1);
                lstAnimations.SelectedIndex = 0;
                updateAnimations(false);
            }
        }

        private void scrlSpawnRange_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameProjectiles[_editorIndex].Animations[lstAnimations.SelectedIndex].SpawnRange = scrlSpawnRange.Value;
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
            Globals.GameProjectiles[_editorIndex].Quantity = scrlAmount.Value;
            updateAnimations();
        }
    }
}
