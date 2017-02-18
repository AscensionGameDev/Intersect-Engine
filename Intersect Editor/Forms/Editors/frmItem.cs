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
using Intersect_Editor.Forms.Editors;
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
            cmbPic.Items.Clear();
            cmbPic.Items.Add("None");

            string[] itemnames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add("None");
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add("None");
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObject.Animation));
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add("None");
            cmbTeachSpell.Items.AddRange(Database.GetGameObjectList(GameObject.Spell));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add("None");
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObject.CommonEvent));
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add("None");
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add("None");
            string[] paperdollnames = GameContentManager.GetTextureNames(GameContentManager.TextureType.Paperdoll);
            for (var i = 0; i < paperdollnames.Length; i++)
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
                nudPrice.Value = _editorItem.Price;
                nudStr.Value = _editorItem.StatsGiven[0];
                nudMag.Value = _editorItem.StatsGiven[1];
                nudDef.Value = _editorItem.StatsGiven[2];
                nudMR.Value = _editorItem.StatsGiven[3];
                nudSpd.Value = _editorItem.StatsGiven[4];
                nudDamage.Value = _editorItem.Damage;
                nudCritChance.Value = _editorItem.CritChance;
                nudScaling.Value = _editorItem.Scaling;
                nudRange.Value = _editorItem.StatGrowth;
                chkBound.Checked = Convert.ToBoolean(_editorItem.Bound);
                chkStackable.Checked = Convert.ToBoolean(_editorItem.Stackable);
                if (_editorItem.Data1 < -1 || _editorItem.Data1 >= cmbEquipmentSlot.Items.Count)
                {
                    _editorItem.Data1 = 0;
                }
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbToolType.SelectedIndex = _editorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.AttackAnimation) + 1;
                if (_editorItem.ItemType == (int)ItemTypes.Equipment) cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
                nudEffectPercent.Value = _editorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(_editorItem.Data4);
                cmbMalePaperdoll.SelectedIndex = cmbMalePaperdoll.FindString(_editorItem.MalePaperdoll);
                cmbFemalePaperdoll.SelectedIndex = cmbFemalePaperdoll.FindString(_editorItem.FemalePaperdoll);
                if (_editorItem.ItemType == (int)ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = _editorItem.Data1;
                    nudInterval.Value = _editorItem.Data2;
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
                cmbAnimation.SelectedIndex = Database.GameObjectListIndex(GameObject.Animation, _editorItem.Animation) + 1;

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

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbConsumable.Visible = false;
            gbSpell.Visible = false;
            gbEquipment.Visible = false;
            grpEvent.Visible = false;
            gbBags.Visible = false;

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
                nudInterval.Value = _editorItem.Data2;
                gbConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Spell"))
            {
                cmbTeachSpell.SelectedIndex = Database.GameObjectListIndex(GameObject.Spell,_editorItem.Data1) + 1;
                gbSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Event"))
            {
                cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObject.CommonEvent,_editorItem.Data1) + 1;
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Equipment"))
            {
                gbEquipment.Visible = true;
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
            }
            else if (cmbType.SelectedIndex == cmbType.Items.IndexOf("Bag"))
            {
                if (_editorItem.Data1 < 1) { _editorItem.Data1 = 1; } //Cant have no space or negative space.
                gbBags.Visible = true;
                nudBag.Value = _editorItem.Data1;
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

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = cmbConsume.SelectedIndex;
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

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Projectile = Database.GameObjectIdFromList(GameObject.Projectile, cmbProjectile.SelectedIndex - 1);
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new frmDynamicRequirements(_editorItem.UseReqs, RequirementType.Item );
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Animation = Database.GameObjectIdFromList(GameObject.Animation, cmbAnimation.SelectedIndex -1);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Database.GameObjectIdFromList(GameObject.CommonEvent, cmbEvent.SelectedIndex - 1);
        }

        private void cmbTeachSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Database.GameObjectIdFromList(GameObject.Spell, cmbTeachSpell.SelectedIndex - 1);
        }

        private void nudPrice_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Price = (int)nudPrice.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Scaling = (int)nudScaling.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Damage = (int)nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.CritChance = (int)nudCritChance.Value;
        }

        private void nudEffectPercent_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = (int)nudEffectPercent.Value;
        }

        private void nudRange_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatGrowth = (int)nudRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[0] = (int)nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[1] = (int)nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[2] = (int)nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[3] = (int)nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[4] = (int)nudSpd.Value;
        }

        private void nudBag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = (int)nudBag.Value;
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data2 =(int)nudInterval.Value;
        }

        private void chkBound_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Bound = Convert.ToInt32(chkBound.Checked);
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            _editorItem.Stackable = Convert.ToInt32(chkStackable.Checked);
        }
    }
}
