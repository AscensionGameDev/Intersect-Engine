using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect.Editor.ContentManagement;
using Intersect.Editor.Core;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmItem : EditorForm
    {
        private List<ItemBase> mChanged = new List<ItemBase>();
        private string mCopiedItem;
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
            cmbPic.Items.Add(Strings.General.none);

            var itemnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbScalingStat.Items.Clear();
            for (int x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(Database.GetGameObjectList(GameObjectType.Animation));
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add(Strings.General.none);
            cmbTeachSpell.Items.AddRange(Database.GetGameObjectList(GameObjectType.Spell));
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.General.none);
            cmbEvent.Items.AddRange(Database.GetGameObjectList(GameObjectType.CommonEvent));
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add(Strings.General.none);
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add(Strings.General.none);
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
            Text = Strings.ItemEditor.title;
            toolStripItemNew.Text = Strings.ItemEditor.New;
            toolStripItemDelete.Text = Strings.ItemEditor.delete;
            toolStripItemCopy.Text = Strings.ItemEditor.copy;
            toolStripItemPaste.Text = Strings.ItemEditor.paste;
            toolStripItemUndo.Text = Strings.ItemEditor.undo;

            grpItems.Text = Strings.ItemEditor.items;
            grpGeneral.Text = Strings.ItemEditor.general;
            lblName.Text = Strings.ItemEditor.name;
            lblType.Text = Strings.ItemEditor.type;
            cmbType.Items.Clear();
            for (int i = 0; i < Strings.ItemEditor.types.Length; i++)
            {
                cmbType.Items.Add(Strings.ItemEditor.types[i]);
            }
            lblDesc.Text = Strings.ItemEditor.description;
            lblPic.Text = Strings.ItemEditor.picture;
            lblPrice.Text = Strings.ItemEditor.price;
            lblAnim.Text = Strings.ItemEditor.animation;
            chkBound.Text = Strings.ItemEditor.bound;
            chkStackable.Text = Strings.ItemEditor.stackable;
            btnEditRequirements.Text = Strings.ItemEditor.requirements;

            grpEquipment.Text = Strings.ItemEditor.equipment;
            lblEquipmentSlot.Text = Strings.ItemEditor.slot;
            grpStatBonuses.Text = Strings.ItemEditor.bonuses;
            lblStr.Text = Strings.ItemEditor.attackbonus;
            lblDef.Text = Strings.ItemEditor.defensebonus;
            lblSpd.Text = Strings.ItemEditor.speedbonus;
            lblMag.Text = Strings.ItemEditor.abilitypowerbonus;
            lblMR.Text = Strings.ItemEditor.magicresistbonus;
            lblRange.Text = Strings.ItemEditor.bonusrange;
            lblBonusEffect.Text = Strings.ItemEditor.bonuseffect;
            lblEffectPercent.Text = Strings.ItemEditor.bonusamount;
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < Strings.ItemEditor.bonuseffects.Length; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.ItemEditor.bonuseffects[i]);
            }

            grpWeaponProperties.Text = Strings.ItemEditor.weaponproperties;
            chk2Hand.Text = Strings.ItemEditor.twohanded;
            lblDamage.Text = Strings.ItemEditor.basedamage;
            lblCritChance.Text = Strings.ItemEditor.critchance;
            lblDamageType.Text = Strings.ItemEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (int i = 0; i < Strings.Combat.damagetypes.Length; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }
            lblScalingStat.Text = Strings.ItemEditor.scalingstat;
            lblScalingAmount.Text = Strings.ItemEditor.scalingamount;
            lblAttackAnimation.Text = Strings.ItemEditor.attackanimation;
            lblProjectile.Text = Strings.ItemEditor.projectile;
            lblToolType.Text = Strings.ItemEditor.tooltype;

            lblMalePaperdoll.Text = Strings.ItemEditor.malepaperdoll;
            lblFemalePaperdoll.Text = Strings.ItemEditor.femalepaperdoll;

            grpBags.Text = Strings.ItemEditor.bagpanel;
            lblBag.Text = Strings.ItemEditor.bagslots;

            grpSpell.Text = Strings.ItemEditor.spellpanel;
            lblSpell.Text = Strings.ItemEditor.spell;

            grpEvent.Text = Strings.ItemEditor.eventpanel;
            lblEvent.Text = Strings.ItemEditor.Event;

            grpConsumable.Text = Strings.ItemEditor.consumeablepanel;
            lblVital.Text = Strings.ItemEditor.vital;
            lblInterval.Text = Strings.ItemEditor.consumeinterval;
            cmbConsume.Items.Clear();
            for (int i = 0; i < 2; i++)
            {
                cmbConsume.Items.Add(Strings.Combat.vitals[i]);
            }
            cmbConsume.Items.Add(Strings.Combat.exp);

            btnSave.Text = Strings.ItemEditor.save;
            btnCancel.Text = Strings.ItemEditor.cancel;
        }

        public void InitEditor()
        {
            lstItems.Items.Clear();
            lstItems.Items.AddRange(Database.GetGameObjectList(GameObjectType.Item));
            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.General.none);
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Clear();
            for (int i = 0; i < Strings.ItemEditor.bonuseffects.Length; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.ItemEditor.bonuseffects[i]);
            }
            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add(Strings.General.none);
            cmbProjectile.Items.AddRange(Database.GetGameObjectList(GameObjectType.Projectile));
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                txtDesc.Text = mEditorItem.Desc;
                cmbType.SelectedIndex = (int) mEditorItem.ItemType;
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
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.AttackAnimationId) + 1;
                RefreshExtendedData();
                if (mEditorItem.ItemType == ItemTypes.Equipment)
                    cmbEquipmentBonus.SelectedIndex = mEditorItem.Data2;
                nudEffectPercent.Value = mEditorItem.Data3;
                chk2Hand.Checked = Convert.ToBoolean(mEditorItem.Data4);
                cmbMalePaperdoll.SelectedIndex = cmbMalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.MalePaperdoll));
                cmbFemalePaperdoll.SelectedIndex = cmbFemalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.FemalePaperdoll));
                if (mEditorItem.ItemType == ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = (int) mEditorItem.ConsumableType;
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
                    Database.GameObjectListIndex(GameObjectType.Animation, mEditorItem.AnimationId) +
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

            if ((int) mEditorItem.ItemType != cmbType.SelectedIndex)
            {
                mEditorItem.ConsumableType = ConsumableType.None;
                mEditorItem.Damage = 0;
                mEditorItem.Tool = -1;
                mEditorItem.Data1 = 0;
                mEditorItem.Data2 = 0;
                mEditorItem.Data3 = 0;
                mEditorItem.Data4 = 0;
            }

            if (cmbType.SelectedIndex == (int) ItemTypes.Consumable)
            {
                cmbConsume.SelectedIndex = (int) mEditorItem.ConsumableType;
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

            mEditorItem.ItemType = (ItemTypes) cmbType.SelectedIndex;
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
            mEditorItem.ConsumableType = (ConsumableType) cmbConsume.SelectedIndex;
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
                if (DarkMessageBox.ShowWarning(Strings.ItemEditor.deleteprompt,
                        Strings.ItemEditor.deletetitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
                mCopiedItem = mEditorItem.JsonData;
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
                if (DarkMessageBox.ShowWarning(Strings.ItemEditor.undoprompt,
                        Strings.ItemEditor.undotitle, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
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
            mEditorItem.AttackAnimation = AnimationBase.Lookup.Get<AnimationBase>(Database.GameObjectIdFromList(GameObjectType.Animation,
                cmbAttackAnimation.SelectedIndex - 1));
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
            var frm = new FrmDynamicRequirements(mEditorItem.UsageRequirements, RequirementType.Item);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animation =
                AnimationBase.Lookup.Get<AnimationBase>(Database.GameObjectIdFromList(GameObjectType.Animation, cmbAnimation.SelectedIndex - 1));
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
            mEditorItem.Bound = chkBound.Checked;
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Stackable = chkStackable.Checked;
        }
    }
}