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
            _directionGrid = new Bitmap(Intersect_Editor.Properties.Resources.ProjectileDirection);
            lstProjectiles.SelectedIndex = 0;
            scrlAnimation.Maximum = Constants.MaxAnimations - 1;
            UpdateEditor();
        }

        private void lstProjectiles_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        public void InitEditor()
        {
            _projectilesBackup = new ByteBuffer[Constants.MaxProjectiles];
            _changed = new bool[Constants.MaxProjectiles];
            for (var i = 0; i < Constants.MaxProjectiles; i++)
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
            scrlAnimation.Value = Globals.GameProjectiles[_editorIndex].Animation;
            scrlSpeed.Value = Globals.GameProjectiles[_editorIndex].Speed;
            scrlSpawn.Value = Globals.GameProjectiles[_editorIndex].Delay;
            scrlQuantity.Value = Globals.GameProjectiles[_editorIndex].Quantity;
            scrlRange.Value = Globals.GameProjectiles[_editorIndex].Range;
            scrlSpell.Value = Globals.GameProjectiles[_editorIndex].Spell;
            chkIgnoreMapBlocks.Checked = Globals.GameProjectiles[_editorIndex].IgnoreMapBlocks;
            chkIgnoreActiveResources.Checked = Globals.GameProjectiles[_editorIndex].IgnoreActiveResources;
            chkIgnoreInactiveResources.Checked = Globals.GameProjectiles[_editorIndex].IgnoreExhaustedResources;
            chkIgnoreZDimensionBlocks.Checked = Globals.GameProjectiles[_editorIndex].IgnoreZDimension;
            chkHoming.Checked = Globals.GameProjectiles[_editorIndex].Homing;
            chkRotation.Checked = Globals.GameProjectiles[_editorIndex].AutoRotate;

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
            lblQuantity.Text = "Quantity: " + scrlQuantity.Value;
            lblRange.Text = "Range: " + scrlRange.Value;
            
            Render();
            _changed[_editorIndex] = true;
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
                    gfx.DrawImage(_directionGrid, new Rectangle(x * Globals.TileWidth, y * Globals.TileHeight, Globals.TileWidth, Globals.TileHeight), new Rectangle(0, 0, Globals.TileWidth, Globals.TileHeight), GraphicsUnit.Pixel);
                    for (var i = 0; i < ProjectileStruct.MaxProjectileDirections; i++)
                    {
                        if (Globals.GameProjectiles[_editorIndex].SpawnLocations[x, y].Directions[i] == true)
                        {
                            gfx.DrawImage(_directionGrid, new Rectangle((x * Globals.TileWidth) + DirectionOffsetX(i), (y * Globals.TileHeight) + DirectionOffsetY(i), (Globals.TileWidth - 2) / 3, (Globals.TileHeight - 2) / 3), new Rectangle(Globals.TileWidth + DirectionOffsetX(i), DirectionOffsetY(i), (Globals.TileWidth - 2) / 3, (Globals.TileHeight - 2) / 3), GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            gfx.DrawImage(_directionGrid, new Rectangle((picSpawns.Width / 2) - (((Globals.TileHeight - 2) / 3) / 2), (picSpawns.Height / 2) - (((Globals.TileHeight - 2) / 3) / 2), (Globals.TileWidth - 2) / 3, (Globals.TileHeight - 2) / 3), new Rectangle(43, 11, (Globals.TileWidth - 2) / 3, (Globals.TileHeight - 2) / 3), GraphicsUnit.Pixel);
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
            if (scrlAnimation.Value == -1)
            {
                lblAnimation.Text = "Animation: None";
            }
            else
            {
                lblAnimation.Text = "Animation: " + (scrlAnimation.Value + 1) + ".  " + Globals.GameAnimations[scrlAnimation.Value].Name;
            }
            Globals.GameProjectiles[_editorIndex].Animation = scrlAnimation.Value;
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

        private void scrlQuantity_Scroll(object sender, ScrollEventArgs e)
        {
            lblQuantity.Text = "Quantity: " + scrlQuantity.Value;
            Globals.GameProjectiles[_editorIndex].Quantity = scrlQuantity.Value;
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
            Globals.GameProjectiles[_editorIndex].AutoRotate = chkRotation.Checked;
        }

        private void picSpawns_MouseDown(object sender, MouseEventArgs e)
        {
            double x = e.X / Globals.TileWidth;
            double y = e.Y / Globals.TileHeight;
            double i, j;

            x = Math.Floor(x);
            y = Math.Floor(y);

            i = (e.X - (x * Globals.TileWidth)) / (Globals.TileWidth / 3);
            j = (e.Y - (y * Globals.TileHeight)) / (Globals.TileWidth / 3);

            i = Math.Floor(i);
            j = Math.Floor(j);

            Globals.GameProjectiles[_editorIndex].SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)] = !Globals.GameProjectiles[_editorIndex].SpawnLocations[(int)x, (int)y].Directions[FindDirection((int)i, (int)j)];

            Render();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxProjectiles; i++)
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
            for (var i = 0; i < Constants.MaxProjectiles; i++)
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
    }
}
