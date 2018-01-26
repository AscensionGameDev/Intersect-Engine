using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms
{
    public partial class FrmItem : EditorForm
    {
        private List<ItemBase> _changed = new List<ItemBase>();
        private byte[] _copiedItem;
        private ItemBase _editorItem;

        public FrmItem()
        {
            ApplyHooks();
            InitializeComponent();
            lstItems.LostFocus += itemList_FocusChanged;
            lstItems.GotFocus += itemList_FocusChanged;
        }

        protected override void GameObjectUpdatedDelegate(GameObjectType type)
        {
            if (type == GameObjectType.Item)
            {
                InitEditor();
                if (_editorItem != null && !ItemBase.Lookup.Values.Contains(_editorItem))
                {
                    _editorItem = null;
                    UpdateEditor();
                }
            }
            else if (type == GameObjectType.Class || type == GameObjectType.Projectile ||
                     type == GameObjectType.Animation ||
                     type == GameObjectType.Spell)
            {
                frmItem_Load(null, null);
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
            if (changingName) return;
            _editorItem =
                ItemBase.Lookup.Get<ItemBase>(
                    Database.GameObjectIdFromList(GameObjectType.Item, lstItems.SelectedIndex));
            UpdateEditor();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            cmbPic.Items.Clear();
            cmbPic.Items.Add(Strings.Get("general", "none"));

            var itemnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.Get("general", "none"));
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add(Strings.Get("general", "none"));
            cmbTeachSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.Get("general", "none"));
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add(Strings.Get("general", "none"));
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add(Strings.Get("general", "none"));
            string[] paperdollnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Paperdoll);
            for (var i = 0; i < paperdollnames.Length; i++)
            {
                cmbMalePaperdoll.Items.Add(paperdollnames[i]);
                cmbFemalePaperdoll.Items.Add(paperdollnames[i]);
            }

            nudStr.Maximum = Options.MaxStatValue;
            nudMag.Maximum = Options.MaxStatValue;
            nudDef.Maximum = Options.MaxStatValue;
            nudMR.Maximum = Options.MaxStatValue;
            nudSpd.Maximum = Options.MaxStatValue;

            InitLocalization();
            UpdateEditor();
        }

        private void InitLocalization()
        {
            Text = Strings.Get("itemeditor", "title");
            toolStripItemNew.Text = Strings.Get("itemeditor", "new");
            toolStripItemDelete.Text = Strings.Get("itemeditor", "delete");
            toolStripItemCopy.Text = Strings.Get("itemeditor", "copy");
            toolStripItemPaste.Text = Strings.Get("itemeditor", "paste");
            toolStripItemUndo.Text = Strings.Get("itemeditor", "undo");

            grpItems.Text = Strings.Get("itemeditor", "items");
            grpGeneral.Text = Strings.Get("itemeditor", "general");
            lblName.Text = Strings.Get("itemeditor", "name");
            lblType.Text = Strings.Get("itemeditor", "type");
            cmbType.Items.Clear();
            for (int i = 0; i < 7; i++)
            {
                cmbType.Items.Add(Strings.Get("itemeditor", "type" + i));
            }
            lblDesc.Text = Strings.Get("itemeditor", "description");
            lblPic.Text = Strings.Get("itemeditor", "picture");
            lblPrice.Text = Strings.Get("itemeditor", "price");
            lblAnim.Text = Strings.Get("itemeditor", "animation");
            chkBound.Text = Strings.Get("itemeditor", "bound");
            chkStackable.Text = Strings.Get("itemeditor", "stackable");
            btnEditRequirements.Text = Strings.Get("itemeditor", "requirements");

            grpEquipment.Text = Strings.Get("itemeditor", "equipment");
            lblEquipmentSlot.Text = Strings.Get("itemeditor", "slot");
            grpStatBonuses.Text = Strings.Get("itemeditor", "bonuses");
            lblStr.Text = Strings.Get("itemeditor", "attackbonus");
            lblDef.Text = Strings.Get("itemeditor", "defensebonus");
            lblSpd.Text = Strings.Get("itemeditor", "speedbonus");
            lblMag.Text = Strings.Get("itemeditor", "abilitypowerbonus");
            lblMR.Text = Strings.Get("itemeditor", "magicresistbonus");
            lblRange.Text = Strings.Get("itemeditor", "bonusrange");
            lblBonusEffect.Text = Strings.Get("itemeditor", "bonuseffect");
            lblEffectPercent.Text = Strings.Get("itemeditor", "bonusamount");
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.Get("itemeditor", "bonuseffect" + i));
            }

            grpWeaponProperties.Text = Strings.Get("itemeditor", "weaponproperties");
            chk2Hand.Text = Strings.Get("itemeditor", "twohanded");
            lblDamage.Text = Strings.Get("itemeditor", "basedamage");
            lblCritChance.Text = Strings.Get("itemeditor", "critchance");
            lblDamageType.Text = Strings.Get("itemeditor", "damagetype");
            cmbDamageType.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbDamageType.Items.Add(Strings.Get("itemeditor", "damagetype" + i));
            }
            lblScalingStat.Text = Strings.Get("itemeditor", "scalingstat");
            lblScalingAmount.Text = Strings.Get("itemeditor", "scalingamount");
            lblAttackAnimation.Text = Strings.Get("itemeditor", "attackanimation");
            lblProjectile.Text = Strings.Get("itemeditor", "projectile");
            lblToolType.Text = Strings.Get("itemeditor", "tooltype");

            lblMalePaperdoll.Text = Strings.Get("itemeditor", "malepaperdoll");
            lblFemalePaperdoll.Text = Strings.Get("itemeditor", "femalepaperdoll");

            grpBags.Text = Strings.Get("itemeditor", "bagpanel");
            lblBag.Text = Strings.Get("itemeditor", "bagslots");

            grpSpell.Text = Strings.Get("itemeditor", "spellpanel");
            lblSpell.Text = Strings.Get("itemeditor", "spell");

            grpEvent.Text = Strings.Get("itemeditor", "eventpanel");
            lblEvent.Text = Strings.Get("itemeditor", "event");

            grpConsumable.Text = Strings.Get("itemeditor", "consumeablepanel");
            lblVital.Text = Strings.Get("itemeditor", "vital");
            lblInterval.Text = Strings.Get("itemeditor", "consumeinterval");
            cmbConsume.Items.Clear();
            for (int i = 0; i < 2; i++)
            {
                cmbConsume.Items.Add(Strings.Get("combat", "vital" + i));
            }

            btnSave.Text = Strings.Get("itemeditor", "save");
            btnCancel.Text = Strings.Get("itemeditor", "cancel");
        }

        public void InitEditor()
        {
            lstItems.Items.Clear();
            lstItems.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.Get("general", "none"));
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < 3; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.Get("itemeditor", "bonuseffect" + i));
            }
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add(Strings.Get("general", "none"));
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
        }

        private void UpdateEditor()
        {
            if (_editorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = _editorItem.Name;
                txtDesc.Text = _editorItem.Desc;
                cmbType.SelectedIndex = _editorItem.ItemType;
                cmbPic.SelectedIndex = cmbPic.FindString(TextUtils.NullToNone(_editorItem.Pic));
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
                cmbToolType.SelectedIndex = _editorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.AttackAnimation) + 1;
                RefreshExtendedData();
                if (_editorItem.ItemType == (int) ItemTypes.Equipment)
                    cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
                nudEffectPercent.Value = _editorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(_editorItem.Data4);
                cmbMalePaperdoll.SelectedIndex = cmbMalePaperdoll.FindString(TextUtils.NullToNone(_editorItem.MalePaperdoll));
                cmbFemalePaperdoll.SelectedIndex = cmbFemalePaperdoll.FindString(TextUtils.NullToNone(_editorItem.FemalePaperdoll));
                if (_editorItem.ItemType == (int) ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = _editorItem.Data1;
                    nudInterval.Value = _editorItem.Data2;
                }
                if (cmbPic.SelectedIndex > 0)
                {
                    picItem.BackgroundImage = System.Drawing.Image.FromFile("resources/items/" + cmbPic.Text);
                }
                else
                {
                    picItem.BackgroundImage = null;
                }
                if (cmbMalePaperdoll.SelectedIndex > 0)
                {
                    picMalePaperdoll.BackgroundImage =
                        System.Drawing.Image.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text);
                }
                else
                {
                    picFemalePaperdoll.BackgroundImage = null;
                }

                if (cmbFemalePaperdoll.SelectedIndex > 0)
                {
                    picFemalePaperdoll.BackgroundImage =
                        System.Drawing.Image.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text);
                }
                else
                {
                    picFemalePaperdoll.BackgroundImage = null;
                }

                cmbDamageType.SelectedIndex = _editorItem.DamageType;
                cmbScalingStat.SelectedIndex = _editorItem.ScalingStat;

                //External References
                cmbProjectile.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Projectile, _editorItem.Projectile) + 1;
                cmbAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, _editorItem.Animation) +
                    1;

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

        private void RefreshExtendedData()
        {
            grpConsumable.Visible = false;
            grpSpell.Visible = false;
            grpEquipment.Visible = false;
            grpEvent.Visible = false;
            grpBags.Visible = false;

            if (_editorItem.ItemType != cmbType.SelectedIndex)
            {
                _editorItem.Damage = 0;
                _editorItem.Tool = -1;
                _editorItem.Data1 = 0;
                _editorItem.Data2 = 0;
                _editorItem.Data3 = 0;
                _editorItem.Data4 = 0;
            }

            if (cmbType.SelectedIndex == (int) ItemTypes.Consumable)
            {
                cmbConsume.SelectedIndex = _editorItem.Data1;
                nudInterval.Value = _editorItem.Data2;
                grpConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Spell)
            {
                cmbTeachSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell, _editorItem.Data1) + 1;
                grpSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Event)
            {
                cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CommonEvent, _editorItem.Data1) +
                                         1;
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Equipment)
            {
                grpEquipment.Visible = true;
                if (_editorItem.Data1 < -1 || _editorItem.Data1 >= cmbEquipmentSlot.Items.Count)
                {
                    _editorItem.Data1 = 0;
                }
                cmbEquipmentSlot.SelectedIndex = _editorItem.Data1;
                cmbEquipmentBonus.SelectedIndex = _editorItem.Data2;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Bag)
            {
                if (_editorItem.Data1 < 1)
                {
                    _editorItem.Data1 = 1;
                } //Cant have no space or negative space.
                grpBags.Visible = true;
                nudBag.Value = _editorItem.Data1;
            }

            _editorItem.ItemType = cmbType.SelectedIndex;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshExtendedData();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            changingName = true;
            _editorItem.Name = txtName.Text;
            lstItems.Items[Database.GameObjectListIndex(GameObjectType.Item, _editorItem.Index)] = txtName.Text;
            changingName = false;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Pic = cmbPic.SelectedIndex < 1 ? null : cmbPic.Text;
            if (cmbPic.SelectedIndex > 0)
            {
                picItem.BackgroundImage = System.Drawing.Image.FromFile("resources/items/" + cmbPic.Text);
            }
            else
            {
                picItem.BackgroundImage = null;
            }
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = cmbConsume.SelectedIndex;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.MalePaperdoll = TextUtils.SanitizeNone(cmbMalePaperdoll.Text);
            if (cmbMalePaperdoll.SelectedIndex > 0)
            {
                picMalePaperdoll.BackgroundImage =
                    System.Drawing.Image.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text);
            }
            else
            {
                picMalePaperdoll.BackgroundImage = null;
            }
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
            _editorItem.FemalePaperdoll = TextUtils.SanitizeNone(cmbFemalePaperdoll.Text);
            if (cmbFemalePaperdoll.SelectedIndex > 0)
            {
                picFemalePaperdoll.BackgroundImage =
                    System.Drawing.Image.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text);
            }
            else
            {
                picFemalePaperdoll.BackgroundImage = null;
            }
        }

        private void toolStripItemNew_Click(object sender, EventArgs e)
        {
            PacketSender.SendCreateObject(GameObjectType.Item);
        }

        private void toolStripItemDelete_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstItems.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.Get("itemeditor", "deleteprompt"),
                        Strings.Get("itemeditor", "deletetitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(_editorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (_editorItem != null && lstItems.Focused)
            {
                _copiedItem = _editorItem.BinaryData;
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
                if (DarkMessageBox.ShowWarning(Strings.Get("itemeditor", "undoprompt"),
                        Strings.Get("itemeditor", "undotitle"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
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
            _editorItem.AttackAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbAttackAnimation.SelectedIndex - 1);
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
            _editorItem.Projectile = Database.GameObjectIdFromList(GameObjectType.Projectile,
                cmbProjectile.SelectedIndex - 1);
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new frmDynamicRequirements(_editorItem.UseReqs, RequirementType.Item);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Animation =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Database.GameObjectIdFromList(GameObjectType.CommonEvent, cmbEvent.SelectedIndex - 1);
        }

        private void cmbTeachSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = Database.GameObjectIdFromList(GameObjectType.Spell, cmbTeachSpell.SelectedIndex - 1);
        }

        private void nudPrice_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Price = (int) nudPrice.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudEffectPercent_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data3 = (int) nudEffectPercent.Value;
        }

        private void nudRange_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatGrowth = (int) nudRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[0] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[1] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[2] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[3] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.StatsGiven[4] = (int) nudSpd.Value;
        }

        private void nudBag_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data1 = (int) nudBag.Value;
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            _editorItem.Data2 = (int) nudInterval.Value;
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