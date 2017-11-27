using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Editor.Classes.Localization;
using Intersect.Utilities;

namespace Intersect.Editor.Forms
{
    public partial class FrmItem : EditorForm
    {
        private List<ItemBase> mChanged = new List<ItemBase>();
        private byte[] mCopiedItem;
        private ItemBase mEditorItem;

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
                if (mEditorItem != null && !ItemBase.Lookup.Values.Contains(mEditorItem))
                {
                    mEditorItem = null;
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
            foreach (var item in mChanged)
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
            foreach (var item in mChanged)
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
            if (mChangingName) return;
            mEditorItem =
                ItemBase.Lookup.Get<ItemBase>(
                    Database.GameObjectIdFromList(GameObjectType.Item, lstItems.SelectedIndex));
            UpdateEditor();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            cmbPic.Items.Clear();
            cmbPic.Items.Add(Strings.general.none);

            var itemnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.general.none);
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.general.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add(Strings.general.none);
            cmbTeachSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.general.none);
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add(Strings.general.none);
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add(Strings.general.none);
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
            Text = Strings.itemeditor.title;
            toolStripItemNew.Text = Strings.itemeditor.New;
            toolStripItemDelete.Text = Strings.itemeditor.delete;
            toolStripItemCopy.Text = Strings.itemeditor.copy;
            toolStripItemPaste.Text = Strings.itemeditor.paste;
            toolStripItemUndo.Text = Strings.itemeditor.undo;

            grpItems.Text = Strings.itemeditor.items;
            grpGeneral.Text = Strings.itemeditor.general;
            lblName.Text = Strings.itemeditor.name;
            lblType.Text = Strings.itemeditor.type;
            cmbType.Items.Clear();
            for (int i = 0; i < Strings.itemeditor.types.Length; i++)
            {
                cmbType.Items.Add(Strings.itemeditor.types[i]);
            }
            lblDesc.Text = Strings.itemeditor.description;
            lblPic.Text = Strings.itemeditor.picture;
            lblPrice.Text = Strings.itemeditor.price;
            lblAnim.Text = Strings.itemeditor.animation;
            chkBound.Text = Strings.itemeditor.bound;
            chkStackable.Text = Strings.itemeditor.stackable;
            btnEditRequirements.Text = Strings.itemeditor.requirements;

