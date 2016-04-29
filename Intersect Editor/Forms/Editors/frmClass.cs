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
using System.Drawing;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Maps.MapList;


namespace Intersect_Editor.Forms
{
    public partial class frmClass : Form
    {
        private ByteBuffer[] _classesBackup;
        private bool[] _changed;
        private int _editorIndex;

        public frmClass()
        {
            InitializeComponent();
        }

        private void txtHP_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtHP.Text, out x);
            Globals.GameClasses[_editorIndex].MaxVital[(int)Vitals.Health] = x;
        }

        private void txtMana_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMana.Text, out x);
            Globals.GameClasses[_editorIndex].MaxVital[(int)Vitals.Mana] = x;
        }

        private void scrlStr_Scroll(object sender, ScrollEventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            Globals.GameClasses[_editorIndex].Stat[(int)Stats.Attack] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, ScrollEventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            Globals.GameClasses[_editorIndex].Stat[(int)Stats.AbilityPower] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, ScrollEventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            Globals.GameClasses[_editorIndex].Stat[(int)Stats.Defense] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, ScrollEventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            Globals.GameClasses[_editorIndex].Stat[(int)Stats.MagicResist] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            Globals.GameClasses[_editorIndex].Stat[(int)Stats.Speed] = scrlSpd.Value;
        }

        private void scrlPoints_Scroll(object sender, ScrollEventArgs e)
        {
            lblPoints.Text = @"Points: " + scrlPoints.Value;
            Globals.GameClasses[_editorIndex].Points = scrlPoints.Value;
        }

        private void scrlDropIndex_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateDropValues();
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            Globals.GameClasses[_editorIndex].Items[scrlDropIndex.Value - 1].ItemNum = scrlDropItem.Value;
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            Globals.GameClasses[_editorIndex].Items[scrlDropIndex.Value - 1].Amount = x;
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value - 1;
            lblDropIndex.Text = "Item Index: " + (index + 1);
            scrlDropItem.Value = Globals.GameClasses[_editorIndex].Items[index].ItemNum;
            lblDropItem.Text = @"Item " + (scrlDropItem.Value + 1) + @" - " + Globals.GameItems[scrlDropItem.Value].Name;
            txtDropAmount.Text = Globals.GameClasses[_editorIndex].Items[index].Amount.ToString();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameClasses[_editorIndex].Name = txtName.Text;
            lstClasses.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void lstSpells_Click(object sender, EventArgs e)
        {
            if (lstSpells.Items.Count > 0)
            {
                scrlSpell.Value = Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].SpellNum;
                scrlLevel.Value = Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].Level;
                lblSpellNum.Text = @"Spell: " + (scrlSpell.Value + 1) + " " + Globals.GameSpells[scrlSpell.Value].Name;
                lblLevel.Text = @"Level: " + scrlLevel.Value;
            }
        }

        private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
        {
            int n = 0;

            if (lstSpells.SelectedIndex >= 0)
            {
                Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].SpellNum = scrlSpell.Value;

                // Refresh List
                n = lstSpells.SelectedIndex;
                lstSpells.Items.Clear();
                for (int i = 0; i < Globals.GameClasses[_editorIndex].Spells.Count; i++)
                {
                    lstSpells.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameSpells[Globals.GameClasses[Globals.CurrentMap].Spells[i].SpellNum].Name + " - lvl: " + Globals.GameClasses[Globals.CurrentMap].Spells[i].Level);
                }
                lstSprites.SelectedIndex = n;
            }
            lblSpellNum.Text = @"Spell: " + (scrlSpell.Value + 1) + " " + Globals.GameSpells[scrlSpell.Value].Name;
        }

        private void scrlLevel_Scroll(object sender, ScrollEventArgs e)
        {
            int n = 0;

            if (lstSpells.SelectedIndex >= 0)
            {
                Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].Level = scrlLevel.Value;

                // Refresh List
                n = lstSpells.SelectedIndex;
                lstSpells.Items.Clear();
                for (int i = 0; i < Globals.GameClasses[_editorIndex].Spells.Count; i++)
                {
                    lstSpells.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameSpells[Globals.GameClasses[Globals.CurrentMap].Spells[i].SpellNum].Name + " - lvl: " + Globals.GameClasses[Globals.CurrentMap].Spells[i].Level);
                }
                lstSprites.SelectedIndex = n;
            }
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            var n = new ClassSpell();

            n.SpellNum = scrlSpell.Value;
            n.Level = scrlLevel.Value;

            Globals.GameClasses[_editorIndex].Spells.Add(n);

            lstSpells.Items.Add(Convert.ToString(lstSpells.Items.Count + 1) + ") " + Globals.GameSpells[scrlSpell.Value].Name + " - Lvl: " + scrlLevel.Value);
            lstSpells.SelectedIndex = lstSpells.Items.Count - 1;
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex == -1) return;
            Globals.GameClasses[_editorIndex].Spells.RemoveAt(lstSpells.SelectedIndex);
            lstSpells.Items.RemoveAt(lstSpells.SelectedIndex);

            // Refresh List
            lstSpells.Items.Clear();
            for (int i = 0; i < Globals.GameClasses[_editorIndex].Spells.Count; i++)
            {
                lstSpells.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameSpells[Globals.GameClasses[Globals.CurrentMap].Spells[i].SpellNum].Name + " - lvl: " + Globals.GameClasses[Globals.CurrentMap].Spells[i].Level);
            }

            if (lstSpells.Items.Count > 0)
            {
                lstSpells.SelectedIndex = 0;
            }
        }

        private void lstClasses_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }


        private void UpdateEditor()
        {
            _editorIndex = lstClasses.SelectedIndex;
            txtName.Text = Globals.GameClasses[_editorIndex].Name;
            scrlStr.Value = Globals.GameClasses[_editorIndex].Stat[(int)Stats.Attack];
            scrlMag.Value = Globals.GameClasses[_editorIndex].Stat[(int)Stats.AbilityPower];
            scrlDef.Value = Globals.GameClasses[_editorIndex].Stat[(int)Stats.Defense];
            scrlMR.Value = Globals.GameClasses[_editorIndex].Stat[(int)Stats.MagicResist];
            scrlSpd.Value = Globals.GameClasses[_editorIndex].Stat[(int)Stats.Speed];
            txtHP.Text = Globals.GameClasses[_editorIndex].MaxVital[(int)Vitals.Health].ToString();
            txtMana.Text = Globals.GameClasses[_editorIndex].MaxVital[(int)Vitals.Mana].ToString();
            scrlPoints.Value = Globals.GameClasses[_editorIndex].Points;

            lblStr.Text = @"Strength: " + scrlStr.Value;
            lblMag.Text = @"Magic: " + scrlMag.Value;
            lblDef.Text = @"Armor: " + scrlDef.Value;
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            lblPoints.Text = @"Points: " + scrlPoints.Value;

            // Add the spells
            lstSpells.Items.Clear();
            for (int i = 0; i < Globals.GameClasses[_editorIndex].Spells.Count; i++)
            {
                lstSpells.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameSpells[Globals.GameClasses[_editorIndex].Spells[i].SpellNum].Name + " - lvl: " + Globals.GameClasses[_editorIndex].Spells[i].Level);
            }

            // Don't select if there are no Spells, to avoid crashes.
            if (lstSpells.Items.Count > 0)
            {
                lstSpells.SelectedIndex = 0;
                scrlSpell.Value = Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].SpellNum;
                scrlLevel.Value = Globals.GameClasses[_editorIndex].Spells[lstSpells.SelectedIndex].Level;
            }
            else
            {
                scrlSpell.Value = 0;
                scrlLevel.Value = 0;
            }

            lblSpellNum.Text = @"Strength: " + (scrlSpell.Value + 1) + " " + Globals.GameSpells[scrlSpell.Value].Name;
            lblLevel.Text = @"Level: " + scrlLevel.Value;

            // Add the sprites
            lstSprites.Items.Clear();
            for (int i = 0; i < Globals.GameClasses[_editorIndex].Sprites.Count; i++)
            {
                if (Globals.GameClasses[_editorIndex].Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - M");
                }
                else
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - F");
                }
            }

            // Don't select if there are no Spells, to avoid crashes.
            if (lstSprites.Items.Count > 0)
            {
                lstSprites.SelectedIndex = 0;
                cmbSprite.SelectedIndex = cmbSprite.FindString(Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Sprite);
                if (Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Gender == 0)
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }
            }

            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                if (MapList.GetOrderedMaps()[i].MapNum == Globals.GameClasses[_editorIndex].SpawnMap)
                {
                    cmbWarpMap.SelectedIndex = i;
                    break;
                }
            }
            if (cmbWarpMap.SelectedIndex == -1)
            {
                cmbWarpMap.SelectedIndex = 0;
                Globals.GameClasses[_editorIndex].SpawnMap = MapList.GetOrderedMaps()[0].MapNum;
            }
            scrlX.Value = Globals.GameClasses[_editorIndex].SpawnX;
            scrlY.Value = Globals.GameClasses[_editorIndex].SpawnY;
            lblX.Text = @"X: " + scrlX.Value;
            lblY.Text = @"Y: " + scrlY.Value;
            cmbDirection.SelectedIndex = Globals.GameClasses[_editorIndex].SpawnDir;

            scrlDropIndex.Value = 1;
            UpdateDropValues();
            DrawSprite();
            _changed[_editorIndex] = true;
        }

        private void frmClass_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            scrlDropItem.Maximum = Options.MaxItems - 1;
            scrlSpell.Maximum = Options.MaxSpells - 1;
            scrlLevel.Maximum = Options.MaxLevel;
            UpdateEditor();
        }

        public void InitEditor()
        {
            _classesBackup = new ByteBuffer[Options.MaxClasses];
            _changed = new bool[Options.MaxClasses];
            for (var i = 0; i < Options.MaxClasses; i++)
            {
                _classesBackup[i] = new ByteBuffer();
                _classesBackup[i].WriteBytes(Globals.GameClasses[i].ClassData());
                lstClasses.Items.Add((i + 1) + ") " + Globals.GameClasses[i].Name);
                _changed[i] = false;
            }
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].MapNum + ". " + MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
            lstClasses.SelectedIndex = 0;
        }

        private void lstSprites_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                cmbSprite.SelectedIndex = cmbSprite.FindString(Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Sprite);
                if (Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Gender == 0)
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }
            }
        }

        private void rbMale_Click(object sender, EventArgs e)
        {
            int n = 0;

            if (lstSprites.Items.Count > 0)
            {
                Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Gender = 0;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < Globals.GameClasses[_editorIndex].Sprites.Count; i++)
                {
                    if (Globals.GameClasses[_editorIndex].Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - F");
                    }
                }
                lstSprites.SelectedIndex = n;
            }
        }

        private void rbFemale_Click(object sender, EventArgs e)
        {
            int n = 0;

            if (lstSprites.Items.Count > 0)
            {
                Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Gender = 1;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < Globals.GameClasses[_editorIndex].Sprites.Count; i++)
                {
                    if (Globals.GameClasses[_editorIndex].Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - F");
                    }
                }
                lstSprites.SelectedIndex = n;
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;

            if (lstSprites.SelectedIndex >= 0)
            {
                Globals.GameClasses[_editorIndex].Sprites[lstSprites.SelectedIndex].Sprite = cmbSprite.Text;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < Globals.GameClasses[_editorIndex].Sprites.Count; i++)
                {
                    if (Globals.GameClasses[_editorIndex].Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - F");
                    }
                }
                lstSprites.SelectedIndex = n;
            }
            DrawSprite();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var n = new ClassSprite();

            n.Sprite = cmbSprite.Text;
            if (rbMale.Checked == true)
            {
                n.Gender = 0;
            }
            else
            {
                n.Gender = 1;
            }

            Globals.GameClasses[_editorIndex].Sprites.Add(n);

            if (n.Gender == 0)
            {
                lstSprites.Items.Add(Convert.ToString(lstSprites.Items.Count) + ") " + n.Sprite + " - M");
            }
            else
            {
                lstSprites.Items.Add(Convert.ToString(lstSprites.Items.Count) + ") " + n.Sprite + " - F");
            }

            lstSprites.SelectedIndex = lstSprites.Items.Count - 1;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSprites.SelectedIndex == -1) return;
            Globals.GameClasses[_editorIndex].Sprites.RemoveAt(lstSprites.SelectedIndex);
            lstSprites.Items.RemoveAt(lstSprites.SelectedIndex);

            // Refresh List
            lstSprites.Items.Clear();
            for (int i = 0; i < Globals.GameClasses[_editorIndex].Sprites.Count; i++)
            {
                if (Globals.GameClasses[_editorIndex].Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - M");
                }
                else
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameClasses[_editorIndex].Sprites[i].Sprite + " - F");
                }
            }

            if (lstSprites.Items.Count > 0)
            {
                lstSprites.SelectedIndex = 0;
            }
        }

        private void DrawSprite()
        {
            var picSpriteBmp = new Bitmap(picSprite.Width, picSprite.Height);
            var gfx = System.Drawing.Graphics.FromImage(picSpriteBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picSprite.Width, picSprite.Height));
            if (cmbSprite.SelectedIndex > 0)
            {
                var img = Bitmap.FromFile("resources/entities/" + cmbSprite.Text);
                gfx.DrawImage(img, new Rectangle(0, 0, img.Width / 4, img.Height / 4), new Rectangle(0, 0, img.Width / 4, img.Height / 4), GraphicsUnit.Pixel);
                img.Dispose();
            }
            gfx.Dispose();
            picSprite.BackgroundImage = picSpriteBmp;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxClasses; i++)
            {
                Globals.GameClasses[i].Load(_classesBackup[i].ToArray(),i);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempItem = new ClassStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempItem.ClassData());
            Globals.GameClasses[_editorIndex].Load(tempBuff.ToArray(),_editorIndex);
            tempBuff.Dispose();
            UpdateEditor();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Options.MaxClasses; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendClass(i, Globals.GameClasses[i].ClassData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, scrlX.Value, scrlY.Value);
            frmWarpSelection.ShowDialog();
            if (frmWarpSelection.GetResult())
            {
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == frmWarpSelection.GetMap())
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                scrlX.Value = frmWarpSelection.GetX();
                scrlY.Value = frmWarpSelection.GetY();
                lblX.Text = @"X: " + scrlX.Value;
                lblY.Text = @"Y: " + scrlY.Value;
                Globals.GameClasses[_editorIndex].SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                Globals.GameClasses[_editorIndex].SpawnX = scrlX.Value;
                Globals.GameClasses[_editorIndex].SpawnY = scrlY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameClasses[_editorIndex].SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
        }

        private void scrlX_Scroll(object sender, ScrollEventArgs e)
        {
            lblX.Text = @"X: " + scrlX.Value;
            Globals.GameClasses[_editorIndex].SpawnX = scrlX.Value;
        }

        private void scrlY_Scroll(object sender, ScrollEventArgs e)
        {
            lblY.Text = @"Y: " + scrlY.Value;
            Globals.GameClasses[_editorIndex].SpawnY = scrlY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameClasses[_editorIndex].SpawnDir = cmbDirection.SelectedIndex;
        }
    }
}
