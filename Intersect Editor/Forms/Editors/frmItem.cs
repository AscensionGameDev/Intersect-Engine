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
using System.Windows.Forms;
using System.Drawing;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class FrmItem : Form
    {
        private ByteBuffer[] _itemsBackup;
        private bool[] _changed;
        private int _editorIndex;

        public FrmItem()
        {
            InitializeComponent();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            int i = 0;
            lstItems.SelectedIndex = 0;
            cmbPic.Items.Clear();
            cmbPic.Items.Add("None");

            for (i= 0; i < Intersect_Editor.Classes.Graphics.ItemNames.Length; i++)
            {
                cmbPic.Items.Add(Intersect_Editor.Classes.Graphics.ItemNames[i]);
            }

            i = 0;

            while (Globals.GameClasses[i].Name != "")
            {
                cmbClass.Items.Add(Globals.GameClasses[i].Name);
                i = i + 1;
                if (i >= Constants.MaxClasses)
                {
                    break;
                }
            }

            scrlProjectile.Maximum = Constants.MaxProjectiles;
            UpdateEditor();
        }

        public void InitEditor()
        {
            _itemsBackup = new ByteBuffer[Constants.MaxItems];
            _changed = new bool[Constants.MaxItems];
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                _itemsBackup[i] = new ByteBuffer();
                _itemsBackup[i].WriteBytes(Globals.GameItems[i].ItemData());
                lstItems.Items.Add((i + 1) + ") " + Globals.GameItems[i].Name);
                _changed[i] = false;
            }
            cmbEquipmentSlot.Items.AddRange(Enums.EquipmentSlots.ToArray());
            cmbToolType.Items.Add("None");
            cmbToolType.Items.AddRange(Enums.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Add("None");
            cmbEquipmentBonus.Items.AddRange(Enums.ItemBonusEffects.ToArray());
        }

        private void UpdateEditor()
        {
            _editorIndex = lstItems.SelectedIndex;

            txtName.Text = Globals.GameItems[_editorIndex].Name;
            txtDesc.Text = Globals.GameItems[_editorIndex].Desc;
            cmbType.SelectedIndex = Globals.GameItems[_editorIndex].Type;
            cmbPic.SelectedIndex = cmbPic.FindString(Globals.GameItems[_editorIndex].Pic);
            scrlPrice.Value = Globals.GameItems[_editorIndex].Price;
            scrlAnim.Value = Globals.GameItems[_editorIndex].Animation;
            scrlLevelReq.Value = Globals.GameItems[_editorIndex].LevelReq;
            cmbClass.SelectedIndex = Globals.GameItems[_editorIndex].ClassReq;
            scrlAttackReq.Value = Globals.GameItems[_editorIndex].StatsReq[0];
            scrlAbilityPowerReq.Value = Globals.GameItems[_editorIndex].StatsReq[1];
            scrlDefenseReq.Value = Globals.GameItems[_editorIndex].StatsReq[2];
            scrlMagicResistReq.Value = Globals.GameItems[_editorIndex].StatsReq[3];
            scrlSpeedReq.Value = Globals.GameItems[_editorIndex].StatsReq[4];
            scrlAttack.Value = Globals.GameItems[_editorIndex].StatsGiven[0];
            scrlAbilityPower.Value = Globals.GameItems[_editorIndex].StatsGiven[1];
            scrlDefense.Value = Globals.GameItems[_editorIndex].StatsGiven[2];
            scrlMagicResist.Value = Globals.GameItems[_editorIndex].StatsGiven[3];
            scrlSpeed.Value = Globals.GameItems[_editorIndex].StatsGiven[4];
            scrlDamage.Value = Globals.GameItems[_editorIndex].Damage;
            scrlRange.Value = Globals.GameItems[_editorIndex].StatGrowth;
            cmbEquipmentSlot.SelectedIndex = Globals.GameItems[_editorIndex].Data1;
            cmbToolType.SelectedIndex = Globals.GameItems[_editorIndex].Tool;
            cmbEquipmentBonus.SelectedIndex = Globals.GameItems[_editorIndex].Data2;
            scrlEffectAmount.Value = Globals.GameItems[_editorIndex].Data3;
            chk2Hand.Checked = Convert.ToBoolean(Globals.GameItems[_editorIndex].Data4);
            cmbPaperdoll.SelectedIndex = cmbPaperdoll.FindString(Globals.GameItems[_editorIndex].Paperdoll);
            scrlProjectile.Value = Globals.GameItems[_editorIndex].Projectile;
            if (cmbPic.SelectedIndex > 0) { picItem.BackgroundImage = Bitmap.FromFile("Resources/Items/" + cmbPic.Text); }
            else { picItem.BackgroundImage = null; }
            _changed[_editorIndex] = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                if (_changed[i])
                {
                    PacketSender.SendItem(i, Globals.GameItems[i].ItemData());
                }
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void scrlInterval_Scroll(object sender, EventArgs e)
        {
            lblInterval.Text = @"Interval: " + scrlInterval.Value;
            Globals.GameItems[_editorIndex].Data2 = scrlInterval.Value;
        }

        private void lstItems_Click(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        private void scrlLevel_Scroll(object sender, EventArgs e)
        {
            lblLevelReq.Text = @"Level: " + scrlLevelReq.Value;
            Globals.GameItems[_editorIndex].LevelReq = scrlLevelReq.Value;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbConsumable.Visible = false;
            gbSpell.Visible = false;
            gbEquipment.Visible = false;

            if (Globals.GameItems[_editorIndex].Type != cmbType.SelectedIndex)
            {
                Globals.GameItems[_editorIndex].Damage = 0;
                Globals.GameItems[_editorIndex].Tool = 0;
                Globals.GameItems[_editorIndex].Data1 = 0;
                Globals.GameItems[_editorIndex].Data2 = 0;
                Globals.GameItems[_editorIndex].Data3 = 0;
                Globals.GameItems[_editorIndex].Data4 = 0;
            }

            if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Consumable"))
            {
                cmbConsume.SelectedIndex = Globals.GameItems[_editorIndex].Data1;
                scrlInterval.Value = Globals.GameItems[_editorIndex].Data2;
                gbConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Spell"))
            {
                scrlSpell.Value = Globals.GameItems[_editorIndex].Data1;
                gbSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Equipment"))
            {
                gbEquipment.Visible = true;
                cmbEquipmentSlot.SelectedIndex = Globals.GameItems[_editorIndex].Data1;
            }

            Globals.GameItems[_editorIndex].Type = cmbType.SelectedIndex;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var tempItem = new ItemStruct();
            var tempBuff = new ByteBuffer();
            tempBuff.WriteBytes(tempItem.ItemData());
            Globals.GameItems[_editorIndex].LoadItem(tempBuff);
            UpdateEditor();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
            {
                Globals.GameItems[i].LoadItem(_itemsBackup[i]);
            }

            Hide();
            Globals.CurrentEditor = -1;
            Dispose();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Name = txtName.Text;
            lstItems.Items[_editorIndex] = (_editorIndex + 1) + ") " + txtName.Text;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Pic = cmbPic.Text;
            if (cmbPic.SelectedIndex > 0) { picItem.BackgroundImage = Bitmap.FromFile("Resources/Items/" + cmbPic.Text); }
            else { picItem.BackgroundImage = null; }
        }

        private void scrlPrice_Scroll(object sender, EventArgs e)
        {
            lblPrice.Text = @"Price: " + scrlPrice.Value;
            Globals.GameItems[_editorIndex].Price = scrlPrice.Value;
        }

        private void scrlAnim_Scroll(object sender, EventArgs e)
        {
            lblAnim.Text = @"Animation: " + scrlAnim.Value + @" None";
            Globals.GameItems[_editorIndex].Animation = scrlAnim.Value;
        }

        private void scrlAttackReq_Scroll(object sender, EventArgs e)
        {
            lblAttackReq.Text = @"Attack: " + scrlAttackReq.Value;
            Globals.GameItems[_editorIndex].StatsReq[0] = scrlAttackReq.Value;
        }

        private void scrlAbilityPowerReq_Scroll(object sender, EventArgs e)
        {
            lblAbilityPowerReq.Text = @"Ability Pwr: " + scrlAbilityPowerReq.Value;
            Globals.GameItems[_editorIndex].StatsReq[1] = scrlAbilityPowerReq.Value;
        }

        private void scrlDefenseReq_Scroll(object sender, EventArgs e)
        {
            lblDefenseReq.Text = @"Defense: " + scrlDefenseReq.Value;
            Globals.GameItems[_editorIndex].StatsReq[2] = scrlDefenseReq.Value;
        }

        private void scrlMagicResistReq_Scroll(object sender, EventArgs e)
        {
            lblMagicResistReq.Text = @"Magic Resist: " + scrlMagicResistReq.Value;
            Globals.GameItems[_editorIndex].StatsReq[3] = scrlMagicResistReq.Value;
        }

        private void scrlSpeedReq_Scroll(object sender, EventArgs e)
        {
            lblSpeedReq.Text = @"Speed: " + scrlSpeedReq.Value;
            Globals.GameItems[_editorIndex].StatsReq[4] = scrlSpeedReq.Value;
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].ClassReq = cmbClass.SelectedIndex;
        }

        private void scrlSpell_Scroll(object sender, EventArgs e)
        {
            lblSpell.Text = @"Spell: " + scrlSpell.Value + Globals.GameSpells[scrlSpell.Value].Name;
            Globals.GameItems[_editorIndex].Data1 = scrlSpell.Value;
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Data1 = cmbConsume.SelectedIndex;
        }

        private void scrlAttack_Scroll(object sender, EventArgs e)
        {
            lblAttack.Text = @"Attack: " + scrlAttack.Value;
            Globals.GameItems[_editorIndex].StatsGiven[0] = scrlAttack.Value;
        }

        private void scrlAbilityPower_Scroll(object sender, EventArgs e)
        {
            lblAbilityPower.Text = @"Ability Pwr: " + scrlAbilityPower.Value;
            Globals.GameItems[_editorIndex].StatsGiven[1] = scrlAbilityPower.Value;
        }

        private void scrlDefense_Scroll(object sender, EventArgs e)
        {
            lblDefense.Text = @"Defense: " + scrlDefense.Value;
            Globals.GameItems[_editorIndex].StatsGiven[2] = scrlDefense.Value;
        }

        private void scrlMagicResist_Scroll(object sender, EventArgs e)
        {
            lblMagicResist.Text = @"Magic Resist: " + scrlMagicResist.Value;
            Globals.GameItems[_editorIndex].StatsGiven[3] = scrlMagicResist.Value;
        }

        private void scrlSpeed_Scroll(object sender, EventArgs e)
        {
            lblSpeed.Text = @"Speed: " + scrlSpeed.Value;
            Globals.GameItems[_editorIndex].StatsGiven[4] = scrlSpeed.Value;
        }

        private void scrlDmg_Scroll(object sender, EventArgs e)
        {
            lblDamage.Text = @"Damage: " + scrlDamage.Value;
            Globals.GameItems[_editorIndex].Damage = scrlDamage.Value;
        }

        private void scrlRange_Scroll(object sender, EventArgs e)
        {
            lblRange.Text = @"Stat Bonus Range: +- " + scrlRange.Value;
            Globals.GameItems[_editorIndex].StatGrowth = scrlRange.Value;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Paperdoll = cmbPaperdoll.Text;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Desc = txtDesc.Text;
        }

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Data1 = cmbEquipmentSlot.SelectedIndex;
            if (cmbEquipmentSlot.SelectedIndex == Enums.WeaponIndex)
            {
                chk2Hand.Visible = true;
                cmbToolType.Visible = true;
                lblToolType.Visible = true;
                lblDamage.Visible = true;
                scrlDamage.Visible = true;
            }
            else
            {
                chk2Hand.Visible = false;
                cmbToolType.Visible = false;
                lblToolType.Visible = false;
                lblDamage.Visible = false;
                scrlDamage.Visible = false;
                Globals.GameItems[_editorIndex].Tool = -1;
                Globals.GameItems[_editorIndex].Damage = 0;
                Globals.GameItems[_editorIndex].Data4 = 0;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Tool = cmbToolType.SelectedIndex;
        }

        private void cmbEquipmentBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Data2 = cmbEquipmentBonus.SelectedIndex;
        }

        private void chk2Hand_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Data4 = Convert.ToInt32(chk2Hand.Checked);
        }

        private void FrmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void scrlEffectAmount_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Data3 = scrlEffectAmount.Value;
            lblEffectPercent.Text = "Effect Amount: " + Globals.GameItems[_editorIndex].Data3 + "%";
        }

        private void scrlProjectile_ValueChanged(object sender, EventArgs e)
        {
            Globals.GameItems[_editorIndex].Projectile = scrlProjectile.Value;
            if (scrlProjectile.Value > 0)
            {
                lblProjectile.Text = "Projectile: " + scrlProjectile.Value + " " + Globals.GameProjectiles[scrlProjectile.Value].Name;
            }
            else
            {
                lblProjectile.Text = "Projectile: " + scrlProjectile.Value + " None";
            }
        }
    }
}
