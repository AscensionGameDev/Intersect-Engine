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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Core;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;


namespace Intersect_Editor.Forms
{
    public partial class FrmItem : Form
    {
        private List<ItemBase> _changed = new List<ItemBase>();
        private ItemBase _editorItem = null;
        private byte[] _copiedItem = null;

        public FrmItem()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
            lstItems.LostFocus += itemList_FocusChanged;
            lstItems.GotFocus += itemList_FocusChanged;
        }

        private void GameObjectUpdatedDelegate(GameObject type)
        {
            if (type == GameObject.Item)
            {
                InitEditor();
                if (_editorItem != null && !ItemBase.GetObjects().Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObject.Class || type == GameObject.Projectile || type == GameObject.Animation || type == GameObject.Spell)
            {
                frmItem_Load(null,null);
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

        private void lstItems_Click(object sender, EventArgs e)
        {
            _editorItem = ItemBase.GetItem(Database.GameObjectIdFromList(GameObject.Item, lstItems.SelectedIndex));
            UpdateEditor();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            int i = 0;
            cmbPic.Items.Clear();
            cmbPic.Items.Add("None");

            string[] itemnames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            i = 0;

            cmbClass.Items.Clear();
            cmbClass.Items.Add("None");
            cmbClass.Items.AddRange(Database.GetGameObjectList(GameObject.Class));

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add("None");
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
            scrlAnim.Maximum = AnimationBase.ObjectCount()-1;
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add("None");
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add("None");
            string[] paperdollnames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Paperdoll);
            for (i = 0; i < paperdollnames.Length; i++)
            {
                cmbMalePaperdoll.Items.Add(paperdollnames[i]);
                cmbFemalePaperdoll.Items.Add(paperdollnames[i]);
            }

            UpdateEditor();
        }

        public void InitEditor()
        {
            lstItems.Items.Clear();
            lstItems.Items.AddRange(Database.GetGameObjectList(GameObject.Item));
            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add("None");
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Clear();
            cmbEquipmentBonus.Items.Add("None");
            cmbEquipmentBonus.Items.Add("Cooldown Reduction");
            cmbEquipmentBonus.Items.Add("Life Steal");
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add("None");
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObject.Projectile));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                txtDesc.Text = _editorItem.Desc;
                cmbType.SelectedIndex = _editorItem.ItemType;
                cmbPic.SelectedIndex = cmbPic.FindString(_editorItem.Pic);
                scrlPrice.Value = _editorItem.Price;
                scrlLevelReq.Value = _editorItem.LevelReq;
                cmbClass.SelectedIndex = Database.GameObjectListIndex(GameObject.Class,_editorItem.ClassReq) + 1;
                scrlAttackReq.Value = _editorItem.StatsReq[0];
                scrlAbilityPowerReq.Value = _editorItem.StatsReq[1];
                scrlDefenseReq.Value = _editorItem.StatsReq[2];
                scrlMagicResistReq.Value = _editorItem.StatsReq[3];
                scrlSpeedReq.Value = _editorItem.StatsReq[4];
                scrlAttack.Value = _editorItem.StatsGiven[0];
                scrlAbilityPower.Value = _editorItem.StatsGiven[1];
                scrlDefense.Value = _editorItem.StatsGiven[2];
                scrlMagicResist.Value = _editorItem.StatsGiven[3];
                scrlSpeed.Value = _editorItem.StatsGiven[4];
                scrlDamage.Value = _editorItem.Damage;
                scrlDamage_Scroll(null,null);
                scrlCritChance.Value = _editorItem.CritChance;
                scrlCritChance_Scroll(null,null);
                scrlScaling.Value = _editorItem.Scaling;
                scrlScaling_Scroll(null,null);
                scrlRange.Value = _editorItem.StatGrowth;
                if (_editorItem.Data1 < -1 || _editorItem.Data1 >= cmbEquipmentSlot.Items.Count)
                {
                    _editorItem.Data1 = 0;
                }
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbToolType.SelectedIndex = _editorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.AttackAnimation) + 1;
                cmbGender.SelectedIndex = _editorItem.GenderReq;
                if (_editorItem.ItemType == (int)ItemTypes.Equipment) cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
                scrlEffectAmount.Value = _editorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(_editorItem.Data4);
                cmbMalePaperdoll.SelectedIndex = cmbMalePaperdoll.FindString(_editorItem.MalePaperdoll);
                cmbFemalePaperdoll.SelectedIndex = cmbFemalePaperdoll.FindString(_editorItem.FemalePaperdoll);
                if (_editorItem.ItemType == (int)ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = _editorItem.Data1;
                    scrlInterval.Value = _editorItem.Data2;
                }
                if (cmbPic.SelectedIndex > 0)
                {
                    picItem.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/items/" + cmbPic.Text);
                }
                else
                {
                    picItem.BackgroundImage = null;
                }
                if (cmbMalePaperdoll.SelectedIndex > 0)
                {
                    picMalePaperdoll.BackgroundImage =
                        System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text);
                }
                else
                {
                    picFemalePaperdoll.BackgroundImage = null;
                }

                if (cmbFemalePaperdoll.SelectedIndex > 0)
                {
                    picFemalePaperdoll.BackgroundImage =
                        System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text);
                }
                else
                {
                    picFemalePaperdoll.BackgroundImage = null;
                }

                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;

                //External References
                cmbProjectile.SelectedIndex = Database.GameObjectListIndex(GameObject.Projectile, _editorItem.Projectile) + 1;
                scrlAnim.Value = Database.GameObjectListIndex(GameObject.Animation, _editorItem.Animation);
                scrlAnim_Scroll(null, null);

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

        private void scrlInterval_Scroll(object sender, EventArgs e)
        {
            lblInterval.Text = @"Interval: " + scrlInterval.Value;
            _editorItem.Data2 = scrlInterval.Value;
        }

        private void scrlLevel_Scroll(object sender, EventArgs e)
        {
            lblLevelReq.Text = @"Level: " + scrlLevelReq.Value;
            _editorItem.LevelReq = scrlLevelReq.Value;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbConsumable.Visible = false;
            gbSpell.Visible = false;
            gbEquipment.Visible = false;
            grpEvent.Visible = false;

            scrlSpell.Maximum = Database.GetGameObjectList(GameObject.Spell).Length - 1;
            scrlEvent.Maximum = Database.GetGameObjectList(GameObject.CommonEvent).Length - 1;

            if (_editorItem.ItemType != cmbType.SelectedIndex)
            {
                _editorItem.Damage = 0;
                _editorItem.Tool = -1;
                _editorItem.Data1 = 0;
                _editorItem.Data2 = 0;
                _editorItem.Data3 = 0;
                _editorItem.Data4 = 0;
            }

            if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Consumable"))
            {
                cmbConsume.SelectedIndex = _editorItem.Data1;
                scrlInterval.Value = _editorItem.Data2;
                gbConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Spell"))
            {
                scrlSpell.Value = Database.GameObjectListIndex(GameObject.Spell,_editorItem.Data1);
                scrlSpell_Scroll(null, null);
                gbSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Event"))
            {
                scrlEvent.Value = Database.GameObjectListIndex(GameObject.CommonEvent,_editorItem.Data1);
                scrlEvent_Scroll(null, null);
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Equipment"))
            {
                gbEquipment.Visible = true;
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
            }

            _editorItem.ItemType = cmbType.SelectedIndex;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Name = txtName.Text;
            lstItems.Items[Database.GameObjectListIndex(GameObject.Item,_editorItem.GetId())] =  txtName.Text;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Pic = cmbPic.Text;
            if (cmbPic.SelectedIndex > 0) { picItem.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/items/" + cmbPic.Text); }
            else { picItem.BackgroundImage = null; }
        }

        private void scrlPrice_Scroll(object sender, EventArgs e)
        {
            lblPrice.Text = @"Price: " + scrlPrice.Value;
            _editorItem.Price = scrlPrice.Value;
        }

        private void scrlAnim_Scroll(object sender, EventArgs e)
        {
            if (scrlAnim.Value > -1)
            {
                _editorItem.Animation = Database.GameObjectIdFromList(GameObject.Animation, scrlAnim.Value);
                lblAnim.Text = "Animation: " + AnimationBase.GetName(_editorItem.Animation);
            }
            else
            {
                _editorItem.Animation = -1;
                lblAnim.Text = "Animation: None";
            }
        }

        private void scrlAttackReq_Scroll(object sender, EventArgs e)
        {
            lblAttackReq.Text = @"Attack: " + scrlAttackReq.Value;
            _editorItem.StatsReq[0] = scrlAttackReq.Value;
        }

        private void scrlAbilityPowerReq_Scroll(object sender, EventArgs e)
        {
            lblAbilityPowerReq.Text = @"Ability Pwr: " + scrlAbilityPowerReq.Value;
            _editorItem.StatsReq[1] = scrlAbilityPowerReq.Value;
        }

        private void scrlDefenseReq_Scroll(object sender, EventArgs e)
        {
            lblDefenseReq.Text = @"Defense: " + scrlDefenseReq.Value;
            _editorItem.StatsReq[2] = scrlDefenseReq.Value;
        }

        private void scrlMagicResistReq_Scroll(object sender, EventArgs e)
        {
            lblMagicResistReq.Text = @"Magic Resist: " + scrlMagicResistReq.Value;
            _editorItem.StatsReq[3] = scrlMagicResistReq.Value;
        }

        private void scrlSpeedReq_Scroll(object sender, EventArgs e)
        {
            lblSpeedReq.Text = @"Speed: " + scrlSpeedReq.Value;
            _editorItem.StatsReq[4] = scrlSpeedReq.Value;
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ClassReq = Database.GameObjectIdFromList(GameObject.Class,cmbClass.SelectedIndex-1);
        }

        private void scrlSpell_Scroll(object sender, EventArgs e)
        {
            if (scrlSpell.Value > -1)
            {
                _editorItem.Data1 = Database.GameObjectIdFromList(GameObject.Spell, scrlSpell.Value);
                lblSpell.Text = "Spell: " + SpellBase.GetName(_editorItem.Data1);
            }
            else
            {
                _editorItem.Data1 = -1;
                lblSpell.Text = "Spell: None";
            }
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = cmbConsume.SelectedIndex;
        }

        private void scrlAttack_Scroll(object sender, EventArgs e)
        {
            lblAttack.Text = @"Attack: " + scrlAttack.Value;
            _editorItem.StatsGiven[0] = scrlAttack.Value;
        }

        private void scrlAbilityPower_Scroll(object sender, EventArgs e)
        {
            lblAbilityPower.Text = @"Ability Pwr: " + scrlAbilityPower.Value;
            _editorItem.StatsGiven[1] = scrlAbilityPower.Value;
        }

        private void scrlDefense_Scroll(object sender, EventArgs e)
        {
            lblDefense.Text = @"Defense: " + scrlDefense.Value;
            _editorItem.StatsGiven[2] = scrlDefense.Value;
        }

        private void scrlMagicResist_Scroll(object sender, EventArgs e)
        {
            lblMagicResist.Text = @"Magic Resist: " + scrlMagicResist.Value;
            _editorItem.StatsGiven[3] = scrlMagicResist.Value;
        }

        private void scrlSpeed_Scroll(object sender, EventArgs e)
        {
            lblSpeed.Text = @"Speed: " + scrlSpeed.Value;
            _editorItem.StatsGiven[4] = scrlSpeed.Value;
        }

        private void scrlRange_Scroll(object sender, EventArgs e)
        {
            lblRange.Text = @"Stat Bonus Range: +- " + scrlRange.Value;
            _editorItem.StatGrowth = scrlRange.Value;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.MalePaperdoll = cmbMalePaperdoll.Text;
            if (cmbMalePaperdoll.SelectedIndex > 0) { picMalePaperdoll.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text); }
            else { picMalePaperdoll.BackgroundImage = null; }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            _editorItem.Desc = txtDesc.Text;
        }

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = cmbEquipmentSlot.SelectedIndex;
            if (cmbEquipmentSlot.SelectedIndex == Options.WeaponIndex)
            {
                grpWeaponProperties.Show();
            }
            else
            {
                grpWeaponProperties.Hide();

                _editorItem.Projectile = -1;
                _editorItem.Tool = -1;
                _editorItem.Damage = 0;
                _editorItem.Data4 = 0;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Tool = cmbToolType.SelectedIndex - 1;
        }

        private void cmbEquipmentBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data2 = cmbEquipmentBonus.SelectedIndex;
        }

        private void chk2Hand_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Data4 = Convert.ToInt32(chk2Hand.Checked);
        }

        private void FrmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void scrlEffectAmount_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = scrlEffectAmount.Value;
            lblEffectPercent.Text = "Effect Amount: " + _editorItem.Data3 + "%";
        }

        private void scrlEvent_Scroll(object sender, ScrollValueEventArgs e)
        {
            if (scrlEvent.Value > -1)
            {
                _editorItem.Data1 = Database.GameObjectIdFromList(GameObject.CommonEvent, scrlEvent.Value);
                lblEvent.Text = "Event: " + EventBase.GetName(_editorItem.Data1);
            }
            else
            {
                _editorItem.Data1 = -1;
                lblEvent.Text = "Event: None";
            }
        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.GenderReq = cmbGender.SelectedIndex;
        }

        private void cmbFemalePaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.FemalePaperdoll = cmbFemalePaperdoll.Text;
            if (cmbFemalePaperdoll.SelectedIndex > 0) { picFemalePaperdoll.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text); }
            else { picFemalePaperdoll.BackgroundImage = null; }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Item);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstItems.Focused)
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
            if (_editorItem != null && lstItems.Focused)
            {
                _copiedItem = _editorItem.GetData();
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && _copiedItem != null && lstItems.Focused)
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
            toolStripItemCopy.Enabled = _editorItem != null && lstItems.Focused;
            toolStripItemPaste.Enabled = _editorItem != null && _copiedItem != null && lstItems.Focused;
            toolStripItemDelete.Enabled = _editorItem != null && lstItems.Focused;
            toolStripItemUndo.Enabled = _editorItem != null && lstItems.Focused;
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

        private void lblEffectPercent_Click(object sender, EventArgs e)
        {

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

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void scrlScaling_Scroll(object sender, ScrollValueEventArgs e)
        {
            lblScaling.Text = "Scaling Amount: x" + ((double) scrlScaling.Value/100f);
            _editorItem.Scaling = scrlScaling.Value;
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Projectile = Database.GameObjectIdFromList(GameObject.Projectile, cmbProjectile.SelectedIndex - 1);
        }

        private void lblCritChance_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void lblScaling_Click(object sender, EventArgs e)
        {

        }

        private void lblAttackAnimation_Click(object sender, EventArgs e)
        {

        }

        private void lblDamage_Click(object sender, EventArgs e)
        {

        }
    }
}
