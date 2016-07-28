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

namespace Intersect_Editor.Forms
{
    public partial class frmNpc : Form
    {
        private List<NpcBase> _changed = new List<NpcBase>();
        private NpcBase _editorItem = null;

        public frmNpc()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Npc)
            {
                InitEditor();
                if (_editorItem != null && !NpcBase.GetObjects().ContainsValue(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Npc);
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

        private void lstNpcs_Click(object sender, EventArgs e)
        {
            _editorItem = NpcBase.GetNpc(Database.GameObjectIdFromList(GameObject.Npc, lstNpcs.SelectedIndex));
            UpdateEditor();
        }

        private void frmNpc_Load(object sender, EventArgs e)
        {
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            cmbSprite.Items.AddRange(GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity));
            scrlDropItem.Maximum = ItemBase.ObjectCount() - 1;
            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstNpcs.Items.Clear();
            lstNpcs.Items.AddRange(Database.GetGameObjectList(GameObject.Npc));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                cmbBehavior.SelectedIndex = _editorItem.Behavior;
                cmbSprite.SelectedIndex = cmbSprite.FindString(_editorItem.Sprite);
                scrlSightRange.Value = _editorItem.SightRange;
                scrlSpawnDuration.Value = _editorItem.SpawnDuration;
                scrlStr.Value = _editorItem.Stat[(int) Stats.Attack];
                scrlMag.Value = _editorItem.Stat[(int) Stats.AbilityPower];
                scrlDef.Value = _editorItem.Stat[(int) Stats.Defense];
                scrlMR.Value = _editorItem.Stat[(int) Stats.MagicResist];
                scrlSpd.Value = _editorItem.Stat[(int) Stats.Speed];
                txtHP.Text = _editorItem.MaxVital[(int) Vitals.Health].ToString();
                txtMana.Text = _editorItem.MaxVital[(int) Vitals.Mana].ToString();
                txtExp.Text = _editorItem.Experience.ToString();
                lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
                lblSightRange.Text = @"Sight Range: " + scrlSightRange.Value;
                lblStr.Text = @"Strength: " + scrlStr.Value;
                lblMag.Text = @"Magic: " + scrlMag.Value;
                lblDef.Text = @"Armor: " + scrlDef.Value;
                lblMR.Text = @"Magic Resist: " + scrlMR.Value;
                lblSpd.Text = @"Move Speed: " + scrlSpd.Value;

                // Add the spells to the list
                lstSpells.Items.Clear();
                for (int i = 0; i < _editorItem.Spells.Count; i++)
                {
                    if (_editorItem.Spells[i] != -1)
                    {
                        lstSpells.Items.Add("Spell: " + (Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[i]) + 1) + " " + SpellBase.GetName(_editorItem.Spells[i]));
                    }
                    else
                    {
                        lstSpells.Items.Add("Spell: 0 None");
                    }
                }
                scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[0]);
                if (scrlSpell.Value > -1)
                {
                    lblSpell.Text = "Spell: " + (scrlSpell.Value + 1) + " " + SpellBase.GetName(Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value));
                }
                else
                {
                    lblSpell.Text = "Spell: 0 None";
                }
                lstSpells.SelectedIndex = 0;
                cmbFreq.SelectedIndex = _editorItem.SpellFrequency;

                // Add the aggro NPC's to the list
                lstAggro.Items.Clear();
                for (int i = 0; i < _editorItem.AggroList.Count; i++)
                {
                    if (_editorItem.AggroList[i] != -1)
                    {
                        lstAggro.Items.Add("NPC: " + (Database.GameObjectListIndex(GameObject.Npc, _editorItem.AggroList[i]) + 1) + " " + NpcBase.GetName(_editorItem.AggroList[i]));
                    }
                    else
                    {
                        lstAggro.Items.Add("NPC: 0 None");
                    }
                }
                scrlNPC.Value = Database.GameObjectListIndex(GameObject.Npc, _editorItem.AggroList[0]);
                if (scrlNPC.Value > -1)
                {
                    lblNPC.Text = "NPC: " + (scrlNPC.Value + 1) + " " + NpcBase.GetName(Database.GameObjectIdFromList(GameObject.Npc, scrlNPC.Value));
                }
                else
                {
                    lblNPC.Text = "NPC: 0 None";
                }
                lstAggro.SelectedIndex = 0;

                scrlDropIndex.Value = 0;
                UpdateDropValues();
                DrawNpcSprite();
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

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstNpcs.Items[Database.GameObjectListIndex(GameObject.Npc,_editorItem.GetId())] = txtName.Text;
        }

        private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Behavior = (byte)cmbBehavior.SelectedIndex;
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSprite.SelectedIndex > 0)
            {
                _editorItem.Sprite = cmbSprite.Text;
            }
            else
            {
                _editorItem.Sprite = "";
            }
            DrawNpcSprite();
        }

        private void scrlSpawnDuration_Scroll(object sender, ScrollEventArgs e)
        {
            lblSpawnDuration.Text = @"Spawn Duration: " + scrlSpawnDuration.Value;
            _editorItem.SpawnDuration = scrlSpawnDuration.Value;
        }

        private void scrlSightRange_Scroll(object sender, ScrollEventArgs e)
        {
            lblSightRange.Text = @"Sight Range: " + scrlSightRange.Value;
            _editorItem.SightRange = scrlSightRange.Value;
        }

        private void scrlStr_Scroll(object sender, EventArgs e)
        {
            lblStr.Text = @"Strength: " + scrlStr.Value;
            _editorItem.Stat[(int)Stats.Attack] = scrlStr.Value;
        }

        private void scrlMag_Scroll(object sender, EventArgs e)
        {
            lblMag.Text = @"Magic: " + scrlMag.Value;
            _editorItem.Stat[(int)Stats.AbilityPower] = scrlMag.Value;
        }

        private void scrlDef_Scroll(object sender, EventArgs e)
        {
            lblDef.Text = @"Armor: " + scrlDef.Value;
            _editorItem.Stat[(int)Stats.Defense] = scrlDef.Value;
        }

        private void scrlMR_Scroll(object sender, EventArgs e)
        {
            lblMR.Text = @"Magic Resist: " + scrlMR.Value;
            _editorItem.Stat[(int)Stats.MagicResist] = scrlMR.Value;
        }

        private void scrlSpd_Scroll(object sender, EventArgs e)
        {
            lblSpd.Text = @"Move Speed: " + scrlSpd.Value;
            _editorItem.Stat[(int)Stats.Speed] = scrlSpd.Value;
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

        private void txtExp_TextChanged(object sender, EventArgs e)
        {
            int x = 0;
            int.TryParse(txtExp.Text, out x);
            _editorItem.Experience  = x;
        }

        private void DrawNpcSprite()
        {
            var picSpriteBmp = new Bitmap(picNpc.Width, picNpc.Height);
            var gfx = System.Drawing.Graphics.FromImage(picSpriteBmp);
            gfx.FillRectangle(Brushes.Black, new Rectangle(0, 0, picNpc.Width, picNpc.Height));
            if (cmbSprite.SelectedIndex > 0)
            {
                var img = Bitmap.FromFile("resources/entities/" + cmbSprite.Text);
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
            _editorItem.Drops[scrlDropIndex.Value].Amount = x;
        }

        private void scrlDropItem_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlDropItem.Value == -1)
            {
                _editorItem.Drops[scrlDropIndex.Value].ItemNum = -1;
                lblDropItem.Text = @"Item None";
            }
            else
            {
                _editorItem.Drops[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item,scrlDropItem.Value);
                lblDropItem.Text = @"Item " + ItemBase.GetName(_editorItem.Drops[scrlDropIndex.Value].ItemNum);
            }
            
        }

        private void UpdateDropValues()
        {
            int index = scrlDropIndex.Value;
            lblDropIndex.Text = "Drop: " + (index + 1);
            scrlDropItem.Value = Database.GameObjectListIndex(GameObject.Item,_editorItem.Drops[index].ItemNum);
            if (scrlDropItem.Value == -1)
            {
                _editorItem.Drops[scrlDropIndex.Value].ItemNum = -1;
                lblDropItem.Text = @"Item None";
            }
            else
            {
                _editorItem.Drops[scrlDropIndex.Value].ItemNum = Database.GameObjectIdFromList(GameObject.Item, scrlDropItem.Value);
                lblDropItem.Text = @"Item " + ItemBase.GetName(_editorItem.Drops[scrlDropIndex.Value].ItemNum);
            }
            txtDropAmount.Text = _editorItem.Drops[index].Amount.ToString();
            scrlDropChance.Value = _editorItem.Drops[index].Chance;
            lblDropChance.Text = @"Chance (" + scrlDropChance.Value + @"/100)";
        }

        private void scrlDropChance_Scroll(object sender, ScrollEventArgs e)
        {
            _editorItem.Drops[scrlDropIndex.Value].Chance = scrlDropChance.Value;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lstSpells.Items.Add(lblSpell.Text);
            _editorItem.Spells.Add(scrlSpell.Value);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSpells.Items.Count > 1)
            {
                int i = lstSpells.SelectedIndex;
                lstSpells.Items.RemoveAt(i);
                _editorItem.Spells.RemoveAt(i);
            }
        }

        private void scrlSpell_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlSpell.Value > -1)
            {
                lblSpell.Text = "Spell: " + (scrlSpell.Value + 1) + " " + SpellBase.GetName(Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value));
            }
            else
            {
                lblSpell.Text = "Spell: 0 None";
            }
            lstSpells.Items[lstSpells.SelectedIndex] = lblSpell.Text;
            _editorItem.Spells[lstSpells.SelectedIndex] = Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value);
        }

        private void lstSpells_Click(object sender, EventArgs e)
        {
            scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell, _editorItem.Spells[lstSpells.SelectedIndex]);
            if (scrlSpell.Value > -1)
            {
                lblSpell.Text = "Spell: " + (scrlSpell.Value + 1) + " " + SpellBase.GetName(Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value));
            }
            else
            {
                lblSpell.Text = "Spell: 0 None";
            }
        }

        private void cmbFreq_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.SpellFrequency = cmbFreq.SelectedIndex;
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.NpcVsNpcEnabled = chkEnabled.Checked;
        }

        private void chkAttackAllies_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.AttackAllies = chkAttackAllies.Checked;
        }

        private void scrlNPC_Scroll(object sender, ScrollEventArgs e)
        {
            if (scrlNPC.Value > -1)
            {
                lblNPC.Text = "NPC " + scrlNPC.Value + " " + NpcBase.GetName(Database.GameObjectIdFromList(GameObject.Npc, scrlNPC.Value));
            }
            else
            {
                lblNPC.Text = "NPC 0 None";
            }
            _editorItem.AggroList[lstAggro.SelectedIndex] = Database.GameObjectIdFromList(GameObject.Npc, scrlNPC.Value);
            lstAggro.Items[lstAggro.SelectedIndex] = lblNPC.Text;
        }

        private void lstAggro_SelectedIndexChanged(object sender, EventArgs e)
        {
            scrlNPC.Value = Database.GameObjectListIndex(GameObject.Npc, _editorItem.AggroList[lstAggro.SelectedIndex]);
            if (scrlSpell.Value > -1)
            {
                lblNPC.Text = "NPC " + scrlNPC.Value + " " + NpcBase.GetName(Database.GameObjectIdFromList(GameObject.Npc, scrlNPC.Value));
            }
            else
            {
                lblNPC.Text = "NPC 0 None";
            }
        }

        private void btnAddAggro_Click(object sender, EventArgs e)
        {
            lstAggro.Items.Add(lblNPC.Text);
            _editorItem.AggroList.Add(scrlNPC.Value);
        }

        private void btnRemoveAggro_Click(object sender, EventArgs e)
        {
            if (lstAggro.Items.Count > 1)
            {
                int i = lstAggro.SelectedIndex;
                lstAggro.Items.RemoveAt(i);
                _editorItem.AggroList.RemoveAt(i);
            }
        }
    }
}
