﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using DarkUI.Forms;

using Intersect.Editor.Content;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.Utilities;

namespace Intersect.Editor.Forms.Editors
{

    public partial class FrmItem : EditorForm
    {

        private List<ItemBase> mChanged = new List<ItemBase>();

        private string mCopiedItem;

        private ItemBase mEditorItem;

        private List<string> mExpandedFolders = new List<string>();

        private List<string> mKnownFolders = new List<string>();

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
            else if (type == GameObjectType.Class ||
                     type == GameObjectType.Projectile ||
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

        private void frmItem_Load(object sender, EventArgs e)
        {
            cmbPic.Items.Clear();
            cmbPic.Items.Add(Strings.General.none);

            var itemnames = GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Item);
            cmbPic.Items.AddRange(itemnames);

            cmbAttackAnimation.Items.Clear();
            cmbAttackAnimation.Items.Add(Strings.General.none);
            cmbAttackAnimation.Items.AddRange(AnimationBase.Names);
            cmbScalingStat.Items.Clear();
            for (var x = 0; x < Options.MaxStats; x++)
            {
                cmbScalingStat.Items.Add(Globals.GetStatName(x));
            }

            cmbAnimation.Items.Clear();
            cmbAnimation.Items.Add(Strings.General.none);
            cmbAnimation.Items.AddRange(AnimationBase.Names);
            cmbEquipmentAnimation.Items.Clear();
            cmbEquipmentAnimation.Items.Add(Strings.General.none);
            cmbEquipmentAnimation.Items.AddRange(AnimationBase.Names);
            cmbTeachSpell.Items.Clear();
            cmbTeachSpell.Items.Add(Strings.General.none);
            cmbTeachSpell.Items.AddRange(SpellBase.Names);
            cmbEvent.Items.Clear();
            cmbEvent.Items.Add(Strings.General.none);
            cmbEvent.Items.AddRange(EventBase.Names);
            cmbMalePaperdoll.Items.Clear();
            cmbMalePaperdoll.Items.Add(Strings.General.none);
            cmbFemalePaperdoll.Items.Clear();
            cmbFemalePaperdoll.Items.Add(Strings.General.none);
            var paperdollnames =
                GameContentManager.GetSmartSortedTextureNames(GameContentManager.TextureType.Paperdoll);

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

            nudStr.Minimum = -Options.MaxStatValue;
            nudMag.Minimum = -Options.MaxStatValue;
            nudDef.Minimum = -Options.MaxStatValue;
            nudMR.Minimum = -Options.MaxStatValue;
            nudSpd.Minimum = -Options.MaxStatValue;

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
            for (var i = 0; i < Strings.ItemEditor.types.Count; i++)
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

            cmbRarity.Items.Clear();
            for (var i = 0; i < Strings.ItemEditor.rarity.Count; i++)
            {
                cmbRarity.Items.Add(Strings.ItemEditor.rarity[i]);
            }

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
            lblEquipmentAnimation.Text = Strings.ItemEditor.equipmentanimation;
            cmbEquipmentBonus.Items.Clear();
            for (var i = 0; i < Strings.ItemEditor.bonuseffects.Count; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.ItemEditor.bonuseffects[i]);
            }

            grpWeaponProperties.Text = Strings.ItemEditor.weaponproperties;
            chk2Hand.Text = Strings.ItemEditor.twohanded;
            lblDamage.Text = Strings.ItemEditor.basedamage;
            lblCritChance.Text = Strings.ItemEditor.critchance;
            lblCritMultiplier.Text = Strings.ItemEditor.critmultiplier;
            lblDamageType.Text = Strings.ItemEditor.damagetype;
            cmbDamageType.Items.Clear();
            for (var i = 0; i < Strings.Combat.damagetypes.Count; i++)
            {
                cmbDamageType.Items.Add(Strings.Combat.damagetypes[i]);
            }

            lblScalingStat.Text = Strings.ItemEditor.scalingstat;
            lblScalingAmount.Text = Strings.ItemEditor.scalingamount;
            lblAttackAnimation.Text = Strings.ItemEditor.attackanimation;
            lblProjectile.Text = Strings.ItemEditor.projectile;
            lblToolType.Text = Strings.ItemEditor.tooltype;

            lblCooldown.Text = Strings.ItemEditor.cooldown;

