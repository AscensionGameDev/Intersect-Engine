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
        private List<ClassBase> _changed = new List<ClassBase>();
        private ClassBase _editorItem = null;

        public frmClass()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Class)
            {
                InitEditor();
                if (_editorItem != null && !ClassBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Class);
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

        private void lstClasses_Click(object sender, EventArgs e)
        {
            _editorItem = ClassBase.GetClass(Database.GameObjectIdFromList(GameObject.Class, lstClasses.SelectedIndex));
            UpdateEditor();
        }

        private void txtHP_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtHP.Text, out x);
            _editorItem.MaxVital[(int)Vitals.Health] = x;
        }

        private void txtMana_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMana.Text, out x);
            _editorItem.MaxVital[(int)Vitals.Mana] = x;
        }

        private void scrlStr_Scroll(object sender, ScrollEventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            _editorItem.Stat[(int)Stats.Attack] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, ScrollEventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            _editorItem.Stat[(int)Stats.AbilityPower] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, ScrollEventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            _editorItem.Stat[(int)Stats.Defense] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, ScrollEventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            _editorItem.Stat[(int)Stats.MagicResist] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            _editorItem.Stat[(int)Stats.Speed] = scrlSpd.Value;
        }

        private void scrlPoints_Scroll(object sender, ScrollEventArgs e)
        {
            lblPoints.Text = @"Points: " + scrlPoints.Value;
            _editorItem.Points = scrlPoints.Value;
        }

        private void scrlDropIndex_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateDropValues();
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlDropItem.Value > -1)
            {
                _editorItem.Items[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item,scrlDropItem.Value);
                lblDropItem.Text = @"Item " + ItemBase.GetName(_editorItem.Items[scrlDropIndex.Value].ItemNum);
            }
            else
            {
                _editorItem.Items[scrlDropIndex.Value].ItemNum = -1;
                lblDropItem.Text = @"Item None";
            }
        }

        private void txtDropAmount_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtDropAmount.Text, out x);
            _editorItem.Items[scrlDropIndex.Value].Amount = x;
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value;
            lblDropIndex.Text = "Item Index: " + (index + 1);
            scrlDropItem.Value = Database.GameObjectListIndex(GameObject.Item,_editorItem.Items[index].ItemNum);
            if (scrlDropItem.Value > -1)
            {
                _editorItem.Items[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item, scrlDropItem.Value);
                lblDropItem.Text = @"Item " + ItemBase.GetName(_editorItem.Items[scrlDropIndex.Value].ItemNum);
            }
            else
            {
                _editorItem.Items[scrlDropIndex.Value].ItemNum = -1;
                lblDropItem.Text = @"Item None";
            }
            txtDropAmount.Text = _editorItem.Items[index].Amount.ToString();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstClasses.Items[Database.GameObjectListIndex(GameObject.Class,_editorItem.GetId())] = txtName.Text;
        }

        private void lstSpells_Click(object sender, EventArgs e)
        {
            if (lstSpells.Items.Count > 0)
            {
                scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell,_editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                scrlLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
                lblSpellNum.Text = @"Spell: " +
                                   SpellBase.GetName(_editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                lblLevel.Text = @"Level: " + scrlLevel.Value;
            }
        }

        private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlSpell.Value > -1)
            {
                if (lstSpells.SelectedIndex >= 0)
                {
                    _editorItem.Spells[lstSpells.SelectedIndex].SpellNum = Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value);

                   UpdateSpellList();
                }
                lblSpellNum.Text = @"Spell: " + SpellBase.GetName(Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value));
            }
            else
            {
                lblSpellNum.Text = @"Spell: None";
            }
        }

        private void UpdateSpellList(bool keepIndex = true)
        {
            // Refresh List
            int n = lstSpells.SelectedIndex;
            lstSpells.Items.Clear();
            for (int i = 0; i < _editorItem.Spells.Count; i++)
            {
                lstSpells.Items.Add(Convert.ToString(i + 1) + ") " + SpellBase.GetName(_editorItem.Spells[i].SpellNum) + " - lvl: " + _editorItem.Spells[i].Level);
            }
            if (keepIndex) lstSprites.SelectedIndex = n;
        }

        private void scrlLevel_Scroll(object sender, ScrollEventArgs e)
        {
            if (lstSpells.SelectedIndex >= 0)
            {
                _editorItem.Spells[lstSpells.SelectedIndex].Level = scrlLevel.Value;

                UpdateSpellList();
            }
            lblLevel.Text = @"Level: " + scrlLevel.Value;
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            var n = new ClassSpell();

            n.SpellNum = Database.GameObjectIdFromList(GameObject.Spell,scrlSpell.Value);
            n.Level = scrlLevel.Value;

            _editorItem.Spells.Add(n);
            UpdateSpellList(false);
        }

        private void btnRemoveSpell_Click(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex == -1) return;
            _editorItem.Spells.RemoveAt(lstSpells.SelectedIndex);
            lstSpells.Items.RemoveAt(lstSpells.SelectedIndex);

            UpdateSpellList(false);

            if (lstSpells.Items.Count > 0)
            {
                lstSpells.SelectedIndex = 0;
            }
        }


        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();
                txtName.Text = _editorItem.Name;
                scrlStr.Value = _editorItem.Stat[(int) Stats.Attack];
                scrlMag.Value = _editorItem.Stat[(int) Stats.AbilityPower];
                scrlDef.Value = _editorItem.Stat[(int) Stats.Defense];
                scrlMR.Value = _editorItem.Stat[(int) Stats.MagicResist];
                scrlSpd.Value = _editorItem.Stat[(int) Stats.Speed];
                txtHP.Text = _editorItem.MaxVital[(int) Vitals.Health].ToString();
                txtMana.Text = _editorItem.MaxVital[(int) Vitals.Mana].ToString();
                scrlPoints.Value = _editorItem.Points;

                lblStr.Text = @"Strength: " + scrlStr.Value;
                lblMag.Text = @"Magic: " + scrlMag.Value;
                lblDef.Text = @"Armor: " + scrlDef.Value;
                lblMR.Text = @"Magic Resist: " + scrlMR.Value;
                lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
                lblPoints.Text = @"Points: " + scrlPoints.Value;

                UpdateSpellList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    scrlSpell.Value = _editorItem.Spells[lstSpells.SelectedIndex].SpellNum;
                    scrlLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
                }
                else
                {
                    scrlSpell.Value = -1;
                    scrlLevel.Value = 0;
                }

                scrlSpell.Value = -1;
                lblSpellNum.Text = @"Spell: None";
                lblLevel.Text = @"Level: " + scrlLevel.Value;

                // Add the sprites
                lstSprites.Items.Clear();
                for (int i = 0; i < _editorItem.Sprites.Count; i++)
                {
                    if (_editorItem.Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " +
                                             _editorItem.Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " +
                                             _editorItem.Sprites[i].Sprite + " - F");
                    }
                }

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSprites.Items.Count > 0)
                {
                    lstSprites.SelectedIndex = 0;
                    cmbSprite.SelectedIndex =
                        cmbSprite.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Sprite);
                    if (_editorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
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
                    if (MapList.GetOrderedMaps()[i].MapNum == _editorItem.SpawnMap)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                if (cmbWarpMap.SelectedIndex == -1)
                {
                    cmbWarpMap.SelectedIndex = 0;
                    _editorItem.SpawnMap = MapList.GetOrderedMaps()[0].MapNum;
                }
                scrlX.Value = _editorItem.SpawnX;
                scrlY.Value = _editorItem.SpawnY;
                lblX.Text = @"X: " + scrlX.Value;
                lblY.Text = @"Y: " + scrlY.Value;
                cmbDirection.SelectedIndex = _editorItem.SpawnDir;
                
                UpdateDropValues();
                DrawSprite();
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

        private void frmClass_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            scrlDropItem.Maximum = ItemBase.ObjectCount() - 1;
            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
            scrlLevel.Maximum = Options.MaxLevel;
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstClasses.Items.Clear();
            lstClasses.Items.AddRange(Database.GetGameObjectList(GameObject.Class));
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].MapNum + ". " + MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
        }

        private void lstSprites_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                cmbSprite.SelectedIndex = cmbSprite.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Sprite);
                if (_editorItem.Sprites[lstSprites.SelectedIndex].Gender == 0)
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
                _editorItem.Sprites[lstSprites.SelectedIndex].Gender = 0;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < _editorItem.Sprites.Count; i++)
                {
                    if (_editorItem.Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - F");
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
                _editorItem.Sprites[lstSprites.SelectedIndex].Gender = 1;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < _editorItem.Sprites.Count; i++)
                {
                    if (_editorItem.Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - F");
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
                _editorItem.Sprites[lstSprites.SelectedIndex].Sprite = cmbSprite.Text;

                // Refresh List
                n = lstSprites.SelectedIndex;
                lstSprites.Items.Clear();
                for (int i = 0; i < _editorItem.Sprites.Count; i++)
                {
                    if (_editorItem.Sprites[i].Gender == 0)
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - M");
                    }
                    else
                    {
                        lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - F");
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

            _editorItem.Sprites.Add(n);

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
            _editorItem.Sprites.RemoveAt(lstSprites.SelectedIndex);
            lstSprites.Items.RemoveAt(lstSprites.SelectedIndex);

            // Refresh List
            lstSprites.Items.Clear();
            for (int i = 0; i < _editorItem.Sprites.Count; i++)
            {
                if (_editorItem.Sprites[i].Gender == 0)
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - M");
                }
                else
                {
                    lstSprites.Items.Add(Convert.ToString(i + 1) + ") " + _editorItem.Sprites[i].Sprite + " - F");
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
                _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                _editorItem.SpawnX = scrlX.Value;
                _editorItem.SpawnY = scrlY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
        }

        private void scrlX_Scroll(object sender, ScrollEventArgs e)
        {
            lblX.Text = @"X: " + scrlX.Value;
            _editorItem.SpawnX = scrlX.Value;
        }

        private void scrlY_Scroll(object sender, ScrollEventArgs e)
        {
            lblY.Text = @"Y: " + scrlY.Value;
            _editorItem.SpawnY = scrlY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnDir = cmbDirection.SelectedIndex;
        }
    }
}
