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

        private void scrlDropIndex_Scroll(object sender, ScrollValueEventArgs e)
        {
            UpdateDropValues();
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
            cmbItem.SelectedIndex = Database.GameObjectListIndex(GameObject.Item,_editorItem.Items[index].ItemNum) + 1;
            txtDropAmount.Text = _editorItem.Items[index].Amount.ToString();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstClasses.Items[Database.GameObjectListIndex(GameObject.Class,_editorItem.GetId())] = txtName.Text;
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
            if (keepIndex) lstSpells.SelectedIndex = n;
        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {
            var n = new ClassSpell();

            n.SpellNum = Database.GameObjectIdFromList(GameObject.Spell,cmbSpell.SelectedIndex);
            n.Level = (int)nudLevel.Value;

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
                nudStr.Value = _editorItem.BaseStat[(int) Stats.Attack];
                nudMag.Value = _editorItem.BaseStat[(int) Stats.AbilityPower];
                nudDef.Value = _editorItem.BaseStat[(int) Stats.Defense];
                nudMR.Value = _editorItem.BaseStat[(int) Stats.MagicResist];
                nudSpd.Value = _editorItem.BaseStat[(int) Stats.Speed];
                txtHP.Text = _editorItem.BaseVital[(int) Vitals.Health].ToString();
                txtMana.Text = _editorItem.BaseVital[(int) Vitals.Mana].ToString();
                nudPoints.Value = _editorItem.BasePoints;
                chkLocked.Checked = Convert.ToBoolean(_editorItem.Locked);

                //Combat
                nudDamage.Value = _editorItem.Damage;
                nudCritChance.Value = _editorItem.CritChance;
                nudScaling.Value = _editorItem.Scaling / 100;
                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;
                cmbAttackAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.AttackAnimation) + 1;

                //Regen
                nudHPRegen.Value = _editorItem.VitalRegen[(int) Vitals.Health];
                nudMpRegen.Value = _editorItem.VitalRegen[(int) Vitals.Mana];

                //Exp
                txtBaseExp.Text = _editorItem.BaseExp.ToString();
                nudExpIncrease.Value = _editorItem.ExpIncrease;

                //Stat Increases
                UpdateIncreases();

                UpdateSpellList(false);

                // Don't select if there are no Spells, to avoid crashes.
                if (lstSpells.Items.Count > 0)
                {
                    lstSpells.SelectedIndex = 0;
                    cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                    nudLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
                }
                else
                {
                    cmbSpell.SelectedIndex = -1;
                    nudLevel.Value = 0;
                }

                cmbSpell.SelectedIndex = -1;

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
                nudX.Value = _editorItem.SpawnX;
                nudY.Value = _editorItem.SpawnY;
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
            cmbItem.Items.Clear();
            cmbItem.Items.Add("None");
            cmbItem.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
            cmbSpell.Items.Clear();
            cmbSpell.Items.AddRange(Database.GetGameObjectList(GameObject.Spell));
            nudLevel.Maximum = Options.MaxLevel;
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
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, (int)nudX.Value, (int)nudY.Value);
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
                nudX.Value = frmWarpSelection.GetX();
                nudY.Value = frmWarpSelection.GetY();
                _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
                _editorItem.SpawnX = (int)nudX.Value;
                _editorItem.SpawnY = (int)nudY.Value;
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editorItem == null) return;
            _editorItem.SpawnMap = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
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
                nudHpIncrease.Maximum = 10000;
                nudMpIncrease.Maximum = 10000;
                nudStrengthIncrease.Maximum = Options.MaxStatValue;
                nudArmorIncrease.Maximum = Options.MaxStatValue;
                nudMagicIncrease.Maximum = Options.MaxStatValue;
                nudMagicResistIncrease.Maximum = Options.MaxStatValue;
                nudSpeedIncrease.Maximum = Options.MaxStatValue;
            }
            else
            {
                nudHpIncrease.Maximum = 100;
                nudMpIncrease.Maximum = 100;
                nudStrengthIncrease.Maximum = 100;
                nudArmorIncrease.Maximum = 100;
                nudMagicIncrease.Maximum = 100;
                nudMagicResistIncrease.Maximum = 100;
                nudSpeedIncrease.Maximum = 100;
            }

            nudHpIncrease.Value = Math.Min(nudHpIncrease.Maximum,_editorItem.VitalIncrease[(int)Vitals.Health]);
            nudMpIncrease.Value = Math.Min(nudMpIncrease.Maximum,_editorItem.VitalIncrease[(int)Vitals.Mana]);
            lblHpIncrease.Text = "Max Hp (+ %):";
            lblMpIncrease.Text = "Max Mp (+ %):";

            nudStrengthIncrease.Value = Math.Min(nudStrengthIncrease.Maximum, _editorItem.StatIncrease[(int) Stats.Attack]);
            nudArmorIncrease.Value = Math.Min(nudArmorIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.Defense]);
            nudMagicIncrease.Value = Math.Min(nudMagicIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.AbilityPower]);
            nudMagicResistIncrease.Value = Math.Min(nudMagicResistIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.MagicResist]);
            nudSpeedIncrease.Value = Math.Min(nudSpeedIncrease.Maximum, _editorItem.StatIncrease[(int)Stats.Speed]);

            lblStrengthIncrease.Text = "Strength (+ %):";
            lblArmorIncrease.Text = "Armor (+ %):";
            lblMagicIncrease.Text = "Magic (+ %):";
            lblMagicResistIncrease.Text = "Magic Resist (+ %):";
            lblSpeedIncrease.Text = "Move Speed (+ %):";

            nudPointsIncrease.Value = _editorItem.PointIncrease;
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

        private void cmbAttackAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.AttackAnimation = Database.GameObjectIdFromList(GameObject.Animation, cmbAttackAnimation.SelectedIndex - 1);
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void cmbSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                _editorItem.Spells[lstSpells.SelectedIndex].SpellNum = Database.GameObjectIdFromList(GameObject.Spell,
                    cmbSpell.SelectedIndex);
                UpdateSpellList();
            }
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Items[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item, cmbItem.SelectedIndex -1);
        }

        private void lstSpells_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex > -1)
            {
                cmbSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[lstSpells.SelectedIndex].SpellNum);
                nudLevel.Value = _editorItem.Spells[lstSpells.SelectedIndex].Level;
            }
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Scaling = (int)(nudScaling.Value * 100);
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnX = (int)nudX.Value;
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.SpawnY = (int)nudY.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int)Stats.Attack] = (int)nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int)Stats.AbilityPower] = (int)nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int)Stats.Defense] = (int)nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int)Stats.MagicResist] = (int)nudMR.Value;
        }

        private void nudPoints_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BasePoints = (int)nudPoints.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.BaseStat[(int)Stats.Speed] = (int)nudSpd.Value;
        }

        private void nudLevel_ValueChanged(object sender, EventArgs e)
        {
            if (lstSpells.SelectedIndex >= 0)
            {
                _editorItem.Spells[lstSpells.SelectedIndex].Level = (int)nudLevel.Value;

                UpdateSpellList();
            }
        }

        private void nudHPRegen_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalRegen[(int)Vitals.Health] = (int)nudHPRegen.Value;
            UpdateIncreases();
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalRegen[(int)Vitals.Mana] = (int)nudMpRegen.Value;
            UpdateIncreases();
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Damage = (int)nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.CritChance = (int)nudCritChance.Value;
        }

        private void nudExpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.ExpIncrease = (int)nudExpIncrease.Value;
        }

        private void nudHpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalIncrease[(int)Vitals.Health] = (int)nudHpIncrease.Value;
        }

        private void nudMpIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.VitalIncrease[(int)Vitals.Mana] = (int)nudMpIncrease.Value;
        }

        private void nudArmorIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Defense] = (int)nudArmorIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicResistIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.MagicResist] = (int)nudMagicResistIncrease.Value;
            UpdateIncreases();
        }

        private void nudStrengthIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Attack] = (int)nudStrengthIncrease.Value;
            UpdateIncreases();
        }

        private void nudMagicIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.AbilityPower] = (int)nudMagicIncrease.Value;
            UpdateIncreases();
        }

        private void nudSpeedIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatIncrease[(int)Stats.Speed] = (int)nudSpeedIncrease.Value;
            UpdateIncreases();
        }

        private void nudPointsIncrease_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.PointIncrease = (int)nudPointsIncrease.Value;
            UpdateIncreases();
        }
    }
}