            grpVitalBonuses.Text = Strings.ItemEditor.vitalbonuses;
            lblHealthBonus.Text = Strings.ItemEditor.health;
            lblManaBonus.Text = Strings.ItemEditor.mana;

            grpRegen.Text = Strings.ItemEditor.regen;
            lblHpRegen.Text = Strings.ItemEditor.hpregen;
            lblManaRegen.Text = Strings.ItemEditor.mpregen;
            lblRegenHint.Text = Strings.ItemEditor.regenhint;

            grpAttackSpeed.Text = Strings.ItemEditor.attackspeed;
            lblAttackSpeedModifier.Text = Strings.ItemEditor.attackspeedmodifier;
            lblAttackSpeedValue.Text = Strings.ItemEditor.attackspeedvalue;
            cmbAttackSpeedModifier.Items.Clear();
            foreach (var val in Strings.ItemEditor.attackspeedmodifiers.Values)
            {
                cmbAttackSpeedModifier.Items.Add(val.ToString());
            }

            lblMalePaperdoll.Text = Strings.ItemEditor.malepaperdoll;
            lblFemalePaperdoll.Text = Strings.ItemEditor.femalepaperdoll;

            grpBags.Text = Strings.ItemEditor.bagpanel;
            lblBag.Text = Strings.ItemEditor.bagslots;

            grpSpell.Text = Strings.ItemEditor.spellpanel;
            lblSpell.Text = Strings.ItemEditor.spell;
            chkQuickCast.Text = Strings.ItemEditor.quickcast;
            chkDestroy.Text = Strings.ItemEditor.destroyspell;

            grpEvent.Text = Strings.ItemEditor.eventpanel;

            grpConsumable.Text = Strings.ItemEditor.consumeablepanel;
            lblVital.Text = Strings.ItemEditor.vital;
            lblInterval.Text = Strings.ItemEditor.consumeamount;
            cmbConsume.Items.Clear();
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                cmbConsume.Items.Add(Strings.Combat.vitals[i]);
            }

            cmbConsume.Items.Add(Strings.Combat.exp);

            //Searching/Sorting
            btnChronological.ToolTipText = Strings.ItemEditor.sortchronologically;
            txtSearch.Text = Strings.ItemEditor.searchplaceholder;
            lblFolder.Text = Strings.ItemEditor.folderlabel;