            grpEquipment.Text = Strings.itemeditor.equipment;
            lblEquipmentSlot.Text = Strings.itemeditor.slot;
            grpStatBonuses.Text = Strings.itemeditor.bonuses;
            lblStr.Text = Strings.itemeditor.attackbonus;
            lblDef.Text = Strings.itemeditor.defensebonus;
            lblSpd.Text = Strings.itemeditor.speedbonus;
            lblMag.Text = Strings.itemeditor.abilitypowerbonus;
            lblMR.Text = Strings.itemeditor.magicresistbonus;
            lblRange.Text = Strings.itemeditor.bonusrange;
            lblBonusEffect.Text = Strings.itemeditor.bonuseffect;
            lblEffectPercent.Text = Strings.itemeditor.bonusamount;
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < Strings.itemeditor.bonuseffects.Length; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.itemeditor.bonuseffects[i]);
            }

            grpWeaponProperties.Text = Strings.itemeditor.weaponproperties;
            chk2Hand.Text = Strings.itemeditor.twohanded;
            lblDamage.Text = Strings.itemeditor.basedamage;
            lblCritChance.Text = Strings.itemeditor.critchance;
            lblDamageType.Text = Strings.itemeditor.damagetype;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.combat.damagetypes.Length; i++)
            {
                cmbDamageType.Items.Add(Strings.combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.itemeditor.scalingstat;
            lblScalingAmount.Text = Strings.itemeditor.scalingamount;
            lblAttackAnimation.Text = Strings.itemeditor.attackanimation;
            lblProjectile.Text = Strings.itemeditor.projectile;
            lblToolType.Text = Strings.itemeditor.tooltype;

            lblMalePaperdoll.Text = Strings.itemeditor.malepaperdoll;
            lblFemalePaperdoll.Text = Strings.itemeditor.femalepaperdoll;

            grpBags.Text = Strings.itemeditor.bagpanel;
            lblBag.Text = Strings.itemeditor.bagslots;

            grpSpell.Text = Strings.itemeditor.spellpanel;
            lblSpell.Text = Strings.itemeditor.spell;

            grpEvent.Text = Strings.itemeditor.eventpanel;
            lblEvent.Text = Strings.itemeditor.Event;

            grpConsumable.Text = Strings.itemeditor.consumeablepanel;
            lblVital.Text = Strings.itemeditor.vital;
            lblInterval.Text = Strings.itemeditor.consumeinterval;
            cmbConsume.Items.Clear();
            for (int i = 0; i < 2; i++)
            {
                cmbConsume.Items.Add(Strings.combat.vitals[i]);
            }

            btnSave.Text = Strings.itemeditor.save;
            btnCancel.Text = Strings.itemeditor.cancel;
        }

        public void InitEditor()
        {
            lstItems.Items.Clear();
            lstItems.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.general.none);
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < Strings.itemeditor.bonuseffects.Length; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.itemeditor.bonuseffects[i]);
            }
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add(Strings.general.none);
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                txtDesc.Text = mEditorItem.Desc;
                cmbType.SelectedIndex = mEditorItem.ItemType;
                cmbPic.SelectedIndex = cmbPic.FindString(TextUtils.NullToNone(mEditorItem.Pic));
                nudPrice.Value = mEditorItem.Price;
                nudStr.Value = mEditorItem.StatsGiven[0];
                nudMag.Value = mEditorItem.StatsGiven[1];
                nudDef.Value = mEditorItem.StatsGiven[2];
                nudMR.Value = mEditorItem.StatsGiven[3];
                nudSpd.Value = mEditorItem.StatsGiven[4];
                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudScaling.Value = mEditorItem.Scaling;
                nudRange.Value = mEditorItem.StatGrowth;
                chkBound.Checked = Convert.ToBoolean(mEditorItem.Bound);
                chkStackable.Checked = Convert.ToBoolean(mEditorItem.Stackable);
                cmbToolType.SelectedIndex = mEditorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.AttackAnimation) + 1;
                RefreshExtendedData();
                if (mEditorItem.ItemType == (int) ItemTypes.Equipment)
                    cmbEquipmentBonus.SelectedIndex = mEditorItem.Data2;
                nudEffectPercent.Value = mEditorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(mEditorItem.Data4);
                cmbMalePaperdoll.SelectedIndex = cmbMalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.MalePaperdoll));
                cmbFemalePaperdoll.SelectedIndex = cmbFemalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.FemalePaperdoll));
                if (mEditorItem.ItemType == (int) ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = mEditorItem.Data1;
                    nudInterval.Value = mEditorItem.Data2;
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

                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;

                //External References
                cmbProjectile.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Projectile, mEditorItem.Projectile) + 1;
                cmbAnimation.SelectedIndex =
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.Animation) +
                    1;

                if (mChanged.IndexOf(mEditorItem) == -1)
                {
                    mChanged.Add(mEditorItem);
                    mEditorItem.MakeBackup();
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

            if (mEditorItem.ItemType != cmbType.SelectedIndex)
            {
                mEditorItem.Damage = 0;
                mEditorItem.Tool = -1;
                mEditorItem.Data1 = 0;
                mEditorItem.Data2 = 0;
                mEditorItem.Data3 = 0;
                mEditorItem.Data4 = 0;
            }

            if (cmbType.SelectedIndex == (int) ItemTypes.Consumable)
            {
                cmbConsume.SelectedIndex = mEditorItem.Data1;
                nudInterval.Value = mEditorItem.Data2;
                grpConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Spell)
            {
                cmbTeachSpell.SelectedIndex = Database.GameObjectListIndex(GameObjectType.Spell, mEditorItem.Data1) + 1;
                grpSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Event)
            {
                cmbEvent.SelectedIndex = Database.GameObjectListIndex(GameObjectType.CommonEvent, mEditorItem.Data1) +
                                         1;
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Equipment)
            {
                grpEquipment.Visible = true;
                if (mEditorItem.Data1 < -1 || mEditorItem.Data1 >= cmbEquipmentSlot.Items.Count)
                {
                    mEditorItem.Data1 = 0;
                }
                cmbEquipmentSlot.SelectedIndex = mEditorItem.Data1;
                cmbEquipmentBonus.SelectedIndex = mEditorItem.Data2;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Bag)
            {
                if (mEditorItem.Data1 < 1)
                {
                    mEditorItem.Data1 = 1;
                } //Cant have no space or negative space.
                grpBags.Visible = true;
                nudBag.Value = mEditorItem.Data1;
            }

            mEditorItem.ItemType = cmbType.SelectedIndex;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshExtendedData();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            mChangingName = true;
            mEditorItem.Name = txtName.Text;
            lstItems.Items[Database.GameObjectListIndex(GameObjectType.Item, mEditorItem.Index)] = txtName.Text;
            mChangingName = false;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Pic = cmbPic.SelectedIndex < 1 ? null : cmbPic.Text;
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
            mEditorItem.Data1 = cmbConsume.SelectedIndex;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.MalePaperdoll = TextUtils.SanitizeNone(cmbMalePaperdoll.Text);
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
            mEditorItem.Desc = txtDesc.Text;
        }

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = cmbEquipmentSlot.SelectedIndex;
            if (cmbEquipmentSlot.SelectedIndex == Options.WeaponIndex)
            {
                grpWeaponProperties.Show();
            }
            else
            {
                grpWeaponProperties.Hide();

                mEditorItem.Projectile = -1;
                mEditorItem.Tool = -1;
                mEditorItem.Damage = 0;
                mEditorItem.Data4 = 0;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Tool = cmbToolType.SelectedIndex - 1;
        }

        private void cmbEquipmentBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data2 = cmbEquipmentBonus.SelectedIndex;
        }

        private void chk2Hand_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Data4 = Convert.ToInt32(chk2Hand.Checked);
        }

        private void FrmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void cmbFemalePaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.FemalePaperdoll = TextUtils.SanitizeNone(cmbFemalePaperdoll.Text);
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
            if (mEditorItem != null && lstItems.Focused)
            {
                if (DarkMessageBox.ShowWarning(Strings.itemeditor.deleteprompt,
                        Strings.itemeditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    PacketSender.SendDeleteObject(mEditorItem);
                }
            }
        }

        private void toolStripItemCopy_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && lstItems.Focused)
            {
                mCopiedItem = mEditorItem.BinaryData;
                toolStripItemPaste.Enabled = true;
            }
        }

        private void toolStripItemPaste_Click(object sender, EventArgs e)
        {
            if (mEditorItem != null && mCopiedItem != null && lstItems.Focused)
            {
                mEditorItem.Load(mCopiedItem);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(Strings.itemeditor.undoprompt,
                        Strings.itemeditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                    DialogResult.Yes)
                {
                    mEditorItem.RestoreBackup();
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
            toolStripItemCopy.Enabled = mEditorItem != null && lstItems.Focused;
            toolStripItemPaste.Enabled = mEditorItem != null && mCopiedItem != null && lstItems.Focused;
            toolStripItemDelete.Enabled = mEditorItem != null && lstItems.Focused;
            toolStripItemUndo.Enabled = mEditorItem != null && lstItems.Focused;
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
            mEditorItem.AttackAnimation = Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbAttackAnimation.SelectedIndex - 1);
        }

        private void cmbDamageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.DamageType = cmbDamageType.SelectedIndex;
        }

        private void cmbScalingStat_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.ScalingStat = cmbScalingStat.SelectedIndex;
        }

        private void cmbProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Projectile = Database.GameObjectIdFromList(GameObjectType.Projectile,
                cmbProjectile.SelectedIndex - 1);
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.UseReqs, RequirementType.Item);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animation =
                Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1);
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = Database.GameObjectIdFromList(GameObjectType.CommonEvent, cmbEvent.SelectedIndex - 1);
        }

        private void cmbTeachSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = Database.GameObjectIdFromList(GameObjectType.Spell, cmbTeachSpell.SelectedIndex - 1);
        }

        private void nudPrice_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Price = (int) nudPrice.Value;
        }

        private void nudScaling_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Scaling = (int) nudScaling.Value;
        }

        private void nudDamage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Damage = (int) nudDamage.Value;
        }

        private void nudCritChance_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritChance = (int) nudCritChance.Value;
        }

        private void nudEffectPercent_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data3 = (int) nudEffectPercent.Value;
        }

        private void nudRange_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatGrowth = (int) nudRange.Value;
        }

        private void nudStr_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[0] = (int) nudStr.Value;
        }

        private void nudMag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[1] = (int) nudMag.Value;
        }

        private void nudDef_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[2] = (int) nudDef.Value;
        }

        private void nudMR_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[3] = (int) nudMR.Value;
        }

        private void nudSpd_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.StatsGiven[4] = (int) nudSpd.Value;
        }

        private void nudBag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data1 = (int) nudBag.Value;
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Data2 = (int) nudInterval.Value;
        }

        private void chkBound_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Bound = Convert.ToInt32(chkBound.Checked);
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Stackable = Convert.ToInt32(chkStackable.Checked);
        }
    }
}