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

        public FrmItem()
        {
            InitializeComponent();
            PacketHandler.GameObjectUpdatedDelegate += GameObjectUpdatedDelegate;
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObject.Item);
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
            cmbClass.Items.AddRange(Database.GetGameObjectList(GameObject.Class));

            scrlProjectile.Maximum = ProjectileBase.ObjectCount()-1;
            scrlSpell.Maximum = SpellBase.ObjectCount() - 1;
            scrlAnim.Maximum = AnimationBase.ObjectCount()-1;
            cmbPaperdoll.Items.Clear();
            cmbPaperdoll.Items.Add("None");
            string[] paperdollnames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Paperdoll);
            for (i = 0; i < paperdollnames.Length; i++)
            {
                cmbPaperdoll.Items.Add(paperdollnames[i]);
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
            scrlProjectile.Maximum = ProjectileBase.ObjectCount();
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
                cmbClass.SelectedIndex = _editorItem.ClassReq;
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
                scrlRange.Value = _editorItem.StatGrowth;
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbToolType.SelectedIndex = _editorItem.Tool;
                if (_editorItem.ItemType == (int)ItemTypes.Equipment) cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
                scrlEffectAmount.Value = _editorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(_editorItem.Data4);
                cmbPaperdoll.SelectedIndex = cmbPaperdoll.FindString(_editorItem.Paperdoll);
                if (cmbPic.SelectedIndex > 0)
                {
                    picItem.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/items/" + cmbPic.Text);
                }
                else
                {
                    picItem.BackgroundImage = null;
                }
                if (cmbPaperdoll.SelectedIndex > 0)
                {
                    picPaperdoll.BackgroundImage =
                        System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbPaperdoll.Text);
                }
                else
                {
                    picPaperdoll.BackgroundImage = null;
                }

                //External References
                scrlProjectile.Value = Database.GameObjectListIndex(GameObject.Projectile, _editorItem.Projectile);
                scrlProjectile_ValueChanged(null, null);
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
                _editorItem.Tool = 0;
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
            _editorItem.ClassReq = cmbClass.SelectedIndex;
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

        private void scrlDmg_Scroll(object sender, EventArgs e)
        {
            lblDamage.Text = @"Damage: " + scrlDamage.Value;
            _editorItem.Damage = scrlDamage.Value;
        }

        private void scrlRange_Scroll(object sender, EventArgs e)
        {
            lblRange.Text = @"Stat Bonus Range: +- " + scrlRange.Value;
            _editorItem.StatGrowth = scrlRange.Value;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Paperdoll = cmbPaperdoll.Text;
            if (cmbPaperdoll.SelectedIndex > 0) { picPaperdoll.BackgroundImage = System.Drawing.Bitmap.FromFile("resources/paperdolls/" + cmbPaperdoll.Text); }
            else { picPaperdoll.BackgroundImage = null; }
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
                chk2Hand.Visible = true;
                cmbToolType.Visible = true;
                lblToolType.Visible = true;
                lblDamage.Visible = true;
                scrlDamage.Visible = true;
                lblProjectile.Visible = true;
                scrlProjectile.Visible = true;
            }
            else
            {
                chk2Hand.Visible = false;
                cmbToolType.Visible = false;
                lblToolType.Visible = false;
                lblDamage.Visible = false;
                scrlDamage.Visible = false;
                lblProjectile.Visible = false;
                scrlProjectile.Visible = false;

                _editorItem.Projectile = -1;
                _editorItem.Tool = -1;
                _editorItem.Damage = 0;
                _editorItem.Data4 = 0;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Tool = cmbToolType.SelectedIndex;
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

        private void scrlProjectile_ValueChanged(object sender, EventArgs e)
        {
            if (scrlProjectile.Value > -1)
            {
                _editorItem.Projectile = Database.GameObjectIdFromList(GameObject.Projectile,scrlProjectile.Value);
                lblProjectile.Text = "Projectile: " + ProjectileBase.GetName(_editorItem.Projectile);
            }
            else
            {
                _editorItem.Projectile = -1;
                lblProjectile.Text = "Projectile: None";
            }
        }

        private void scrlEvent_Scroll(object sender, ScrollEventArgs e)
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
    }
}