            btnSave.Text = Strings.ItemEditor.save;
            btnCancel.Text = Strings.ItemEditor.cancel;
        }

        private void UpdateEditor()
        {
            if (mEditorItem != null)
            {
                pnlContainer.Show();

                txtName.Text = mEditorItem.Name;
                cmbFolder.Text = mEditorItem.Folder;
                txtDesc.Text = mEditorItem.Description;
                cmbType.SelectedIndex = (int) mEditorItem.ItemType;
                cmbPic.SelectedIndex = cmbPic.FindString(TextUtils.NullToNone(mEditorItem.Icon));
                cmbEquipmentAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.EquipmentAnimationId) + 1;
                nudPrice.Value = mEditorItem.Price;
                cmbRarity.SelectedIndex = mEditorItem.Rarity;

                nudStr.Value = mEditorItem.StatsGiven[0];
                nudMag.Value = mEditorItem.StatsGiven[1];
                nudDef.Value = mEditorItem.StatsGiven[2];
                nudMR.Value = mEditorItem.StatsGiven[3];
                nudSpd.Value = mEditorItem.StatsGiven[4];

                nudStrPercentage.Value = mEditorItem.PercentageStatsGiven[0];
                nudMagPercentage.Value = mEditorItem.PercentageStatsGiven[1];
                nudDefPercentage.Value = mEditorItem.PercentageStatsGiven[2];
                nudMRPercentage.Value = mEditorItem.PercentageStatsGiven[3];
                nudSpdPercentage.Value = mEditorItem.PercentageStatsGiven[4];

                nudHealthBonus.Value = mEditorItem.VitalsGiven[0];
                nudManaBonus.Value = mEditorItem.VitalsGiven[1];
                nudHPPercentage.Value = mEditorItem.PercentageVitalsGiven[0];
                nudMPPercentage.Value = mEditorItem.PercentageVitalsGiven[1];
                nudHPRegen.Value = mEditorItem.VitalsRegen[0];
                nudMpRegen.Value = mEditorItem.VitalsRegen[1];

                nudDamage.Value = mEditorItem.Damage;
                nudCritChance.Value = mEditorItem.CritChance;
                nudCritMultiplier.Value = (decimal) mEditorItem.CritMultiplier;
                cmbAttackSpeedModifier.SelectedIndex = mEditorItem.AttackSpeedModifier;
                nudAttackSpeedValue.Value = mEditorItem.AttackSpeedValue;
                nudScaling.Value = mEditorItem.Scaling;
                nudRange.Value = mEditorItem.StatGrowth;
                chkBound.Checked = Convert.ToBoolean(mEditorItem.Bound);
                chkStackable.Checked = Convert.ToBoolean(mEditorItem.Stackable);
                cmbToolType.SelectedIndex = mEditorItem.Tool + 1;
                cmbAttackAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.AttackAnimationId) + 1;
                RefreshExtendedData();
                if (mEditorItem.ItemType == ItemTypes.Equipment)
                {
                    cmbEquipmentBonus.SelectedIndex = (int) mEditorItem.Effect.Type;
                }

                nudEffectPercent.Value = mEditorItem.Effect.Percentage;
                chk2Hand.Checked = mEditorItem.TwoHanded;
                cmbMalePaperdoll.SelectedIndex =
                    cmbMalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.MalePaperdoll));

                cmbFemalePaperdoll.SelectedIndex =
                    cmbFemalePaperdoll.FindString(TextUtils.NullToNone(mEditorItem.FemalePaperdoll));

                if (mEditorItem.ItemType == ItemTypes.Consumable)
                {
                    cmbConsume.SelectedIndex = (int) mEditorItem.Consumable.Type;
                    nudInterval.Value = mEditorItem.Consumable.Value;
                    nudIntervalPercentage.Value = mEditorItem.Consumable.Percentage;
                }

                picItem.BackgroundImage?.Dispose();
                picItem.BackgroundImage = null;
                if (cmbPic.SelectedIndex > 0)
                {
                    picItem.BackgroundImage = System.Drawing.Image.FromFile("resources/items/" + cmbPic.Text);
                }

                picMalePaperdoll.BackgroundImage?.Dispose();
                picMalePaperdoll.BackgroundImage = null;
                if (cmbMalePaperdoll.SelectedIndex > 0)
                {
                    picMalePaperdoll.BackgroundImage =
                        System.Drawing.Image.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text);
                }

                picFemalePaperdoll.BackgroundImage?.Dispose();
                picFemalePaperdoll.BackgroundImage = null;
                if (cmbFemalePaperdoll.SelectedIndex > 0)
                {
                    picFemalePaperdoll.BackgroundImage =
                        System.Drawing.Image.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text);
                }

                cmbDamageType.SelectedIndex = mEditorItem.DamageType;
                cmbScalingStat.SelectedIndex = mEditorItem.ScalingStat;

                //External References
                cmbProjectile.SelectedIndex = ProjectileBase.ListIndex(mEditorItem.ProjectileId) + 1;
                cmbAnimation.SelectedIndex = AnimationBase.ListIndex(mEditorItem.AnimationId) + 1;

                nudCooldown.Value = mEditorItem.Cooldown;

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
                mEditorItem.Consumable.Type = ConsumableType.Health;
                mEditorItem.Consumable.Value = 0;

                mEditorItem.TwoHanded = false;
                mEditorItem.EquipmentSlot = 0;
                mEditorItem.Effect.Type = EffectType.None;
                mEditorItem.Effect.Percentage = 0;

                mEditorItem.SlotCount = 0;

                mEditorItem.Damage = 0;
                mEditorItem.Tool = -1;

                mEditorItem.Spell = null;
                mEditorItem.Event = null;
            }

            if (cmbType.SelectedIndex == (int) ItemTypes.Consumable)
            {
                cmbConsume.SelectedIndex = (int) mEditorItem.Consumable.Type;
                nudInterval.Value = mEditorItem.Consumable.Value;
                nudIntervalPercentage.Value = mEditorItem.Consumable.Percentage;
                grpConsumable.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Spell)
            {
                cmbTeachSpell.SelectedIndex = SpellBase.ListIndex(mEditorItem.SpellId) + 1;
                chkQuickCast.Checked = mEditorItem.QuickCast;
                chkDestroy.Checked = mEditorItem.DestroySpell;
                grpSpell.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Event)
            {
                cmbEvent.SelectedIndex = EventBase.ListIndex(mEditorItem.EventId) + 1;
                grpEvent.Visible = true;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Equipment)
            {
                grpEquipment.Visible = true;
                if (mEditorItem.EquipmentSlot < -1 || mEditorItem.EquipmentSlot >= cmbEquipmentSlot.Items.Count)
                {
                    mEditorItem.EquipmentSlot = 0;
                }

                cmbEquipmentSlot.SelectedIndex = mEditorItem.EquipmentSlot;
                cmbEquipmentBonus.SelectedIndex = (int) mEditorItem.Effect.Type;
            }
            else if (cmbType.SelectedIndex == (int) ItemTypes.Bag)
            {
                // Cant have no space or negative space.
                mEditorItem.SlotCount = Math.Max(1, mEditorItem.SlotCount);
                grpBags.Visible = true;
                nudBag.Value = mEditorItem.SlotCount;
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
            if (lstItems.SelectedNode != null && lstItems.SelectedNode.Tag != null)
            {
                lstItems.SelectedNode.Text = txtName.Text;
            }

            mChangingName = false;
        }

        private void cmbPic_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Icon = cmbPic.SelectedIndex < 1 ? null : cmbPic.Text;
            picItem.BackgroundImage?.Dispose();
            picItem.BackgroundImage = null;
            if (cmbPic.SelectedIndex > 0)
            {
                picItem.BackgroundImage = System.Drawing.Image.FromFile("resources/items/" + cmbPic.Text);
            }
        }

        private void cmbConsume_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Type = (ConsumableType) cmbConsume.SelectedIndex;
        }

        private void cmbPaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.MalePaperdoll = TextUtils.SanitizeNone(cmbMalePaperdoll.Text);
            picMalePaperdoll.BackgroundImage?.Dispose();
            picMalePaperdoll.BackgroundImage = null;
            if (cmbMalePaperdoll.SelectedIndex > 0)
            {
                picMalePaperdoll.BackgroundImage =
                    System.Drawing.Image.FromFile("resources/paperdolls/" + cmbMalePaperdoll.Text);
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            mEditorItem.Description = txtDesc.Text;
        }

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EquipmentSlot = cmbEquipmentSlot.SelectedIndex;
            if (cmbEquipmentSlot.SelectedIndex == Options.WeaponIndex)
            {
                grpWeaponProperties.Show();
            }
            else
            {
                grpWeaponProperties.Hide();

                mEditorItem.Projectile = null;
                mEditorItem.Tool = -1;
                mEditorItem.Damage = 0;
                mEditorItem.TwoHanded = false;
            }
        }

        private void cmbToolType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Tool = cmbToolType.SelectedIndex - 1;
        }

        private void cmbEquipmentBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Effect.Type = (EffectType) cmbEquipmentBonus.SelectedIndex;
        }

        private void chk2Hand_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.TwoHanded = chk2Hand.Checked;
        }

        private void FrmItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CurrentEditor = -1;
        }

        private void cmbFemalePaperdoll_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.FemalePaperdoll = TextUtils.SanitizeNone(cmbFemalePaperdoll.Text);
            picFemalePaperdoll.BackgroundImage?.Dispose();
            picFemalePaperdoll.BackgroundImage = null;
            if (cmbFemalePaperdoll.SelectedIndex > 0)
            {
                picFemalePaperdoll.BackgroundImage =
                    System.Drawing.Image.FromFile("resources/paperdolls/" + cmbFemalePaperdoll.Text);
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
                if (DarkMessageBox.ShowWarning(
                        Strings.ItemEditor.deleteprompt, Strings.ItemEditor.deletetitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
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
                mEditorItem.Load(mCopiedItem, true);
                UpdateEditor();
            }
        }

        private void toolStripItemUndo_Click(object sender, EventArgs e)
        {
            if (mChanged.Contains(mEditorItem) && mEditorItem != null)
            {
                if (DarkMessageBox.ShowWarning(
                        Strings.ItemEditor.undoprompt, Strings.ItemEditor.undotitle, DarkDialogButton.YesNo,
                        Properties.Resources.Icon
                    ) ==
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
            mEditorItem.AttackAnimation =
                AnimationBase.Get(AnimationBase.IdFromList(cmbAttackAnimation.SelectedIndex - 1));
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
            mEditorItem.Projectile = ProjectileBase.Get(ProjectileBase.IdFromList(cmbProjectile.SelectedIndex - 1));
        }

        private void btnEditRequirements_Click(object sender, EventArgs e)
        {
            var frm = new FrmDynamicRequirements(mEditorItem.UsageRequirements, RequirementType.Item);
            frm.ShowDialog();
        }

        private void cmbAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Animation = AnimationBase.Get(AnimationBase.IdFromList(cmbAnimation.SelectedIndex - 1));
        }

        private void cmbEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Event = EventBase.Get(EventBase.IdFromList(cmbEvent.SelectedIndex - 1));
        }

        private void cmbTeachSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Spell = SpellBase.Get(SpellBase.IdFromList(cmbTeachSpell.SelectedIndex - 1));
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
            mEditorItem.Effect.Percentage = (int) nudEffectPercent.Value;
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

        private void nudStrPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[0] = (int) nudStrPercentage.Value;
        }

        private void nudMagPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[1] = (int) nudMagPercentage.Value;
        }

        private void nudDefPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[2] = (int) nudDefPercentage.Value;
        }

        private void nudMRPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[3] = (int) nudMRPercentage.Value;
        }

        private void nudSpdPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageStatsGiven[4] = (int) nudSpdPercentage.Value;
        }

        private void nudBag_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.SlotCount = (int) nudBag.Value;
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Value = (int) nudInterval.Value;
        }

        private void nudIntervalPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Consumable.Percentage = (int) nudIntervalPercentage.Value;
        }

        private void chkBound_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Bound = chkBound.Checked;
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.Stackable = chkStackable.Checked;
        }

        private void nudCritMultiplier_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.CritMultiplier = (double) nudCritMultiplier.Value;
        }

        private void nudCooldown_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.Cooldown = (int) nudCooldown.Value;
        }

        private void nudHealthBonus_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsGiven[0] = (int) nudHealthBonus.Value;
        }

        private void nudManaBonus_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsGiven[1] = (int) nudManaBonus.Value;
        }

        private void nudHPPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageVitalsGiven[0] = (int) nudHPPercentage.Value;
        }

        private void nudMPPercentage_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.PercentageVitalsGiven[1] = (int) nudMPPercentage.Value;
        }

        private void nudHPRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsRegen[0] = (int) nudHPRegen.Value;
        }

        private void nudMpRegen_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.VitalsRegen[1] = (int) nudMpRegen.Value;
        }

        private void cmbEquipmentAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.EquipmentAnimation =
                AnimationBase.Get(AnimationBase.IdFromList(cmbEquipmentAnimation.SelectedIndex - 1));
        }

        private void cmbAttackSpeedModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedModifier = cmbAttackSpeedModifier.SelectedIndex;
            nudAttackSpeedValue.Enabled = cmbAttackSpeedModifier.SelectedIndex > 0;
        }

        private void nudAttackSpeedValue_ValueChanged(object sender, EventArgs e)
        {
            mEditorItem.AttackSpeedValue = (int) nudAttackSpeedValue.Value;
        }

        private void chkQuickCast_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.QuickCast = chkQuickCast.Checked;
        }

        private void chkDestroy_CheckedChanged(object sender, EventArgs e)
        {
            mEditorItem.DestroySpell = chkDestroy.Checked;
        }

        private void cmbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Rarity = cmbRarity.SelectedIndex;
        }

        #region "Item List - Folders, Searching, Sorting, Etc"

        public void InitEditor()
        {
            var selectedId = Guid.Empty;
            var folderNodes = new Dictionary<string, TreeNode>();
            if (lstItems.SelectedNode != null && lstItems.SelectedNode.Tag != null)
            {
                selectedId = (Guid) lstItems.SelectedNode.Tag;
            }

            lstItems.Nodes.Clear();

            cmbEquipmentSlot.Items.Clear();
            cmbEquipmentSlot.Items.AddRange(Options.EquipmentSlots.ToArray());
            cmbToolType.Items.Clear();
            cmbToolType.Items.Add(Strings.General.none);
            cmbToolType.Items.AddRange(Options.ToolTypes.ToArray());
            cmbEquipmentBonus.Items.Clear();
            for (var i = 0; i < Strings.ItemEditor.bonuseffects.Count; i++)
            {
                cmbEquipmentBonus.Items.Add(Strings.ItemEditor.bonuseffects[i]);
            }

            cmbProjectile.Items.Clear();
            cmbProjectile.Items.Add(Strings.General.none);
            cmbProjectile.Items.AddRange(ProjectileBase.Names);

            //Collect folders
            var mFolders = new List<string>();
            foreach (var itm in ItemBase.Lookup)
            {
                if (!string.IsNullOrEmpty(((ItemBase) itm.Value).Folder) &&
                    !mFolders.Contains(((ItemBase) itm.Value).Folder))
                {
                    mFolders.Add(((ItemBase) itm.Value).Folder);
                    if (!mKnownFolders.Contains(((ItemBase) itm.Value).Folder))
                    {
                        mKnownFolders.Add(((ItemBase) itm.Value).Folder);
                    }
                }
            }

            mFolders.Sort();
            mKnownFolders.Sort();
            cmbFolder.Items.Clear();
            cmbFolder.Items.Add("");
            cmbFolder.Items.AddRange(mKnownFolders.ToArray());

            lstItems.Sorted = !btnChronological.Checked;

            if (!btnChronological.Checked && !CustomSearch())
            {
                foreach (var folder in mFolders)
                {
                    var node = lstItems.Nodes.Add(folder);
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                    folderNodes.Add(folder, node);
                }
            }

            foreach (var itm in ItemBase.ItemPairs)
            {
                var node = new TreeNode(itm.Value);
                node.Tag = itm.Key;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;

                var folder = ItemBase.Get(itm.Key).Folder;
                if (!string.IsNullOrEmpty(folder) && !btnChronological.Checked && !CustomSearch())
                {
                    var folderNode = folderNodes[folder];
                    folderNode.Nodes.Add(node);
                    if (itm.Key == selectedId)
                    {
                        folderNode.Expand();
                    }
                }
                else
                {
                    lstItems.Nodes.Add(node);
                }

                if (CustomSearch())
                {
                    if (!node.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                    {
                        node.Remove();
                    }
                }

                if (itm.Key == selectedId)
                {
                    lstItems.SelectedNode = node;
                }
            }

            var selectedNode = lstItems.SelectedNode;

            if (!btnChronological.Checked)
            {
                lstItems.Sort();
            }

            lstItems.SelectedNode = selectedNode;
            foreach (var node in mExpandedFolders)
            {
                if (folderNodes.ContainsKey(node))
                {
                    folderNodes[node].Expand();
                }
            }
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            var folderName = "";
            var result = DarkInputBox.ShowInformation(
                Strings.ItemEditor.folderprompt, Strings.ItemEditor.foldertitle, ref folderName,
                DarkDialogButton.OkCancel
            );

            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderName))
            {
                if (!cmbFolder.Items.Contains(folderName))
                {
                    mEditorItem.Folder = folderName;
                    mExpandedFolders.Add(folderName);
                    InitEditor();
                    cmbFolder.Text = folderName;
                }
            }
        }

        private void lstItems_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if (node != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.Node.Tag != null && e.Node.Tag.GetType() == typeof(Guid))
                    {
                        Clipboard.SetText(e.Node.Tag.ToString());
                    }
                }

                var hitTest = lstItems.HitTest(e.Location);
                if (hitTest.Location != TreeViewHitTestLocations.PlusMinus)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                        {
                            node.Collapse();
                        }
                        else
                        {
                            node.Expand();
                        }
                    }
                }

                if (node.IsExpanded)
                {
                    if (!mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Add(node.Text);
                    }
                }
                else
                {
                    if (mExpandedFolders.Contains(node.Text))
                    {
                        mExpandedFolders.Remove(node.Text);
                    }
                }
            }
        }

        private void lstItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (mChangingName)
            {
                return;
            }

            if (lstItems.SelectedNode == null || lstItems.SelectedNode.Tag == null)
            {
                return;
            }

            mEditorItem = ItemBase.Get((Guid) lstItems.SelectedNode.Tag);
            UpdateEditor();
        }

        private void cmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            mEditorItem.Folder = cmbFolder.Text;
            InitEditor();
        }

        private void btnChronological_Click(object sender, EventArgs e)
        {
            btnChronological.Checked = !btnChronological.Checked;
            InitEditor();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            InitEditor();
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = Strings.ItemEditor.searchplaceholder;
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = Strings.ItemEditor.searchplaceholder;
        }

        private bool CustomSearch()
        {
            return !string.IsNullOrWhiteSpace(txtSearch.Text) && txtSearch.Text != Strings.ItemEditor.searchplaceholder;
        }

        private void txtSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == Strings.ItemEditor.searchplaceholder)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

    }

}
