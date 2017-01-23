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
using System.IO;
using DarkUI.Controls;

namespace Intersect_Editor.Forms
{
    public partial class frmClass : Form
    {
        private List<ClassBase> _changed = new List<ClassBase>();
        private ClassBase _editorItem = null;
        private byte[] _copiedItem = null;

        public frmClass()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstClasses.LostFocus += itemList_FocusChanged;
            lstClasses.GotFocus += itemList_FocusChanged;
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
            _editorItem.BaseVital[(int)Vitals.Health] = x;
        }

        private void txtMana_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtMana.Text, out x);
            _editorItem.BaseVital[(int)Vitals.Mana] = x;
        }

        private void scrlStr_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            _editorItem.BaseStat[(int)Stats.Attack] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            _editorItem.BaseStat[(int)Stats.AbilityPower] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            _editorItem.BaseStat[(int)Stats.Defense] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            _editorItem.BaseStat[(int)Stats.MagicResist] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            _editorItem.BaseStat[(int)Stats.Speed] = scrlSpd.Value;
        }

        private void scrlPoints_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblPoints.Text = @"Points: " + scrlPoints.Value;
            _editorItem.BasePoints = scrlPoints.Value;
        }

        private void scrlDropIndex_Scroll(object sender, ScrollValueEventArgs e)
        {
            UpdateDropValues();
        }

        private void scrlDropItem_Scroll(object sender, ScrollValueEventArgs e)
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

        private void scrlSpell_Scroll(object sender, ScrollValueEventArgs e)
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

        private void scrlLevel_Scroll(object sender, ScrollValueEventArgs e)
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
                scrlStr.Value = _editorItem.BaseStat[(int) Stats.Attack];
                scrlMag.Value = _editorItem.BaseStat[(int) Stats.AbilityPower];
                scrlDef.Value = _editorItem.BaseStat[(int) Stats.Defense];
                scrlMR.Value = _editorItem.BaseStat[(int) Stats.MagicResist];
                scrlSpd.Value = _editorItem.BaseStat[(int) Stats.Speed];
                txtHP.Text = _editorItem.BaseVital[(int) Vitals.Health].ToString();
                txtMana.Text = _editorItem.BaseVital[(int) Vitals.Mana].ToString();
                scrlPoints.Value = _editorItem.BasePoints;
                chkLocked.Checked = Convert.ToBoolean(_editorItem.Locked);

                lblStr.Text = @"Strength: " + scrlStr.Value;
                lblMag.Text = @"Magic: " + scrlMag.Value;
                lblDef.Text = @"Armor: " + scrlDef.Value;
                lblMR.Text = @"Magic Resist: " + scrlMR.Value;
                lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
                lblPoints.Text = @"Points: " + scrlPoints.Value;

                //Combat
                scrlDamage.Value = _editorItem.Damage;
                scrlDamage_Scroll(null, null);
                scrlCritChance.Value = _editorItem.CritChance;
                scrlCritChance_Scroll(null, null);
                scrlScaling.Value = _editorItem.Scaling;
                scrlScaling_Scroll(null, null);
                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.AttackAnimation) + 1;

                //Regen
                lblHpRegen.Text = "HP Regen: " + _editorItem.VitalRegen[(int)Vitals.Health] + "%";
                lblManaRegen.Text = "Mana Regen: " + _editorItem.VitalRegen[(int)Vitals.Mana] + "%";
                scrlHpRegen.Value = _editorItem.VitalRegen[(int) Vitals.Health];
                scrlMpRegen.Value = _editorItem.VitalRegen[(int) Vitals.Mana];

                //Exp
                txtBaseExp.Text = _editorItem.BaseExp.ToString();
                scrlExpIncrease.Value = _editorItem.ExpIncrease;
                lblExpIncrease.Text = "Exp Increase (Per Lvl): " + _editorItem.ExpIncrease + "%";

                //Stat Increases
                UpdateIncreases();

                UpdateSpellList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
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
            UpdateToolStripItems();
        }

        private void frmClass_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            cmbFace.Items.Clear();
            cmbFace.Items.Add("None");
            cmbFace.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Face));
            scrlDropItem.Maximum = ItemBase.ObjectCount() - 1;
            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
            scrlLevel.Maximum = Options.MaxLevel;
            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add("None");
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstClasses.Items.Clear();
            lstClasses.Items.AddRange(Database.GetGameObjectList(GameObject.Class));
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            cmbDirection.SelectedIndex = 0;
        }

        private void lstSprites_Click(object sender, EventArgs e)
        {
            if (lstSprites.Items.Count > 0)
            {
                cmbSprite.SelectedIndex = cmbSprite.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Sprite);
                cmbFace.SelectedIndex = cmbFace.FindString(_editorItem.Sprites[lstSprites.SelectedIndex].Face);
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

            n.Sprite = "None";
            n.Face = "None";
            n.Gender = 0;

            _editorItem.Sprites.Add(n);

            if (n.Gender == 0)
            {
                lstSprites.Items.Add(Convert.ToString(_editorItem.Sprites.Count) + ") " + n.Sprite + " - M");
            }
            else
            {
                lstSprites.Items.Add(Convert.ToString(_editorItem.Sprites.Count) + ") " + n.Sprite + " - F");
            }

            lstSprites.SelectedIndex = lstSprites.Items.Count - 1;
            lstSprites_Click(null, null);
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
                if (File.Exists("resources/entities/" + cmbSprite.Text))
                {
                    var img = Bitmap.FromFile("resources/entities/" + cmbSprite.Text);
                    gfx.DrawImage(img, new Rectangle(0, 0, img.Width/4, img.Height/4),
                        new Rectangle(0, 0, img.Width/4, img.Height/4), GraphicsUnit.Pixel);
                    img.Dispose();
                }
            }
            gfx.Dispose();
            picSprite.BackgroundImage = picSpriteBmp;

            var picFaceBmp = new Bitmap(picFace.Width, picFace.Height);
            gfx = System.Drawing.Graphics.FromImage(picFaceBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picSprite.Width, picSprite.Height));
            if (cmbFace.SelectedIndex > 0)
            {
                if (File.Exists("resources/faces/" + cmbFace.Text))
                {
                    var img = Bitmap.FromFile("resources/faces/" + cmbFace.Text);
                    gfx.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height),
                        new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    img.Dispose();
                }
            }
            gfx.Dispose();
            picFace.BackgroundImage = picFaceBmp;
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

        private void scrlX_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblX.Text = @"X: " + scrlX.Value;
            _editorItem.SpawnX = scrlX.Value;
        }

        private void scrlY_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblY.Text = @"Y: " + scrlY.Value;
            _editorItem.SpawnY = scrlY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnDir = cmbDirection.SelectedIndex;
        }

        private void cmbFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;

            if (lstSprites.SelectedIndex >= 0)
            {
                _editorItem.Sprites[lstSprites.SelectedIndex].Face = cmbFace.Text;

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

        private void chkLocked_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Locked = Convert.ToInt32(chkLocked.Checked);
        }

        private void scrlHpRegen_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.VitalRegen[(int) Vitals.Health] = scrlHpRegen.Value;
            lblHpRegen.Text = "HP Regen: " + _editorItem.VitalRegen[(int) Vitals.Health] + "%";
        }

        private void scrlMpRegen_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.VitalRegen[(int)Vitals.Mana] = scrlMpRegen.Value;
            lblManaRegen.Text = "Mana Regen: " + _editorItem.VitalRegen[(int)Vitals.Mana] + "%";
        }

        private void scrlExpIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.ExpIncrease = scrlExpIncrease.Value;
            lblExpIncrease.Text = "Exp Increase (Per Lvl): " + _editorItem.ExpIncrease + "%";
        }

        private void txtBaseExp_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtBaseExp.Text, out x);
            _editorItem.BaseExp = x;
        }

        private void UpdateIncreases()
        {
            if (rdoStaticIncrease.Checked)
            {
                scrlHpIncrease.Maximum = 10000;
                scrlMpIncrease.Maximum = 10000;
                scrlStrengthIncrease.Maximum = Options.MaxStatValue;
                scrlArmorIncrease.Maximum = Options.MaxStatValue;
                scrlMagicIncrease.Maximum = Options.MaxStatValue;
                scrlMagicResistIncrease.Maximum = Options.MaxStatValue;
                scrlSpeedIncrease.Maximum = Options.MaxStatValue;
            }
            else
            {
                scrlHpIncrease.Maximum = 100;
                scrlMpIncrease.Maximum = 100;
                scrlStrengthIncrease.Maximum = 100;
                scrlArmorIncrease.Maximum = 100;
                scrlMagicIncrease.Maximum = 100;
                scrlMagicResistIncrease.Maximum = 100;
                scrlSpeedIncrease.Maximum = 100;
            }

            scrlHpIncrease.Value = Math.Min(scrlHpIncrease.Maximum,_editorItem.VitalIncrease[(int)Vitals.Health]);
            scrlMpIncrease.Value = Math.Min(scrlMpIncrease.Maximum,_editorItem.VitalIncrease[(int)Vitals.Mana]);
            lblHpIncrease.Text = "Max Hp: +" + scrlHpIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");
            lblMpIncrease.Text = "Max Mp: +" + scrlMpIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");

            scrlStrengthIncrease.Value = Math.Min(scrlStrengthIncrease.Maximum, _editorItem.StatIncrease[(int) Stats.Attack]);
            scrlArmorIncrease.Value = Math.Min(scrlArmorIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.Defense]);
            scrlMagicIncrease.Value = Math.Min(scrlMagicIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.AbilityPower]);
            scrlMagicResistIncrease.Value = Math.Min(scrlMagicResistIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.MagicResist]);
            scrlSpeedIncrease.Value = Math.Min(scrlSpeedIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.Speed]);

            lblStrengthIncrease.Text = "Strength: +" + scrlStrengthIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");
            lblArmorIncrease.Text = "Armor: +" + scrlArmorIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");
            lblMagicIncrease.Text = "Magic: +" + scrlMagicIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");
            lblMagicResistIncrease.Text = "Magic Resist: +" + scrlMagicResistIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");
            lblSpeedIncrease.Text = "Move Speed: +" + scrlSpeedIncrease.Value + (rdoPercentageIncrease.Checked ? "%" : "");

            scrlPointsIncrease.Value = _editorItem.PointIncrease;
            lblPointsIncrease.Text = "Points: +" + scrlPointsIncrease.Value;
        }

        private void rdoPercentageIncrease_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void rdoStaticIncrease_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.IncreasePercentage = Convert.ToInt32(rdoPercentageIncrease.Checked);
            UpdateIncreases();
        }

        private void scrlHpIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.VitalIncrease[(int) Vitals.Health] = scrlHpIncrease.Value;
            UpdateIncreases();
        }

        private void scrlMpIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.VitalIncrease[(int)Vitals.Mana] = scrlMpIncrease.Value;
            UpdateIncreases();
        }

        private void scrlStrengthIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Attack] = scrlStrengthIncrease.Value;
            UpdateIncreases();
        }

        private void scrlMagicIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.AbilityPower] = scrlMagicIncrease.Value;
            UpdateIncreases();
        }

        private void scrlArmorIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Defense] = scrlArmorIncrease.Value;
            UpdateIncreases();
        }

        private void scrlSpeedIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Speed] = scrlSpeedIncrease.Value;
            UpdateIncreases();
        }

        private void scrlMagicResistIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.MagicResist] = scrlMagicResistIncrease.Value;
            UpdateIncreases();
        }

        private void scrlPointsIncrease_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.PointIncrease = scrlPointsIncrease.Value;
            UpdateIncreases();
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Class);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstClasses.Focused)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete this game object? This action cannot be reverted!",
                        "Delete Object", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstClasses.Focused)
            {
                _copiedItem = _editorItem.GetData();
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstClasses.Focused)
            {
                _editorItem.Load(_copiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (_changed.Contains(_editorItem) && _editorItem != null)
            {
                if (MessageBox.Show("Are you sure you want to undo changes made to this game object? This action cannot be reverted!",
                        "Undo Changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            toolStripItemCopy.Enabled = _editorItem != null && lstClasses.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstClasses.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstClasses.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstClasses.Focused;
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

        private void scrlDamage_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblDamage.Text = "Base Damage: " + scrlDamage.Value;
            _editorItem.Damage = scrlDamage.Value;
        }

        private void scrlCritChance_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblCritChance.Text = "Crit Chance: " + scrlCritChance.Value + "%";
            _editorItem.CritChance = scrlCritChance.Value;
        }

        private void cmbAttackAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.AttackAnimation = Database.GameObjectIdFromList(GameObject.Animation, cmbAttackAnimation.SelectedIndex - 1);
        }

        private void scrlScaling_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblScaling.Text = "Scaling Amount: x" + ((double)scrlScaling.Value / 100f);
            _editorItem.Scaling = scrlScaling.Value;
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }
    }
}
