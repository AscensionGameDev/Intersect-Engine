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
using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
{
    public partial class frmNpc : Form
    {
        private ByteBuffer[] _npcsBackup;
        private bool[] _changed;
        private int _editorIndex;

        public frmNpc()
        {
            InitializeComponent();
        }

        private void frmNpc_Load(object sender, EventArgs e)
        {
            lstNpcs.SelectedIndex = 0;
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            for (int i = 0; i < Intersect_Editor.Classes.Graphics.EntityFileNames.Count; i++)
            {
                cmbSprite.Items.Add(Intersect_Editor.Classes.Graphics.EntityFileNames[i]);
            }
            scrlDropItem.Maximum = Constants.MaxItems - 1;
            UpdateEditor();
        }

        public void InitEditor()
        {
            _npcsBackup = new ByteBuffer[Constants.MaxNpcs];
            _changed = new bool[Constants.MaxNpcs];
            for (var i = 0; i < Constants.MaxNpcs; i++)
            {
                _npcsBackup[i] = new ByteBuffer();
                _npcsBackup[i].WriteBytes(Globals.GameNpcs[i].NpcData());
                lstNpcs.Items.Add((i + 1) + ") " + Globals.GameNpcs[i].Name);
                _changed[i] = false;
            }
        }

        private void UpdateEditor()
        {
            _editorIndex = lstNpcs.SelectedIndex;

            txtName.Text = Globals.GameNpcs[_editorIndex].Name;
            cmbBehavior.SelectedIndex = Globals.GameNpcs[_editorIndex].Behavior;
            cmbSprite.SelectedIndex = cmbSprite.FindString(Globals.GameNpcs[_editorIndex].Sprite);
            scrlSightRange.Value = Globals.GameNpcs[_editorIndex].SightRange;
            scrlSpawnDuration.Value = Globals.GameNpcs[_editorIndex].SpawnDuration;
            scrlStr.Value = Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Attack];
            scrlMag.Value = Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.AbilityPower];
            scrlDef.Value = Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Defense];
            scrlMR.Value = Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.MagicResist];
            scrlSpd.Value = Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Speed];
            txtHP.Text = Globals.GameNpcs[_editorIndex].MaxVital[(int)Enums.Vitals.Health].ToString();
            txtMana.Text = Globals.GameNpcs[_editorIndex].MaxVital[(int)Enums.Vitals.Mana].ToString();
            txtExp.Text = Globals.GameNpcs[_editorIndex].Experience.ToString();
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            lblSightRange.Text = @"Sight Range: " + scrlSightRange.Value;
            lblStr.Text = @"Strength: " + scrlStr.Value;
            lblMag.Text = @"Magic: " + scrlMag.Value;
            lblDef.Text = @"Armor: " + scrlDef.Value;
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            scrlDropIndex.Value = 1;
            UpdateDropValues();
            DrawNpcSprite();
            _changed[_editorIndex] = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxNpcs; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendNpc(i, Globals.GameNpcs[i].NpcData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void lstNpcs_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempItem = new NpcStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempItem.NpcData());
            Globals.GameNpcs[_editorIndex].Load(tempBuff.ToArray(),_editorIndex);
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxNpcs; i++)
            {
                Globals.GameNpcs[i].Load(_npcsBackup[i].ToArray(),i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameNpcs[_editorIndex].Name = txtName.Text;
            lstNpcs.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameNpcs[_editorIndex].Behavior = (byte)cmbBehavior.SelectedIndex;
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSprite.SelectedIndex > 0)
            {
                Globals.GameNpcs[_editorIndex].Sprite = cmbSprite.Text;
            }
            else
            {
                Globals.GameNpcs[_editorIndex].Sprite = "";
            }
            DrawNpcSprite();
        }

        private void scrlSpawnDuration_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            Globals.GameNpcs[_editorIndex].SpawnDuration = scrlSpawnDuration.Value;
        }

        private void scrlSightRange_Scroll(object sender, ScrollEventArgs e)
        {
            lblSightRange.Text = @"Sight Range: " + scrlSightRange.Value;
            Globals.GameNpcs[_editorIndex].SightRange = scrlSightRange.Value;
        }

        private void scrlStr_Scroll(object sender, EventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Attack] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, EventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.AbilityPower] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, EventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Defense] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, EventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.MagicResist] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, EventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            Globals.GameNpcs[_editorIndex].Stat[(int)Enums.Stats.Speed] = scrlSpd.Value;
        }

        private void txtHP_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtHP.Text, out x);
            Globals.GameNpcs[_editorIndex].MaxVital[(int)Enums.Vitals.Health] = x;
        }

        private void txtMana_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMana.Text, out x);
            Globals.GameNpcs[_editorIndex].MaxVital[(int)Enums.Vitals.Mana] = x;
        }

        private void txtExp_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtExp.Text, out x);
            Globals.GameNpcs[_editorIndex].Experience  = x;
        }

        private void DrawNpcSprite()
        {
            var picSpriteBmp = new Bitmap(picNpc.Width, picNpc.Height);
            var gfx = System.Drawing.Graphics.FromImage(picSpriteBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picNpc.Width, picNpc.Height));
            if (cmbSprite.SelectedIndex > 0)
            {
                var img = Bitmap.FromFile("Resources/Entities/" + cmbSprite.Text);
                gfx.DrawImage(img, new Rectangle(0, 0, img.Width / 4, img.Height / 4), new Rectangle(0, 0, img.Width / 4, img.Height / 4), GraphicsUnit.Pixel);
                img.Dispose();
            }
            gfx.Dispose();
            picNpc.BackgroundImage = picSpriteBmp;
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            Globals.GameNpcs[_editorIndex].Drops[scrlDropIndex.Value - 1].Amount = x;
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            Globals.GameNpcs[_editorIndex].Drops[scrlDropIndex.Value - 1].ItemNum = scrlDropItem.Value;
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value - 1;
            lblDropIndex.Text = "Drop: " + (index + 1);
            scrlDropItem.Value = Globals.GameNpcs[_editorIndex].Drops[index].ItemNum;
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            txtDropAmount.Text = Globals.GameNpcs[_editorIndex].Drops[index].Amount.ToString();
            scrlDropChance.Value = Globals.GameNpcs[_editorIndex].Drops[index].Chance;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlDropChance_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.GameNpcs[_editorIndex].Drops[scrlDropIndex.Value -1].Chance = scrlDropChance.Value;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlDropIndex_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateDropValues();
        }

        private void frmNpc_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }
    }
}
