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
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Forms.Editors;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps.MapList;


namespace Intersect_Editor.Forms
{
    public partial class frmSpell : Form
    {
        private List<SpellBase> _changed = new List<SpellBase>();
        private SpellBase _editorItem = null;
        private byte[] _copiedItem = null;

        public frmSpell()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstSpells.LostFocus += itemList_FocusChanged;
            lstSpells.GotFocus += itemList_FocusChanged;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Spell)
            {
                InitEditor();
                if (_editorItem != null && !SpellBase.GetObjects().Values.Contains(_editorItem))
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

        private void lstSpells_Click(object sender, EventArgs e)
        {
            _editorItem = SpellBase.GetSpell(Database.GameObjectIdFromList(GameObject.Spell, lstSpells.SelectedIndex));
            UpdateEditor();
        }

        private void frmSpell_Load(object sender, EventArgs e)
        {
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObject.Projectile));
            cmbCastAnimation.Items.Clear();
            cmbCastAnimation.Items.Add("None");
            cmbCastAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbHitAnimation.Items.Clear();
            cmbHitAnimation.Items.Add("None");
            cmbHitAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add("None");
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObject.CommonEvent));
            cmbSprite.Items.Clear();
            cmbSprite.Items.Add("None");
            string[] spellNames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Spell);
            for (int i = 0; i < spellNames.Length; i++)
            {
                cmbSprite.Items.Add(spellNames[i]);
            }
            cmbTransform.Items.Clear();
            cmbTransform.Items.Add("None");
            string[] spriteNames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Entity);
            for (int i = 0; i < spriteNames.Length; i++)
            {
                cmbTransform.Items.Add(spriteNames[i]);
            }
            nudWarpX.Maximum = (int) Options.MapWidth;
            nudWarpY.Maximum = (int) Options.MapHeight;
            cmbWarpMap.Items.Clear();
            for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
            {
                cmbWarpMap.Items.Add(MapList.GetOrderedMaps()[i].Name);
            }
            cmbWarpMap.SelectedIndex = 0;
            UpdateEditor();
        }

        public void InitEditor()
        {
            lstSpells.Items.Clear();
            lstSpells.Items.AddRange(Database.GetGameObjectList(GameObject.Spell));
            cmbScalingStat.Items.Clear();
            for (int i = 0; i < Options.MaxStats; i++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(i));
            }
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                txtDesc.Text = _editorItem.Desc;
                cmbType.SelectedIndex = _editorItem.SpellType;

                scrlCastDuration.Value = _editorItem.CastDuration;
                lblCastDuration.Text = "Cast Time (secs): " + ((double) scrlCastDuration.Value/10);

                scrlCooldownDuration.Value = _editorItem.CooldownDuration;
                lblCooldownDuration.Text = "Cooldown (secs): " + ((double) scrlCooldownDuration.Value/10);

                cmbCastAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.CastAnimation) + 1;
                cmbHitAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.HitAnimation) + 1;

                cmbSprite.SelectedIndex = cmbSprite.FindString(_editorItem.Pic);
                if (cmbSprite.SelectedIndex > 0)
                {
                    picSpell.BackgroundImage = Bitmap.FromFile("resources/spells/" + cmbSprite.Text);
                }
                else
                {
                    picSpell.BackgroundImage = null;
                }
                scrlHPCost.Value = _editorItem.VitalCost[(int) Vitals.Health];
                scrlManaCost.Value = _editorItem.VitalCost[(int) Vitals.Mana];
                

                UpdateSpellTypePanels();
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

        private void UpdateSpellTypePanels()
        {
            grpTargetInfo.Hide();
            grpBuffDebuff.Hide();
            grpWarp.Hide();
            grpDash.Hide();
            grpEvent.Hide();
            cmbTargetType.Enabled = true;

            if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Combat Spell"))
            {
                grpTargetInfo.Show();
                grpBuffDebuff.Show();
                cmbTargetType.SelectedIndex = _editorItem.TargetType;
                UpdateTargetTypePanel();

                scrlHPDamage.Value = _editorItem.VitalDiff[(int) Vitals.Health];
                scrlHPDamage_Scroll(null,null);
                scrlManaDamage.Value = _editorItem.VitalDiff[(int) Vitals.Mana];
                scrlManaDamage_Scroll(null,null);
                scrlAttack.Value = _editorItem.StatDiff[(int)Stats.Attack];
                scrlAttack_Scroll(null, null);
                scrlDefense.Value = _editorItem.StatDiff[(int)Stats.Defense];
                scrlDefense_Scroll(null,null);
                scrlSpeed.Value = _editorItem.StatDiff[(int)Stats.Speed];
                scrlSpeed_Scroll(null, null);
                scrlAbilityPwr.Value = _editorItem.StatDiff[(int)Stats.AbilityPower];
                scrlAbilityPwr_Scroll(null,null);
                scrlMagicResist.Value = _editorItem.StatDiff[(int)Stats.MagicResist];
                scrlMagicResist_Scroll(null, null);

                chkFriendly.Checked = Convert.ToBoolean(_editorItem.Friendly);
                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;
                scrlScaling.Value = _editorItem.Scaling;
                scrlScaling_Scroll(null,null);
                scrlCritChance.Value = _editorItem.CritChance;
                scrlCritChance_Scroll(null,null);

                chkHOTDOT.Checked = Convert.ToBoolean(_editorItem.Data1);
                scrlBuffDuration.Value = _editorItem.Data2;
                lblBuffDuration.Text = "Duration: " + ((double)scrlBuffDuration.Value / 10) + "s";
                scrlTick.Value = Math.Max(0,_editorItem.Data4);
                lblTick.Text = "Tick: " + ((double)scrlTick.Value / 10) + "s";
                cmbExtraEffect.SelectedIndex = _editorItem.Data3;
                cmbExtraEffect_SelectedIndexChanged(null, null);

            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Warp to Map"))
            {
                grpWarp.Show();
                for (int i = 0; i < MapList.GetOrderedMaps().Count; i++)
                {
                    if (MapList.GetOrderedMaps()[i].MapNum == _editorItem.Data1)
                    {
                        cmbWarpMap.SelectedIndex = i;
                        break;
                    }
                }
                nudWarpX.Value = _editorItem.Data2;
                nudWarpY.Value = _editorItem.Data3;
                cmbDirection.SelectedIndex = _editorItem.Data4;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Warp to Target"))
            {
                grpTargetInfo.Show();
                cmbTargetType.SelectedIndex = cmbTargetType.Items.IndexOf("Single Target (includes self)");
                cmbTargetType.Enabled = false;
                UpdateTargetTypePanel();
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Dash"))
            {
                grpDash.Show();
                scrlRange.Value = _editorItem.CastRange;
                lblRange.Text = "Range: " + scrlRange.Value;
                chkIgnoreMapBlocks.Checked = Convert.ToBoolean(_editorItem.Data1);
                chkIgnoreActiveResources.Checked = Convert.ToBoolean(_editorItem.Data2);
                chkIgnoreInactiveResources.Checked = Convert.ToBoolean(_editorItem.Data3);
                chkIgnoreZDimensionBlocks.Checked = Convert.ToBoolean(_editorItem.Data4);
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Event"))
            {
                grpEvent.Show();
                cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObject.CommonEvent,_editorItem.Data1) + 1;
            }
        }

        private void UpdateTargetTypePanel()
        {
            lblHitRadius.Hide();
            scrlHitRadius.Hide();
            lblCastRange.Hide();
            scrlCastRange.Hide();
            lblProjectile.Hide();
            cmbProjectile.Hide();
            if (cmbTargetType.SelectedIndex == cmbTargetType.Items.IndexOf("Single Target (includes self)"))
            {
                lblCastRange.Show();
                scrlCastRange.Show();
                scrlCastRange.Value = _editorItem.CastRange;
                lblCastRange.Text = "Cast Range: " + scrlCastRange.Value;
                if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Combat Spell"))
                {
                    lblHitRadius.Show();
                    scrlHitRadius.Show();
                    scrlHitRadius.Value = _editorItem.HitRadius;
                    lblHitRadius.Text = "Hit Radius: " + scrlHitRadius.Value;
                }
            }
            if (cmbTargetType.SelectedIndex == cmbTargetType.Items.IndexOf("AOE") && cmbType.SelectedIndex == cmbType.Items.IndexOf("Combat Spell"))
            {
                lblHitRadius.Show();
                scrlHitRadius.Show();
                scrlHitRadius.Value = _editorItem.HitRadius;
                lblHitRadius.Text = "Hit Radius: " + scrlHitRadius.Value;
            }
                if (cmbTargetType.SelectedIndex < cmbTargetType.Items.IndexOf("Self"))
            {
                lblCastRange.Show();
                scrlCastRange.Show();
                scrlCastRange.Value = _editorItem.CastRange;
                lblCastRange.Text = "Cast Range: " + scrlCastRange.Value;
            }
            if (cmbTargetType.SelectedIndex == cmbTargetType.Items.IndexOf("Linear (projectile)"))
            {
                lblProjectile.Show();
                cmbProjectile.Show();
                cmbProjectile.SelectedIndex = Database.GameObjectListIndex(GameObject.Projectile,_editorItem.Projectile);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstSpells.Items[Database.GameObjectListIndex(GameObject.Spell,_editorItem.GetId())] = txtName.Text;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex != _editorItem.SpellType)
            {
                _editorItem.SpellType = (byte)cmbType.SelectedIndex;
                _editorItem.Data1 = 0;
                _editorItem.Data2 = 0;
                _editorItem.Data3 = 0;
                _editorItem.Data4 = 0;
                UpdateSpellTypePanels();
            }
        }

        private void cmbSprite_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Pic = cmbSprite.Text;
            if (cmbSprite.SelectedIndex > 0) { picSpell.BackgroundImage = Bitmap.FromFile("resources/spells/" + cmbSprite.Text); }
            else { picSpell.BackgroundImage = null; }
        }

        private void scrlCastDuration_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.CastDuration = scrlCastDuration.Value;
            lblCastDuration.Text = "Cast Time (secs): " + ((double)scrlCastDuration.Value / 10);
        }

        private void scrlCooldownDuration_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.CooldownDuration = scrlCooldownDuration.Value;
            lblCooldownDuration.Text = "Cooldown (secs): " + ((double)scrlCooldownDuration.Value / 10);
        }

        private void cmbTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.TargetType = cmbTargetType.SelectedIndex;
            UpdateTargetTypePanel();
        }

        private void scrlCastRange_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.CastRange = scrlCastRange.Value;
            lblCastRange.Text = "Cast Range: " + scrlCastRange.Value;
        }

        private void scrlHitRadius_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.HitRadius = scrlHitRadius.Value;
            lblHitRadius.Text = "Hit Radius: " + scrlHitRadius.Value;
        }

        private void chkHOTDOT_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Convert.ToInt32(chkHOTDOT.Checked);
        }

        private void scrlBuffDuration_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.Data2 = scrlBuffDuration.Value;
            lblBuffDuration.Text = "Duration: " + ((double)scrlBuffDuration.Value / 10) + "s";
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Desc = txtDesc.Text;
        }

        private void cmbExtraEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = cmbExtraEffect.SelectedIndex;
            
            lblSprite.Visible = false;
            cmbTransform.Visible = false;
            picSprite.Visible = false;

            if (cmbExtraEffect.SelectedIndex == cmbExtraEffect.Items.IndexOf("Transform"))
            {
                lblSprite.Visible = true;
                cmbTransform.Visible = true;
                picSprite.Visible = true;

                cmbTransform.SelectedIndex = cmbTransform.FindString(_editorItem.Data5);
                if (cmbTransform.SelectedIndex > 0)
                {
                    Bitmap bmp = new Bitmap(picSprite.Width, picSprite.Height);
                    var g = Graphics.FromImage(bmp);
                    Image src = Bitmap.FromFile("resources/entities/" + cmbTransform.Text);
                    g.DrawImage(src, new Rectangle(picSprite.Width / 2 - src.Width / 8, picSprite.Height / 2 - src.Height / 8, src.Width / 4, src.Height / 4),
                        new Rectangle(0, 0, src.Width / 4, src.Height / 4), GraphicsUnit.Pixel);
                    g.Dispose();
                    src.Dispose();
                    picSprite.BackgroundImage = bmp;
                }
                else { picSprite.BackgroundImage = null; }
            }
        }

        private void frmSpell_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void scrlRange_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblRange.Text = "Range: " + scrlRange.Value;
            _editorItem.CastRange = scrlRange.Value;
        }

        private void scrlTick_Scroll(object sender, ScrollValueEventArgs e)
        {
            _editorItem.Data4 = scrlTick.Value;
            lblTick.Text = "Tick: " + ((double)scrlTick.Value / 10) + "s";
        }

        private void chkIgnoreMapBlocks_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Convert.ToInt32(chkIgnoreMapBlocks.Checked);
        }

        private void chkIgnoreActiveResources_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data2 = Convert.ToInt32(chkIgnoreActiveResources.Checked);
        }

        private void chkIgnoreInactiveResources_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = Convert.ToInt32(chkIgnoreInactiveResources.Checked);
        }

        private void chkIgnoreZDimensionBlocks_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data4 = Convert.ToInt32(chkIgnoreZDimensionBlocks.Checked);
        }

        private void cmbTransform_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data5 = cmbTransform.Text;
            if (cmbTransform.SelectedIndex > 0)
            {
                Bitmap bmp = new Bitmap(picSprite.Width, picSprite.Height);
                var g = Graphics.FromImage(bmp);
                Image src = Bitmap.FromFile("resources/entities/" + cmbTransform.Text);
                g.DrawImage(src, new Rectangle(picSprite.Width/2 - src.Width/8, picSprite.Height/2 - src.Height/8, src.Width/4,src.Height/4),
                    new Rectangle(0, 0, src.Width/4, src.Height/4), GraphicsUnit.Pixel);
                g.Dispose();
                src.Dispose();
                picSprite.BackgroundImage = bmp;
            }
            else { picSprite.BackgroundImage = null; }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Spell);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstSpells.Focused)
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
            if (_editorItem != null && lstSpells.Focused)
            {
                _copiedItem = _editorItem.GetData();
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstSpells.Focused)
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
            toolStripItemCopy.Enabled = _editorItem != null && lstSpells.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstSpells.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstSpells.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstSpells.Focused;
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

        private void scrlHPCost_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblHPCost.Text = "HP Cost: " + scrlHPCost.Value;
            _editorItem.VitalCost[(int) Vitals.Health] = scrlHPCost.Value;
        }

        private void scrlManaCost_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblMPCost.Text = "Mana Cost: " + scrlManaCost.Value;
            _editorItem.VitalCost[(int)Vitals.Mana] = scrlManaCost.Value;
        }

        private void chkFriendly_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Friendly = Convert.ToInt32(chkFriendly.Checked);
        }

        private void scrlHPDamage_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblHPDamage.Text = "HP Damage: " + scrlHPDamage.Value;
            _editorItem.VitalDiff[(int)Vitals.Health] = scrlHPDamage.Value;
        }

        private void scrlManaDamage_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblManaDamage.Text = "Mana Damage: " + scrlManaDamage.Value;
            _editorItem.VitalDiff[(int)Vitals.Mana] = scrlManaDamage.Value;
        }

        private void scrlCritChance_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblCritChance.Text = "Crit Chance: " + scrlCritChance.Value + "%";
            _editorItem.CritChance = scrlCritChance.Value;
        }

        private void scrlAttack_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblAttack.Text = "Attack: " + scrlAttack.Value;
            _editorItem.StatDiff[(int) Stats.Attack] = scrlAttack.Value;
        }

        private void scrlDefense_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblDefense.Text = "Defense: " + scrlDefense.Value;
            _editorItem.StatDiff[(int)Stats.Defense] = scrlDefense.Value;
        }

        private void scrlSpeed_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblSpeed.Text = "Speed: " + scrlSpeed.Value;
            _editorItem.StatDiff[(int)Stats.Speed] = scrlSpeed.Value;
        }

        private void scrlAbilityPwr_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblAbilityPwr.Text = "Ability Pwr: " + scrlAbilityPwr.Value;
            _editorItem.StatDiff[(int)Stats.AbilityPower] = scrlAbilityPwr.Value;
        }

        private void scrlMagicResist_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblMagicResist.Text = "Magic Resist: " + scrlMagicResist.Value;
            _editorItem.StatDiff[(int)Stats.MagicResist] = scrlMagicResist.Value;
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

        private void btnDynamicRequirements_Click(object sender, EventArgs e)
        {
            var frm = new frmDynamicRequirements(_editorItem.CastingReqs);
            frm.ShowDialog();
        }

        private void cmbCastAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.CastAnimation = Database.GameObjectIdFromList(GameObject.Animation, cmbCastAnimation.SelectedIndex - 1);
        }

        private void cmbHitAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.HitAnimation = Database.GameObjectIdFromList(GameObject.Animation, cmbHitAnimation.SelectedIndex - 1);
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Projectile = Database.GameObjectIdFromList(GameObject.Projectile, cmbProjectile.SelectedIndex);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Database.GameObjectIdFromList(GameObject.CommonEvent, cmbEvent.SelectedIndex - 1);
        }

        private void btnVisualMapSelector_Click(object sender, EventArgs e)
        {
            frmWarpSelection frmWarpSelection = new frmWarpSelection();
            frmWarpSelection.SelectTile(MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum, (int)nudWarpX.Value, (int)nudWarpY.Value);
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
                nudWarpX.Value = frmWarpSelection.GetX();
                nudWarpY.Value = frmWarpSelection.GetY();
            }
        }

        private void cmbWarpMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWarpMap.SelectedIndex > -1 && _editorItem != null)
            {
                _editorItem.Data1 = MapList.GetOrderedMaps()[cmbWarpMap.SelectedIndex].MapNum;
            }
        }

        private void nudWarpX_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data2 = (int)nudWarpX.Value;
        }

        private void nudWarpY_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = (int) nudWarpY.Value;
        }

        private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data4 = cmbDirection.SelectedIndex;
        }
    }
}
